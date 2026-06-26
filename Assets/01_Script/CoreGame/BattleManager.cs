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
        [SerializeField] private BattleResultEventChannelSO _resultChannel;
        [SerializeField] private int _maxEnergy = 3;
        [SerializeField] private int _handSize = 5;
        [SerializeField] private float _enemyTurnDelay = 0.7f;
        [SerializeField] private float _drawDelay = 0.1f;
        [SerializeField] private float _projectileTravelTime = 0.28f;

        private BattleUnit _player;
        private readonly List<BattleUnit> _enemies = new();
        private readonly List<CardInstance> _drawPile = new();
        private readonly List<CardInstance> _hand = new();
        private readonly List<CardInstance> _discardPile = new();

        private int _energy;
        private int _block;
        private bool _endTurnRequested;

        public event Action StateChanged;
        public event Action<BattleResult> BattleEnded;
        public event Action<bool> PlayerTurnChanged;

        public int Energy => _energy;
        public int MaxEnergy => _maxEnergy;
        public int Block => _block;
        public int DrawCount => _drawPile.Count;
        public int DiscardCount => _discardPile.Count;
        public bool IsPlayerTurn { get; private set; }
        public BattleUnit Player => _player;
        public IReadOnlyList<BattleUnit> Enemies => _enemies;
        public IReadOnlyList<CardInstance> Hand => _hand;

        public void Begin(BattleUnit player, IEnumerable<BattleUnit> enemies, IEnumerable<CardSO> deck)
        {
            _player = player;
            _enemies.Clear();
            _enemies.AddRange(enemies);

            _drawPile.Clear();
            _hand.Clear();
            _discardPile.Clear();
            foreach (CardSO card in deck)
                if (card != null) _drawPile.Add(new CardInstance(card));
            Shuffle(_drawPile);

            StartCoroutine(Run());
        }

        public BattleUnit FirstLivingEnemy()
        {
            foreach (BattleUnit enemy in _enemies)
                if (enemy.IsAlive) return enemy;
            return null;
        }

        public bool PlayCard(CardInstance card, BattleUnit target)
        {
            if (!IsPlayerTurn || card == null || !_hand.Contains(card)) return false;
            if (_energy < card.Data.cost) return false;

            _energy -= card.Data.cost;
            _hand.Remove(card);
            _discardPile.Add(card);
            if (Toge.Core.AudioManager.Instance != null) Toge.Core.AudioManager.Instance.PlayCard();

            if (card.Data.type == CardType.Defend)
            {
                _block += card.Data.power;
            }
            else
            {
                if (target == null || !target.IsAlive) target = FirstLivingEnemy();
                if (target != null) DeliverAttack(_player, target, Mathf.Max(1, card.Data.power));
            }

            StateChanged?.Invoke();
            return true;
        }

        public void EndTurn()
        {
            if (IsPlayerTurn) _endTurnRequested = true;
        }

        private IEnumerator Run()
        {
            StateChanged?.Invoke();

            while (!IsOver())
            {
                _block = 0;
                _energy = _maxEnergy;
                yield return DrawToHandSize();

                IsPlayerTurn = true;
                PlayerTurnChanged?.Invoke(true);
                StateChanged?.Invoke();

                _endTurnRequested = false;
                yield return new WaitUntil(() => _endTurnRequested || AllEnemiesDead());

                IsPlayerTurn = false;
                PlayerTurnChanged?.Invoke(false);
                yield return new WaitForSeconds(_projectileTravelTime + 0.2f);
                DiscardHand();
                StateChanged?.Invoke();
                if (IsOver()) break;

                yield return EnemyTurn();
                if (IsOver()) break;
            }

            BattleResult result = AllEnemiesDead() ? BattleResult.Win : BattleResult.Lose;
            Debug.Log($"[Battle] {result}");
            BattleEnded?.Invoke(result);
            if (_resultChannel != null) _resultChannel.RaiseEvent(result);
        }

        private IEnumerator EnemyTurn()
        {
            foreach (BattleUnit enemy in _enemies)
            {
                if (!enemy.IsAlive) continue;

                yield return new WaitForSeconds(_enemyTurnDelay);

                DeliverEnemyAttack(enemy);
                yield return new WaitForSeconds(_projectileTravelTime + 0.2f);

                if (!_player.IsAlive) yield break;
            }
        }

        private void DeliverAttack(BattleUnit attacker, BattleUnit target, int damage)
        {
            Lunge(attacker);
            Vector3 from = attacker != null ? attacker.transform.position + Vector3.up * 1.2f : target.transform.position;
            BattleUnit intended = target;

            BattleProjectile.Spawn(from, target.transform, _projectileTravelTime, () =>
            {
                BattleUnit hit = intended != null && intended.IsAlive ? intended : FirstLivingEnemy();
                if (hit == null) return;
                hit.TakeDamage(damage);
                PlayHitFx(hit, damage);
                StateChanged?.Invoke();
            });
        }

        private void DeliverEnemyAttack(BattleUnit enemy)
        {
            Lunge(enemy);
            int incoming = enemy.Attack;
            Vector3 from = enemy.transform.position + Vector3.up * 1.2f;

            BattleProjectile.Spawn(from, _player.transform, _projectileTravelTime, () =>
            {
                if (_player == null) return;
                int absorbed = Mathf.Min(_block, incoming);
                _block -= absorbed;
                int damage = incoming - absorbed;
                if (damage > 0)
                {
                    _player.TakeDamage(damage);
                    PlayHitFx(_player, damage);
                }
                StateChanged?.Invoke();
            });
        }

        private static void Lunge(BattleUnit unit)
        {
            if (unit == null) return;
            UnitHitFx fx = unit.GetComponent<UnitHitFx>();
            if (fx != null) fx.Lunge();
        }

        private static void PlayHitFx(BattleUnit unit, int damage)
        {
            if (unit == null) return;
            UnitHitFx fx = unit.GetComponent<UnitHitFx>();
            if (fx != null) fx.Play(damage);
        }

        private IEnumerator DrawToHandSize()
        {
            while (_hand.Count < _handSize)
            {
                if (_drawPile.Count == 0)
                {
                    if (_discardPile.Count == 0) yield break;
                    ReshuffleDiscardIntoDraw();
                }

                int top = _drawPile.Count - 1;
                CardInstance card = _drawPile[top];
                _drawPile.RemoveAt(top);
                _hand.Add(card);
                StateChanged?.Invoke();
                yield return new WaitForSeconds(_drawDelay);
            }
        }

        private void DiscardHand()
        {
            _discardPile.AddRange(_hand);
            _hand.Clear();
        }

        private void ReshuffleDiscardIntoDraw()
        {
            _drawPile.AddRange(_discardPile);
            _discardPile.Clear();
            Shuffle(_drawPile);
        }

        private static void Shuffle(List<CardInstance> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = UnityEngine.Random.Range(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }

        private bool AllEnemiesDead() => !_enemies.Exists(e => e.IsAlive);

        private bool IsOver() => AllEnemiesDead() || _player == null || !_player.IsAlive;
    }
}
