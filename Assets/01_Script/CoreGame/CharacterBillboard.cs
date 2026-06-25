using UnityEngine;

namespace Toge.Core
{
    public class CharacterBillboard : MonoBehaviour
    {
        [SerializeField] private Camera _camera;

        private void Awake()
        {
            if (_camera == null) _camera = Camera.main;
        }

        private void LateUpdate()
        {
            if (_camera == null) return;

            Vector3 euler = transform.localEulerAngles;
            transform.localEulerAngles = new Vector3(_camera.transform.eulerAngles.x, euler.y, euler.z);
        }
    }
}
