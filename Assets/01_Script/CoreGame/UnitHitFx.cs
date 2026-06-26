using System.Collections;
using UnityEngine;
using Spine.Unity;
using Toge.Core;

namespace Toge.Battle
{
    public class UnitHitFx : MonoBehaviour
    {
        private SkeletonAnimation _spine;
        private Vector3 _home;
        private int _knockDir = 1;
        private Coroutine _routine;

        public void Init(int knockDir)
        {
            _spine = GetComponent<SkeletonAnimation>();
            _home = transform.position;
            _knockDir = knockDir;
        }

        public void Lunge() => Restart(LungeRoutine());

        public void Play(int damage)
        {
            FloatingLabel.Spawn(transform.position + Vector3.up * 2.7f, "-" + damage, new Color(1f, 0.45f, 0.4f));
            Restart(HitRoutine());
        }

        private void Restart(IEnumerator routine)
        {
            if (_routine != null) StopCoroutine(_routine);
            ResetVisual();
            _routine = StartCoroutine(routine);
        }

        private IEnumerator LungeRoutine()
        {
            const float duration = 0.18f;
            float t = 0f;
            while (t < duration)
            {
                t += Time.deltaTime;
                float k = Mathf.Sin(t / duration * Mathf.PI);
                transform.position = _home + new Vector3(-_knockDir * 0.45f * k, 0f, 0f);
                yield return null;
            }
            ResetVisual();
            _routine = null;
        }

        private IEnumerator HitRoutine()
        {
            const float duration = 0.22f;
            float t = 0f;
            while (t < duration)
            {
                t += Time.deltaTime;
                float k = 1f - t / duration;
                Tint(1f, 1f - 0.7f * k, 1f - 0.7f * k);
                transform.position = _home + new Vector3(_knockDir * 0.28f * k, 0f, 0f);
                yield return null;
            }
            ResetVisual();
            _routine = null;
        }

        private void ResetVisual()
        {
            transform.position = _home;
            Tint(1f, 1f, 1f);
        }

        private void Tint(float r, float g, float b)
        {
            if (_spine == null) return;
            Spine.Skeleton skeleton = _spine.Skeleton;
            if (skeleton == null) return;
            skeleton.R = r;
            skeleton.G = g;
            skeleton.B = b;
        }
    }
}
