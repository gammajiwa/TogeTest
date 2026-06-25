using UnityEngine;
using Toge.Enums;

namespace Toge.Data
{
    [CreateAssetMenu(menuName = "Toge/Card", fileName = "NewCard")]
    public class CardSO : ScriptableObject
    {
        [Header("Identity")]
        public string cardName;
        [TextArea] public string description;
        public Sprite art;

        [Header("Effect")]
        public CardType type = CardType.Attack;
        public int cost = 1;
        public int power = 20;
        public TargetType targetType = TargetType.SingleEnemy;
    }
}
