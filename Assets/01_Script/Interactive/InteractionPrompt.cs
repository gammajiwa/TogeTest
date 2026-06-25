using UnityEngine;
using TMPro;

namespace Toge.Interactive
{
    public class InteractionPrompt : MonoBehaviour
    {
        [SerializeField] private TextMeshPro _label;
        [SerializeField] private Camera _camera;
        [SerializeField] private Vector3 _worldOffset = new Vector3(0f, 1.2f, 0f);

        private Vector3 _anchor;

        private void Awake()
        {
            if (_camera == null) _camera = Camera.main;
            Hide();
        }

        private void LateUpdate()
        {
            if (_label == null || !_label.gameObject.activeSelf) return;

            transform.position = _anchor + _worldOffset;
            if (_camera != null) transform.rotation = _camera.transform.rotation;
        }

        public void Show(Vector3 anchor, string text)
        {
            _anchor = anchor;
            if (_label == null) return;

            _label.text = text;
            _label.gameObject.SetActive(true);
        }

        public void Hide()
        {
            if (_label != null) _label.gameObject.SetActive(false);
        }
    }
}
