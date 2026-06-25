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
        public CardRarity rarity = CardRarity.Common;

        [Header("Effect")]
        public CardType type = CardType.Attack;
        public int cost = 1;
        public int power = 20;
        public TargetType targetType = TargetType.SingleEnemy;

        public static Color RarityColor(CardRarity rarity)
        {
            switch (rarity)
            {
                case CardRarity.Rare: return new Color(0.35f, 0.55f, 0.95f);
                case CardRarity.Epic: return new Color(0.65f, 0.35f, 0.85f);
                case CardRarity.Legendary: return new Color(0.95f, 0.72f, 0.2f);
                default: return new Color(0.72f, 0.72f, 0.76f);
            }
        }
    }
}
