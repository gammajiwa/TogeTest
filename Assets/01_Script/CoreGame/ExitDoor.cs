using UnityEngine;
using UnityEngine.SceneManagement;
using Toge.Variables;

namespace Toge.Core
{
    public class ExitDoor : MonoBehaviour
    {
        [SerializeField] private TransformAnchorSO _playerAnchor;
        [SerializeField] private float _triggerRadius = 2f;
        [SerializeField] private string _endScene = "TheEnd";

        private bool _used;

        private void Update()
        {
            if (_used || _playerAnchor == null || !_playerAnchor.IsSet) return;

            Vector3 delta = transform.position - _playerAnchor.Value.position;
            if (delta.sqrMagnitude > _triggerRadius * _triggerRadius) return;

            _used = true;
            if (SceneLoaderManager.Instance != null) SceneLoaderManager.Instance.Load(_endScene);
            else SceneManager.LoadScene(_endScene);
        }
    }
}
