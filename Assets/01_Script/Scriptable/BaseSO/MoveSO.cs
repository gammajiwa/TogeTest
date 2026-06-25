using UnityEngine;
using Toge.Enums;

namespace Toge.Data
{
    [CreateAssetMenu(menuName = "Toge/Data/Move", fileName = "NewMove")]
    public class MoveSO : ScriptableObject
    {
        [Header("Identity")]
        public string displayName;
        [TextArea] public string description;

        [Header("Effect")]
        public int power = 10;
        public TargetType targetType = TargetType.SingleEnemy;
        [Min(0)] public int spCost;
    }
}
