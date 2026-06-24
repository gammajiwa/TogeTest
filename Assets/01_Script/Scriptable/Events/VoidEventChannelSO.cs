using UnityEngine;
using UnityEngine.Events;

namespace Toge.Events
{
    /// <summary>Parameterless event channel for decoupled signals.</summary>
    [CreateAssetMenu(menuName = "Toge/Events/Void Event Channel", fileName = "VoidEventChannel")]
    public class VoidEventChannelSO : ScriptableObject
    {
        public event UnityAction OnEventRaised;

        public void RaiseEvent() => OnEventRaised?.Invoke();
    }
}
