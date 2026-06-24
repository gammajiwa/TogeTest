using UnityEngine;
using Toge.Enums;

namespace Toge.Events
{
    /// <summary>Raised when a battle ends, carrying its result.</summary>
    [CreateAssetMenu(menuName = "Toge/Events/Battle Result Event Channel", fileName = "BattleResultEventChannel")]
    public class BattleResultEventChannelSO : EventChannelSO<BattleResult> { }
}
