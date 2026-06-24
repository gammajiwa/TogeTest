using System.Collections.Generic;
using UnityEngine;

namespace Toge.Data
{
    /// <summary>Static base stats shared by players and enemies.</summary>
    public abstract class EntityDataSO : ScriptableObject
    {
        [Header("Identity")]
        public string displayName;

        [Header("Base Stats")]
        public int maxHealth = 100;
        public int attack = 10;
        public int defense = 5;
        public int speed = 10;

        [Header("Moves")]
        public List<MoveSO> moves = new();
    }
}
