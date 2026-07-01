using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

namespace Toge.Data
{
    public abstract class EntityDataSO : ScriptableObject
    {
        [Header("Identity")]
        public string displayName;

        [Header("Base Stats")]
        public int maxHealth = 100;
        public int attack = 10;
        public int defense = 5;
        public int speed = 10;

        [Header("Battle Visual (Spine)")]
        public SkeletonDataAsset skeleton;
        public string skin;
        public string idleAnimation = "idle";
        public string attackAnimation = "attack001";
        public string deathAnimation = "";
        public float visualScale = 0.25f;
        public bool facesRight = true;
    }
}
