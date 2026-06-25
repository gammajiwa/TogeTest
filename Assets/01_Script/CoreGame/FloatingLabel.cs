using UnityEngine;
using TMPro;

namespace Toge.Core
{
    public class FloatingLabel : MonoBehaviour
    {
        private const float Lifetime = 1.6f;
        private const float RiseSpeed = 1.2f;

        private TextMeshPro _text;
        private Camera _camera;
        private float _age;

        public static void Spawn(Vector3 position, string message, Color color)
        {
            var go = new GameObject("FloatingLabel");
            go.transform.position = position;
            go.AddComponent<FloatingLabel>().Setup(message, color);
        }

        private void Setup(string message, Color color)
        {
            _camera = Camera.main != null ? Camera.main : Object.FindFirstObjectByType<Camera>();
            _text = gameObject.AddComponent<TextMeshPro>();

            TMP_FontAsset font = TMP_Settings.defaultFontAsset;
            if (font != null) _text.font = font;
            _text.text = message;
            _text.color = color;
            _text.fontSize = 4.5f;
            _text.alignment = TextAlignmentOptions.Center;
            _text.GetComponent<RectTransform>().sizeDelta = new Vector2(6f, 1.5f);
        }

        private void Update()
        {
            _age += Time.deltaTime;
            transform.position += Vector3.up * (RiseSpeed * Time.deltaTime);
            if (_camera != null) transform.rotation = _camera.transform.rotation;

            if (_text != null)
            {
                Color color = _text.color;
                color.a = Mathf.Clamp01(1f - _age / Lifetime);
                _text.color = color;
            }

            if (_age >= Lifetime) Destroy(gameObject);
        }
    }
}
