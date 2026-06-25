using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Toge.Battle
{
    public class UnitHealthBar : MonoBehaviour
    {
        private BattleUnit _unit;
        private Camera _camera;
        private RectTransform _bar;
        private Image _fill;
        private float _headHeight;

        public void Init(BattleUnit unit, float headHeight, TMP_FontAsset font)
        {
            _unit = unit;
            _headHeight = headHeight;
            _camera = Camera.main != null ? Camera.main : Object.FindFirstObjectByType<Camera>();

            var go = new GameObject("HealthBar_" + unit.DisplayName);
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

            Color teamColor = unit.Team == BattleTeam.Player
                ? new Color(0.32f, 0.82f, 0.36f)
                : new Color(0.86f, 0.31f, 0.31f);
            _fill = NewImage("Fill", bg.transform, teamColor);
            _fill.type = Image.Type.Filled;
            _fill.fillMethod = Image.FillMethod.Horizontal;
            _fill.fillOrigin = 0;
            RectTransform frt = _fill.rectTransform;
            frt.anchorMin = Vector2.zero;
            frt.anchorMax = Vector2.one;
            frt.offsetMin = new Vector2(3f, 3f);
            frt.offsetMax = new Vector2(-3f, -3f);

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

            Refresh();
        }

        private void LateUpdate()
        {
            if (_unit == null || _bar == null) return;

            _bar.position = transform.position + Vector3.up * _headHeight;
            if (_camera != null) _bar.rotation = _camera.transform.rotation;
            Refresh();
        }

        private void Refresh()
        {
            if (_fill != null && _unit.MaxHealth > 0)
                _fill.fillAmount = (float)_unit.CurrentHealth / _unit.MaxHealth;
        }

        private void OnDestroy()
        {
            if (_bar != null) Destroy(_bar.gameObject);
        }

        private static Image NewImage(string name, Transform parent, Color color)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var img = go.AddComponent<Image>();
            img.color = color;
            return img;
        }
    }
}
