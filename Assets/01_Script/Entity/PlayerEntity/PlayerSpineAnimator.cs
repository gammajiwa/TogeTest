using UnityEngine;
using Spine.Unity;
using Toge.Data;

namespace Toge.Entities
{
    [RequireComponent(typeof(SkeletonAnimation))]
    public class PlayerSpineAnimator : MonoBehaviour
    {
        [SerializeField] private CharacterAnimationSetSO _animations;
        [SerializeField] private CharacterController _controller;

        private SkeletonAnimation _skeletonAnimation;
        private string _currentLocomotion;

        public bool IsAttacking { get; private set; }

        private void Awake()
        {
            _skeletonAnimation = GetComponent<SkeletonAnimation>();
            if (_controller == null) _controller = GetComponentInParent<CharacterController>();
        }

        private void Update()
        {
            if (IsAttacking) return;

            if (!_controller.isGrounded)
            {
                SetLocomotion(_animations.jumpAnimation, false);
                return;
            }

            float speed = new Vector2(_controller.velocity.x, _controller.velocity.z).magnitude;
            string next = speed < _animations.moveThreshold ? _animations.idleAnimation
                        : speed < _animations.runThreshold ? _animations.moveAnimation
                        : _animations.runAnimation;
            SetLocomotion(next, true);
        }

        private void SetLocomotion(string animationName, bool loop)
        {
            if (animationName == _currentLocomotion) return;
            _currentLocomotion = animationName;
            _skeletonAnimation.AnimationState.SetAnimation(0, animationName, loop);
        }

        public void PlayAttack()
        {
            if (IsAttacking) return;

            IsAttacking = true;
            _currentLocomotion = null;
            var entry = _skeletonAnimation.AnimationState.SetAnimation(0, _animations.attackAnimation, false);
            entry.Complete += _ => IsAttacking = false;
        }
    }
}
