using UnityEngine;
using Toge.Data;
using Toge.Events;
using Toge.Variables;

namespace Toge.Core
{
    public class EncounterTrigger : MonoBehaviour
    {
        [SerializeField] private EncounterSO _encounter;
        [SerializeField] private EncounterEventChannelSO _channel;
        [SerializeField] private EncounterAnchorSO _activeEncounter;
        [SerializeField] private TransformAnchorSO _playerAnchor;
        [SerializeField] private GameObject _visual;
        [SerializeField] private float _triggerRadius = 1.5f;

        private bool _fired;

        private void Update()
        {
            if (_fired || _playerAnchor == null || !_playerAnchor.IsSet) return;

            Vector3 delta = transform.position - _playerAnchor.Value.position;
            if (delta.sqrMagnitude > _triggerRadius * _triggerRadius) return;

            _fired = true;
            if (_activeEncounter != null) _activeEncounter.Provide(_encounter);
            if (_visual != null) _visual.SetActive(false);
            if (_channel != null) _channel.RaiseEvent(_encounter);
        }
    }
}
