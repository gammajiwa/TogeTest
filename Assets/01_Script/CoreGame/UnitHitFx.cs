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
        private string _idleAnim = "idle";
        private string _attackAnim;
        private string _deathAnim;
        private bool _dead;
        private Coroutine _routine;

        public void Init(int knockDir, string idle, string attack, string death)
        {
            _spine = GetComponent<SkeletonAnimation>();
            _home = transform.position;
            _knockDir = knockDir;
            _idleAnim = idle;
            _attackAnim = attack;
            _deathAnim = death;
        }

        public void Lunge()
        {
            if (_dead) return;
            PlayClip(_attackAnim, false, _idleAnim);
            Restart(LungeRoutine());
        }

        public void Play(int damage)
        {
            FloatingLabel.Spawn(transform.position + Vector3.up * 2.7f, "-" + damage, new Color(1f, 0.45f, 0.4f));
            if (_dead) return;
            Restart(HitRoutine());
        }

        public void PlayDeath()
        {
            if (_dead) return;
            _dead = true;
            if (_routine != null) StopCoroutine(_routine);
            ResetVisual();
            UnitHealthBar bar = GetComponent<UnitHealthBar>();
            if (bar != null) bar.Hide();
            _routine = StartCoroutine(DeathRoutine());
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

        private IEnumerator DeathRoutine()
        {
            PlayClip(_deathAnim, false, null);

            const float duration = 0.55f;
            float t = 0f;
            while (t < duration)
            {
                t += Time.deltaTime;
                float k = t / duration;
                SetAlpha(1f - k);
                transform.position = _home + new Vector3(_knockDir * 0.5f * k, -0.25f * k, 0f);
                yield return null;
            }
            SetAlpha(0f);
            gameObject.SetActive(false);
        }

        private void PlayClip(string anim, bool loop, string thenIdle)
        {
            if (_spine == null || string.IsNullOrEmpty(anim)) return;
            if (_spine.Skeleton == null || _spine.Skeleton.Data.FindAnimation(anim) == null) return;

            _spine.AnimationState.SetAnimation(0, anim, loop);
            if (!loop && !string.IsNullOrEmpty(thenIdle) && _spine.Skeleton.Data.FindAnimation(thenIdle) != null)
                _spine.AnimationState.AddAnimation(0, thenIdle, true, 0f);
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

        private void SetAlpha(float a)
        {
            if (_spine == null || _spine.Skeleton == null) return;
            _spine.Skeleton.A = a;
        }
    }
}
