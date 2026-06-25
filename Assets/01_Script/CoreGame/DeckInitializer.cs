using UnityEngine;
using Toge.Data;
using Toge.Variables;

namespace Toge.Core
{
    public class DeckInitializer : MonoBehaviour
    {
        [SerializeField] private RuntimeDeckSO _deck;
        [SerializeField] private PlayerDataSO _source;

        private void Awake()
        {
            if (_deck != null && _source != null && !_deck.IsInitialized)
                _deck.Initialize(_source.cards);
        }
    }
}
