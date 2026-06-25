using UnityEngine;
using Toge.Variables;

namespace Toge.Core
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] private TransformAnchorSO _targetAnchor;
        [SerializeField] private Vector3 _offset = new Vector3(0f, 8f, -6f);
        [SerializeField] private float _smoothTime = 0.18f;

        private Vector3 _velocity;

        private void LateUpdate()
        {
            if (_targetAnchor == null || !_targetAnchor.IsSet) return;

            Vector3 p = _targetAnchor.Value.position;
            Vector3 target = new Vector3(p.x + _offset.x, _offset.y, p.z + _offset.z);
            transform.position = Vector3.SmoothDamp(transform.position, target, ref _velocity, _smoothTime);
        }
    }
}
