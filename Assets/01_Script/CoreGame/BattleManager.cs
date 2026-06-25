using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Toge.Data;
using Toge.Enums;
using Toge.Events;

namespace Toge.Battle
{
    public class BattleManager : MonoBehaviour
    {
        [SerializeField] private List<BattleUnit> _playerUnits = new();
        [SerializeField] private List<BattleUnit> _enemyUnits = new();
        [SerializeField] private BattleResultEventChannelSO _resultChannel;
        [SerializeField] private float _turnDelay = 0.6f;

        private readonly List<BattleUnit> _all = new();
        private BattleAction _submitted;

        public event Action<BattleUnit> TurnStarted;
        public event Action StateChanged;
        public event Action<BattleResult> BattleEnded;

        public BattleUnit ActingUnit { get; private set; }
        public bool AwaitingInput { get; private set; }

        public void Begin(IEnumerable<BattleUnit> players, IEnumerable<BattleUnit> enemies)
        {
            _playerUnits = new List<BattleUnit>(players);
            _enemyUnits = new List<BattleUnit>(enemies);
            StartCoroutine(RunBattle());
        }

        public void SubmitAction(BattleAction action) => _submitted = action;

        public BattleUnit FirstLivingEnemy()
        {
            foreach (var unit in _enemyUnits)
                if (unit.IsAlive) return unit;
            return null;
        }

        private IEnumerator RunBattle()
        {
            _all.Clear();
            _all.AddRange(_playerUnits);
            _all.AddRange(_enemyUnits);
            StateChanged?.Invoke();

            while (!IsDefeated(BattleTeam.Player) && !IsDefeated(BattleTeam.Enemy))
            {
                foreach (var unit in TurnOrder())
                {
                    if (!unit.IsAlive) continue;

                    ActingUnit = unit;
                    TurnStarted?.Invoke(unit);

                    BattleAction action;
                    if (unit.Team == BattleTeam.Player)
                    {
                        AwaitingInput = true;
                        _submitted = null;
                        yield return new WaitUntil(() => _submitted != null);
                        AwaitingInput = false;
                        action = _submitted;
                    }
                    else
                    {
                        action = DecideEnemyAction(unit);
                    }

                    if (action != null) Execute(action);
                    StateChanged?.Invoke();
                    yield return new WaitForSeconds(_turnDelay);

                    if (IsDefeated(BattleTeam.Player) || IsDefeated(BattleTeam.Enemy)) break;
                }
            }

            BattleResult result = IsDefeated(BattleTeam.Enemy) ? BattleResult.Win : BattleResult.Lose;
            Debug.Log($"[Battle] {result}");
            BattleEnded?.Invoke(result);
            if (_resultChannel != null) _resultChannel.RaiseEvent(result);
        }

        private List<BattleUnit> TurnOrder()
        {
            var alive = _all.FindAll(u => u.IsAlive);
            alive.Sort((a, b) => b.Speed.CompareTo(a.Speed));
            return alive;
        }

        private BattleAction DecideEnemyAction(BattleUnit unit)
        {
            List<BattleUnit> targets = LivingOpponents(unit.Team);
            if (targets.Count == 0) return null;

            if (unit.Data is EnemyDataSO enemy && enemy.strategy != null)
                return enemy.strategy.DecideAction(unit, targets);

            return new BattleAction(BattleActionType.Attack, unit, targets[0]);
        }

        private void Execute(BattleAction action)
        {
            if (action.Type == BattleActionType.Defend) return;
            if (action.Target == null || !action.Target.IsAlive) return;

            int power = action.Type == BattleActionType.Card && action.Card != null
                ? action.Card.power
                : action.Actor.Attack;

            int damage = Mathf.Max(1, power - action.Target.Defense / 2);
            action.Target.TakeDamage(damage);

            string label = action.Type == BattleActionType.Card && action.Card != null ? action.Card.cardName : "Attack";
            Debug.Log($"[Battle] {action.Actor.DisplayName} plays {label} on {action.Target.DisplayName}: {damage} " +
                      $"(HP {action.Target.CurrentHealth}/{action.Target.MaxHealth})");
        }

        private List<BattleUnit> LivingOpponents(BattleTeam team) => _all.FindAll(u => u.Team != team && u.IsAlive);

        private bool IsDefeated(BattleTeam team) => !_all.Exists(u => u.Team == team && u.IsAlive);
    }
}
