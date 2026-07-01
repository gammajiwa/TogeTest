using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Toge.Battle
{
    public class UnitHealthBar : MonoBehaviour
    {
        private const float TrailDrainSpeed = 0.8f;

        private BattleUnit _unit;
        private Camera _camera;
        private RectTransform _bar;
        private Image _fill;
        private Image _trail;
        private float _headHeight;

        public void Init(BattleUnit unit, float headHeight, TMP_FontAsset font)
        {
            _unit = unit;
            _headHeight = headHeight;
            _camera = Camera.main != null ? Camera.main : Object.FindFirstObjectByType<Camera>();

            var go = new GameObject("HealthBar_" + unit.DisplayName);
            UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(go, gameObject.scene);
            var canvas = go.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            _bar = (RectTransform)go.transform;
            _bar.sizeDelta = new Vector2(180f, 46f);
            _bar.localScale = Vector3.one * 0.01f;
            _bar.position = transform.position + Vector3.up * _headHeight;
            if (_camera != null) _bar.rotation = _camera.transform.rotation;

            Image bg = NewImage("BG", _bar, new Color(0f, 0f, 0f, 0.6f));
            RectTransform bgrt = bg.rectTransform;
            bgrt.anchorMin = new Vector2(0f, 0f);
            bgrt.anchorMax = new Vector2(1f, 0f);
            bgrt.pivot = new Vector2(0.5f, 0f);
            bgrt.sizeDelta = new Vector2(0f, 18f);
            bgrt.anchoredPosition = Vector2.zero;

            _trail = NewFill("Trail", bg.transform, new Color(0.96f, 0.86f, 0.36f, 0.9f));

            Color teamColor = unit.Team == BattleTeam.Player
                ? new Color(0.32f, 0.82f, 0.36f)
                : new Color(0.86f, 0.31f, 0.31f);
            _fill = NewFill("Fill", bg.transform, teamColor);

            var nameGO = new GameObject("Name", typeof(RectTransform));
            nameGO.transform.SetParent(_bar, false);
            var label = nameGO.AddComponent<TextMeshProUGUI>();
            label.font = font;
            label.text = unit.DisplayName;
            label.fontSize = 18f;
            label.alignment = TextAlignmentOptions.Center;
            label.color = Color.white;
            RectTransform lrt = label.rectTransform;
            lrt.anchorMin = new Vector2(0f, 0f);
            lrt.anchorMax = new Vector2(1f, 0f);
            lrt.pivot = new Vector2(0.5f, 0f);
            lrt.sizeDelta = new Vector2(0f, 24f);
            lrt.anchoredPosition = new Vector2(0f, 20f);

            float start = Fraction();
            _fill.fillAmount = start;
            _trail.fillAmount = start;
        }

        public void Hide()
        {
            if (_bar != null) _bar.gameObject.SetActive(false);
            enabled = false;
        }

        private void LateUpdate()
        {
            if (_unit == null || _bar == null) return;

            _bar.position = transform.position + Vector3.up * _headHeight;
            if (_camera != null) _bar.rotation = _camera.transform.rotation;

            float target = Fraction();
            _fill.fillAmount = target;
            if (_trail.fillAmount < target) _trail.fillAmount = target;
            else _trail.fillAmount = Mathf.MoveTowards(_trail.fillAmount, target, TrailDrainSpeed * Time.deltaTime);
        }

        private float Fraction()
        {
            return _unit.MaxHealth > 0 ? (float)_unit.CurrentHealth / _unit.MaxHealth : 0f;
        }

        private void OnDestroy()
        {
            if (_bar != null) Destroy(_bar.gameObject);
        }

        private static Image NewFill(string name, Transform parent, Color color)
        {
            Image img = NewImage(name, parent, color);
            img.type = Image.Type.Filled;
            img.fillMethod = Image.FillMethod.Horizontal;
            img.fillOrigin = 0;
            RectTransform rt = img.rectTransform;
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = new Vector2(3f, 3f);
            rt.offsetMax = new Vector2(-3f, -3f);
            return img;
        }

        private static Sprite _whiteSprite;

        private static Sprite WhiteSprite()
        {
            if (_whiteSprite == null)
            {
                var tex = new Texture2D(4, 4, TextureFormat.RGBA32, false);
                var pixels = new Color32[tex.width * tex.height];
                for (int i = 0; i < pixels.Length; i++) pixels[i] = new Color32(255, 255, 255, 255);
                tex.SetPixels32(pixels);
                tex.Apply();
                _whiteSprite = Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100f);
            }
            return _whiteSprite;
        }

        private static Image NewImage(string name, Transform parent, Color color)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var img = go.AddComponent<Image>();
            img.sprite = WhiteSprite();
            img.color = color;
            return img;
        }
    }
}
