using UnityEngine;
using Toge.Variables;

namespace Toge.Core
{
    /// <summary>Smoothly follows the player (found via a runtime anchor) at a fixed 2.5D angle.</summary>
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] private TransformAnchorSO _targetAnchor;
        [SerializeField] private Vector3 _offset = new Vector3(0f, 8f, -6f);
        [SerializeField] private float _smoothTime = 0.15f;
        [SerializeField] private float _lookHeight = 1f;

        private Vector3 _velocity;

        private void LateUpdate()
        {
            if (_targetAnchor == null || !_targetAnchor.IsSet) return;

            Vector3 targetPos = _targetAnchor.Value.position;
            transform.position = Vector3.SmoothDamp(transform.position, targetPos + _offset, ref _velocity, _smoothTime);
            transform.LookAt(targetPos + Vector3.up * _lookHeight);
        }
    }
}
