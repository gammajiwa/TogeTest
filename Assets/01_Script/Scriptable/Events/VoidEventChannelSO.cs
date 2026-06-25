using UnityEngine;
using UnityEngine.Events;

namespace Toge.Events
{
    [CreateAssetMenu(menuName = "Toge/Events/Void Event Channel", fileName = "VoidEventChannel")]
    public class VoidEventChannelSO : ScriptableObject
    {
        public event UnityAction OnEventRaised;

        public void RaiseEvent() => OnEventRaised?.Invoke();
    }
}
