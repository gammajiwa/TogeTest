using UnityEngine;

namespace Toge.Data
{
    [CreateAssetMenu(menuName = "Toge/Data/Character Animation Set", fileName = "NewAnimationSet")]
    public class CharacterAnimationSetSO : ScriptableObject
    {
        [Header("Animation Names")]
        public string idleAnimation = "idle";
        public string moveAnimation = "move";
        public string runAnimation = "run";
        public string jumpAnimation = "skill_movement_fly";
        public string attackAnimation = "attack001";

        [Header("Locomotion Thresholds")]
        public float moveThreshold = 0.1f;
        public float runThreshold = 6.5f;
    }
}
