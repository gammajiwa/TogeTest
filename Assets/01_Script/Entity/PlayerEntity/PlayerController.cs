using UnityEngine;
using UnityEngine.InputSystem;
using Toge.Data;
using Toge.Variables;

namespace Toge.Entities
{
    [RequireComponent(typeof(CharacterController), typeof(PlayerInteractor))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float _walkSpeed = 5f;
        [SerializeField] private float _runSpeed = 9f;
        [SerializeField] private float _jumpHeight = 1.8f;
        [SerializeField] private float _gravity = -25f;
        [SerializeField] private float _attackRadius = 1.7f;

        [Header("References")]
        [SerializeField] private InputMapSO _input;
        [SerializeField] private TransformAnchorSO _playerAnchor;
        [SerializeField] private Transform _visual;
        [SerializeField] private PlayerSpineAnimator _animator;
        [SerializeField] private PlayerInteractor _interactor;

        private CharacterController _controller;
        private float _verticalVelocity;
        private Vector2? _scriptedMove;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            if (_animator == null) _animator = GetComponentInChildren<PlayerSpineAnimator>();
            if (_interactor == null) _interactor = GetComponent<PlayerInteractor>();
        }

        private void OnEnable()
        {
            if (_playerAnchor != null) _playerAnchor.Provide(transform);
        }

        private void OnDisable()
        {
            if (_playerAnchor != null && _playerAnchor.Value == transform)
                _playerAnchor.Unset();
        }

        public void SetScriptedMove(Vector2? move) => _scriptedMove = move;

        private void Update()
        {
            if (!_scriptedMove.HasValue) HandleActionInput();

            bool attacking = _animator != null && _animator.IsAttacking;
            Vector2 input = attacking ? Vector2.zero : (_scriptedMove ?? ReadMove());

            Vector3 horizontal = new Vector3(input.x, 0f, input.y);
            if (horizontal.sqrMagnitude > 1f) horizontal.Normalize();
            horizontal *= IsRunHeld() ? _runSpeed : _walkSpeed;

            ApplyGravity();
            _controller.Move((horizontal + Vector3.up * _verticalVelocity) * Time.deltaTime);

            FaceMovement(input.x);
        }

        private void HandleActionInput()
        {
            if (Pressed(_input.interact) && _interactor != null)
                _interactor.TryInteract();

            if (Pressed(_input.jump))
            {
                bool interacted = _interactor != null && _interactor.TryInteract();
                if (!interacted) TryJump();
            }

            if (Pressed(_input.attack) && _animator != null)
            {
                _animator.PlayAttack();
                AttackHit();
            }
        }

        private void AttackHit()
        {
            if (Toge.Core.AudioManager.Instance != null) Toge.Core.AudioManager.Instance.PlayAttack();

            Collider[] hits = Physics.OverlapSphere(transform.position, _attackRadius);
            foreach (Collider hit in hits)
                if (hit.TryGetComponent(out Toge.Core.DestructibleObject destructible))
                    destructible.Hit(1);
        }

        private void TryJump()
        {
            if (_controller.isGrounded)
                _verticalVelocity = Mathf.Sqrt(_jumpHeight * -2f * _gravity);
        }

        private void ApplyGravity()
        {
            if (_controller.isGrounded && _verticalVelocity < 0f)
                _verticalVelocity = -2f;
            _verticalVelocity += _gravity * Time.deltaTime;
        }

        private Vector2 ReadMove()
        {
            Keyboard kb = Keyboard.current;
            if (kb == null) return Vector2.zero;

            float x = (Held(_input.moveRight) || kb.rightArrowKey.isPressed ? 1f : 0f)
                    - (Held(_input.moveLeft) || kb.leftArrowKey.isPressed ? 1f : 0f);
            float y = (Held(_input.moveUp) || kb.upArrowKey.isPressed ? 1f : 0f)
                    - (Held(_input.moveDown) || kb.downArrowKey.isPressed ? 1f : 0f);
            return new Vector2(x, y);
        }

        private bool IsRunHeld() => Held(_input.run);

        private static bool Held(Key key)
        {
            Keyboard kb = Keyboard.current;
            return kb != null && kb[key].isPressed;
        }

        private static bool Pressed(Key key)
        {
            Keyboard kb = Keyboard.current;
            return kb != null && kb[key].wasPressedThisFrame;
        }

        private void FaceMovement(float x)
        {
            if (_visual == null || Mathf.Abs(x) < 0.01f) return;
            Vector3 scale = _visual.localScale;
            scale.x = Mathf.Abs(scale.x) * Mathf.Sign(x);
            _visual.localScale = scale;
        }
    }
}
