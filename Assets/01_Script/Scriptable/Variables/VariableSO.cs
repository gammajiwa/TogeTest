using UnityEngine;
using UnityEngine.Events;

namespace Toge.Variables
{
    public abstract class VariableSO<T> : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField] protected T initialValue;

        [System.NonSerialized] private T _runtimeValue;

        public event UnityAction<T> OnChanged;

        public T Value
        {
            get => _runtimeValue;
            set
            {
                _runtimeValue = value;
                OnChanged?.Invoke(value);
            }
        }

        public void ResetValue() => Value = initialValue;

        void ISerializationCallbackReceiver.OnAfterDeserialize() => _runtimeValue = initialValue;
        void ISerializationCallbackReceiver.OnBeforeSerialize() { }
    }
}
