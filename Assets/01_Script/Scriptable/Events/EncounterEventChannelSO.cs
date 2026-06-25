using UnityEngine;
using Toge.Data;

namespace Toge.Events
{
    [CreateAssetMenu(menuName = "Toge/Events/Encounter Event Channel", fileName = "EncounterEventChannel")]
    public class EncounterEventChannelSO : EventChannelSO<EncounterSO> { }
}
