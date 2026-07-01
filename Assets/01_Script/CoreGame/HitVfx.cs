using System.Collections;
using UnityEngine;

namespace Toge.Core
{
    public class HitVfx : MonoBehaviour
    {
        private const int SparkCount = 9;
        private const float Lifetime = 0.34f;

        private static Sprite _sprite;

        public static void Spawn(Vector3 position)
        {
            var go = new GameObject("HitVfx");
            go.transform.position = position;
            go.AddComponent<HitVfx>();
        }

        private void Start()
        {
            Camera cam = Camera.main;
            if (cam != null) transform.rotation = cam.transform.rotation;
            StartCoroutine(Play());
        }

        private IEnumerator Play()
        {
            SpriteRenderer flash = NewPiece(new Color(1f, 1f, 0.85f, 0.9f), 100);
            var sparks = new SpriteRenderer[SparkCount];
            var dirs = new Vector3[SparkCount];
            for (int i = 0; i < SparkCount; i++)
            {
                sparks[i] = NewPiece(new Color(1f, 0.9f, 0.45f, 1f), 105);
                float ang = 360f / SparkCount * i + Random.Range(-14f, 14f);
                dirs[i] = Quaternion.Euler(0f, 0f, ang) * Vector3.right;
            }

            float t = 0f;
            while (t < Lifetime)
            {
                t += Time.deltaTime;
                float k = t / Lifetime;

                float flashK = Mathf.Clamp01(k / 0.45f);
                flash.transform.localScale = Vector3.one * Mathf.Lerp(0.3f, 1.3f, flashK);
                SetAlpha(flash, (1f - flashK) * 0.9f);

                float dist = Mathf.SmoothStep(0f, 0.85f, k);
                float scale = Mathf.Lerp(0.32f, 0.06f, k);
                float alpha = 1f - k;
                for (int i = 0; i < SparkCount; i++)
                {
                    sparks[i].transform.localPosition = dirs[i] * dist;
                    sparks[i].transform.localScale = Vector3.one * scale;
                    SetAlpha(sparks[i], alpha);
                }
                yield return null;
            }
            Destroy(gameObject);
        }

        private SpriteRenderer NewPiece(Color color, int order)
        {
            var go = new GameObject("Piece");
            go.transform.SetParent(transform, false);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = WhiteSprite();
            sr.color = color;
            sr.sortingOrder = order;
            return sr;
        }

        private static void SetAlpha(SpriteRenderer sr, float a)
        {
            Color c = sr.color;
            c.a = a;
            sr.color = c;
        }

        private static Sprite WhiteSprite()
        {
            if (_sprite == null)
            {
                var tex = new Texture2D(4, 4, TextureFormat.RGBA32, false);
                var px = new Color32[16];
                for (int i = 0; i < px.Length; i++) px[i] = new Color32(255, 255, 255, 255);
                tex.SetPixels32(px);
                tex.Apply();
                _sprite = Sprite.Create(tex, new Rect(0f, 0f, 4f, 4f), new Vector2(0.5f, 0.5f), 4f);
            }
            return _sprite;
        }
    }
}
