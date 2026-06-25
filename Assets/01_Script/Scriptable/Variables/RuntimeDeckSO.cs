using System.Collections.Generic;
using UnityEngine;
using Toge.Data;

namespace Toge.Variables
{
    [CreateAssetMenu(menuName = "Toge/Variables/Runtime Deck", fileName = "RuntimeDeck")]
    public class RuntimeDeckSO : ScriptableObject
    {
        [System.NonSerialized] private List<CardSO> _cards;

        public bool IsInitialized => _cards != null;
        public IReadOnlyList<CardSO> Cards => _cards;

        public void Initialize(IEnumerable<CardSO> baseCards)
        {
            _cards = new List<CardSO>();
            if (baseCards != null) _cards.AddRange(baseCards);
        }

        public void Add(CardSO card)
        {
            if (_cards == null) _cards = new List<CardSO>();
            if (card != null) _cards.Add(card);
        }
    }
}
