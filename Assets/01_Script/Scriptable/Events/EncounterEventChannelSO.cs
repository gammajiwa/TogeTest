using UnityEngine;
using Toge.Data;

namespace Toge.Events
{
    /// <summary>Raised to start a battle with the given encounter.</summary>
    [CreateAssetMenu(menuName = "Toge/Events/Encounter Event Channel", fileName = "EncounterEventChannel")]
    public class EncounterEventChannelSO : EventChannelSO<EncounterSO> { }
}
