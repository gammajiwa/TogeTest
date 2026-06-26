using System;
using UnityEngine;

namespace Toge.Battle
{
    public class BattleProjectile : MonoBehaviour
    {
        private Vector3 _start;
        private Transform _target;
        private float _duration;
        private float _elapsed;
        private Action _onHit;
        private bool _done;

        public static void Spawn(Vector3 from, Transform target, float duration, Action onHit)
        {
            var go = new GameObject("BattleProjectile");
            go.transform.position = from;

            BattleProjectile projectile = go.AddComponent<BattleProjectile>();
            projectile._start = from;
            projectile._target = target;
            projectile._duration = Mathf.Max(0.05f, duration);
            projectile._onHit = onHit;
        }

        private void Update()
        {
            if (_done) return;

            _elapsed += Time.deltaTime;
            float k = Mathf.Clamp01(_elapsed / _duration);
            Vector3 destination = _target != null ? _target.position + Vector3.up * 1.2f : _start;
            transform.position = Vector3.Lerp(_start, destination, k);

            if (k < 1f) return;

            _done = true;
            _onHit?.Invoke();
            Destroy(gameObject);
        }
    }
}
