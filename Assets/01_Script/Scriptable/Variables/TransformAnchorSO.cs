using UnityEngine;
using UnityEngine.Events;

namespace Toge.Variables
{
    [CreateAssetMenu(menuName = "Toge/Variables/Transform Anchor", fileName = "TransformAnchor")]
    public class TransformAnchorSO : ScriptableObject
    {
        [System.NonSerialized] private Transform _value;

        public event UnityAction<Transform> OnProvided;
        public event UnityAction OnCleared;

        public Transform Value => _value;
        public bool IsSet => _value != null;

        public void Provide(Transform value)
        {
            _value = value;
            OnProvided?.Invoke(value);
        }

        public void Unset()
        {
            _value = null;
            OnCleared?.Invoke();
        }
    }
}
