using UnityEngine;
using Toge.Enums;
using Toge.Events;

namespace Toge.Core
{
    /// <summary>Tracks the current game state and broadcasts changes over an event channel.</summary>
    public class GameStateManager : MonoBehaviour
    {
        [SerializeField] private GameStateEventChannelSO _stateChannel;
        [SerializeField] private GameState _initialState = GameState.Boot;

        public GameState CurrentState { get; private set; }

        private void Start()
        {
            CurrentState = _initialState;
            _stateChannel?.RaiseEvent(_initialState);
        }

        public void RequestState(GameState next)
        {
            if (next == CurrentState) return;
            CurrentState = next;
            _stateChannel?.RaiseEvent(next);
        }
    }
}
