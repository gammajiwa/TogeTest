using UnityEngine;

namespace Toge.Core
{
    public class PatrolMover : MonoBehaviour
    {
        [SerializeField] private Transform _visual;
        [SerializeField] private float _range = 3f;
        [SerializeField] private float _speed = 1.5f;

        private Vector3 _start;
        private int _direction = 1;

        private void Start()
        {
            _start = transform.position;
            Face();
        }

        private void Update()
        {
            if (_visual != null && !_visual.gameObject.activeSelf) return;

            transform.position += Vector3.right * (_direction * _speed * Time.deltaTime);

            float offset = transform.position.x - _start.x;
            if (offset > _range && _direction > 0)
            {
                _direction = -1;
                Face();
            }
            else if (offset < -_range && _direction < 0)
            {
                _direction = 1;
                Face();
            }
        }

        private void Face()
        {
            if (_visual == null) return;
            Vector3 scale = _visual.localScale;
            scale.x = Mathf.Abs(scale.x) * (_direction > 0 ? 1 : -1);
            _visual.localScale = scale;
        }
    }
}
