using UnityEngine;
using Toge.Enums;

namespace Toge.Events
{
    [CreateAssetMenu(menuName = "Toge/Events/GameState Event Channel", fileName = "GameStateEventChannel")]
    public class GameStateEventChannelSO : EventChannelSO<GameState> { }
}
