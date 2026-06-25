using UnityEngine;
using Toge.Data;
using Toge.Interfaces;

namespace Toge.Battle
{
    public enum BattleTeam { Player, Enemy }

    public class BattleUnit : MonoBehaviour, IDamageable
    {
        [SerializeField] private EntityDataSO _data;
        [SerializeField] private BattleTeam _team;

        public EntityDataSO Data => _data;
        public BattleTeam Team => _team;
        public int MaxHealth { get; private set; }
        public int CurrentHealth { get; private set; }
        public int Attack => _data != null ? _data.attack : 0;
        public int Defense => _data != null ? _data.defense : 0;
        public int Speed => _data != null ? _data.speed : 0;
        public bool IsAlive => CurrentHealth > 0;
        public string DisplayName => _data != null && !string.IsNullOrEmpty(_data.displayName) ? _data.displayName : name;

        private void Awake()
        {
            if (MaxHealth == 0 && _data != null)
            {
                MaxHealth = _data.maxHealth;
                CurrentHealth = MaxHealth;
            }
        }

        public void Init(EntityDataSO data, BattleTeam team)
        {
            _data = data;
            _team = team;
            MaxHealth = data.maxHealth;
            CurrentHealth = MaxHealth;
        }

        public void TakeDamage(int amount)
        {
            CurrentHealth = Mathf.Max(0, CurrentHealth - Mathf.Max(0, amount));
        }
    }
}
