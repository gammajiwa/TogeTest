using System.Collections.Generic;
using UnityEngine;
using Toge.Enums;

namespace Toge.Data
{
    [CreateAssetMenu(menuName = "Toge/Data/Loot Table", fileName = "LootTable")]
    public class LootTableSO : ScriptableObject
    {
        public List<CardSO> pool = new();

        public CardSO Roll()
        {
            if (pool == null || pool.Count == 0) return null;

            int total = 0;
            foreach (CardSO card in pool)
                if (card != null) total += Weight(card.rarity);

            if (total <= 0) return null;

            int roll = Random.Range(0, total);
            foreach (CardSO card in pool)
            {
                if (card == null) continue;
                roll -= Weight(card.rarity);
                if (roll < 0) return card;
            }
            return pool[pool.Count - 1];
        }

        private static int Weight(CardRarity rarity)
        {
            switch (rarity)
            {
                case CardRarity.Rare: return 30;
                case CardRarity.Epic: return 12;
                case CardRarity.Legendary: return 4;
                default: return 60;
            }
        }
    }
}
