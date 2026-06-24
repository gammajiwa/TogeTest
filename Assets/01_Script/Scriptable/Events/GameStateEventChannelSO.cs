using UnityEngine;
using Toge.Enums;

namespace Toge.Events
{
    /// <summary>Broadcasts game-state transitions.</summary>
    [CreateAssetMenu(menuName = "Toge/Events/GameState Event Channel", fileName = "GameStateEventChannel")]
    public class GameStateEventChannelSO : EventChannelSO<GameState> { }
}
