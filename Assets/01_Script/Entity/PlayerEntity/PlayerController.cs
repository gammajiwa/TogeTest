using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using Toge.Variables;

namespace Toge.Entities
{
    /// <summary>Top-down WASD/arrow movement on the XZ plane for the overworld.</summary>
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private TransformAnchorSO _playerAnchor;
        [SerializeField] private Transform _visual;

        private CharacterController _controller;

        private void Awake() => _controller = GetComponent<CharacterController>();

        private void OnEnable() => _playerAnchor?.Provide(transform);

        private void OnDisable()
        {
            if (_playerAnchor != null && _playerAnchor.Value == transform)
                _playerAnchor.Unset();
        }

        private void Update()
        {
            Vector2 input = ReadInput();
            Vector3 move = new Vector3(input.x, 0f, input.y);
            if (move.sqrMagnitude > 1f) move.Normalize();

            _controller.SimpleMove(move * _moveSpeed);
            FaceMovement(input.x);
        }

        private static Vector2 ReadInput()
        {
            Keyboard kb = Keyboard.current;
            if (kb == null) return Vector2.zero;

            float x = Held(kb.dKey, kb.rightArrowKey) - Held(kb.aKey, kb.leftArrowKey);
            float y = Held(kb.wKey, kb.upArrowKey) - Held(kb.sKey, kb.downArrowKey);
            return new Vector2(x, y);
        }

        private static float Held(KeyControl a, KeyControl b) => a.isPressed || b.isPressed ? 1f : 0f;

        private void FaceMovement(float x)
        {
            if (_visual == null || Mathf.Abs(x) < 0.01f) return;
            Vector3 scale = _visual.localScale;
            scale.x = Mathf.Abs(scale.x) * Mathf.Sign(x);
            _visual.localScale = scale;
        }
    }
}
