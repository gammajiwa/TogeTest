using UnityEngine;
using Spine.Unity;
using Toge.Battle;

namespace Toge.Core
{
    public class PatrolMover : MonoBehaviour
    {
        [SerializeField] private Transform _visual;
        [SerializeField] private bool _facesRight = true;
        [SerializeField] private float _range = 3f;
        [SerializeField] private float _speed = 1.5f;

        private Vector3 _start;
        private int _direction = 1;

        private void Start()
        {
            _start = transform.position;
            PlayMove();
            Face();
        }

        private void PlayMove()
        {
            var spine = GetComponentInChildren<SkeletonAnimation>();
            if (spine == null || spine.Skeleton == null) return;

            string move = SpineAnimResolver.ResolveClip(spine, spine.initialSkinName, "move", null);
            if (!string.IsNullOrEmpty(move) && spine.Skeleton.Data.FindAnimation(move) != null)
                spine.AnimationState.SetAnimation(0, move, true);
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
            bool wantRight = _direction > 0;
            Vector3 scale = _visual.localScale;
            scale.x = Mathf.Abs(scale.x) * (wantRight == _facesRight ? 1f : -1f);
            _visual.localScale = scale;
        }
    }
}
