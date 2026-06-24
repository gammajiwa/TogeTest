using UnityEngine;
using UnityEngine.Events;

namespace Toge.Events
{
    /// <summary>Generic base for typed, decoupled event channels.</summary>
    public abstract class EventChannelSO<T> : ScriptableObject
    {
        public event UnityAction<T> OnEventRaised;

        public void RaiseEvent(T value) => OnEventRaised?.Invoke(value);
    }
}
