using UnityEngine;
using Toge.Enums;

namespace Toge.Events
{
    [CreateAssetMenu(menuName = "Toge/Events/Battle Result Event Channel", fileName = "BattleResultEventChannel")]
    public class BattleResultEventChannelSO : EventChannelSO<BattleResult> { }
}
