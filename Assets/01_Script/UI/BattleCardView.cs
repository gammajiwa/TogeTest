using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Toge.Data;

namespace Toge.Battle
{
    [RequireComponent(typeof(RectTransform))]
    public class BattleCardView : MonoBehaviour,
        IBeginDragHandler, IDragHandler, IEndDragHandler,
        IPointerEnterHandler, IPointerExitHandler
    {
        public CardInstance Card { get; private set; }

        private const float HoverLift = 40f;
        private const float HoverScale = 1.12f;
        private const float DragLift = 56f;
        private const float MoveDuration = 0.16f;
        private const float DealDuration = 0.32f;
        private const float DiscardDuration = 0.26f;

        private BattleUI _ui;
        private Image _image;
        private CanvasGroup _group;
        private Vector3 _slot;
        private bool _draggable;
        private bool _dragging;
        private bool _busy;
        private Coroutine _move;

        private void Awake()
        {
            _image = GetComponent<Image>();
            _group = GetComponent<CanvasGroup>();
            if (_group == null) _group = gameObject.AddComponent<CanvasGroup>();
        }

        public void Setup(CardInstance card, BattleUI ui)
        {
            Card = card;
            _ui = ui;
        }

        public void SetAffordable(bool affordable)
        {
            _draggable = affordable;
            if (_image != null)
                _image.color = affordable
                    ? CardSO.RarityColor(Card.Data.rarity)
                    : new Color(0.22f, 0.22f, 0.24f, 0.95f);
        }

        public void SetSlot(Vector3 slot, bool animate)
        {
            _slot = slot;
            if (_busy || _dragging) return;
            if (animate) StartMove(slot, 1f, MoveDuration, null);
            else { transform.localPosition = slot; transform.localScale = Vector3.one; }
        }

        public void ReturnToSlot()
        {
            _dragging = false;
            StartMove(_slot, 1f, MoveDuration, null);
        }

        public void PlayDeal(Vector3 startLocal, Vector3 slot, float delay)
        {
            _slot = slot;
            _busy = true;
            transform.localPosition = startLocal;
            transform.localScale = Vector3.one * 0.5f;
            _group.alpha = 0f;
            StartCoroutine(DealRoutine(delay));
        }

        public void PlayDiscard(Vector3 targetLocal, Action onDone)
        {
            _busy = true;
            _draggable = false;
            _group.blocksRaycasts = false;
            if (_move != null) { StopCoroutine(_move); _move = null; }
            StartCoroutine(DiscardRoutine(targetLocal, onDone));
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_busy || _dragging) return;
            transform.SetAsLastSibling();
            StartMove(_slot + Vector3.up * HoverLift, HoverScale, MoveDuration, null);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_busy || _dragging) return;
            StartMove(_slot, 1f, MoveDuration, null);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!_draggable || _busy) return;
            _dragging = true;
            if (_move != null) { StopCoroutine(_move); _move = null; }
            transform.localPosition = _slot + Vector3.up * DragLift;
            transform.localScale = Vector3.one * HoverScale;
            _ui.BeginDrag(this);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!_dragging) return;
            _ui.UpdateDrag(this, eventData.position);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!_dragging) return;
            _dragging = false;
            _ui.EndDrag(this, eventData.position);
        }

        private void StartMove(Vector3 target, float scale, float duration, Action onDone)
        {
            if (_move != null) StopCoroutine(_move);
            _move = StartCoroutine(MoveRoutine(target, scale, duration, onDone));
        }

        private IEnumerator MoveRoutine(Vector3 target, float scale, float duration, Action onDone)
        {
            Vector3 startPos = transform.localPosition;
            Vector3 startScale = transform.localScale;
            Vector3 endScale = Vector3.one * scale;
            float t = 0f;
            while (t < duration)
            {
                t += Time.deltaTime;
                float k = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(t / duration));
                transform.localPosition = Vector3.Lerp(startPos, target, k);
                transform.localScale = Vector3.Lerp(startScale, endScale, k);
                yield return null;
            }
            transform.localPosition = target;
            transform.localScale = endScale;
            _move = null;
            if (onDone != null) onDone();
        }

        private IEnumerator DealRoutine(float delay)
        {
            if (delay > 0f) yield return new WaitForSeconds(delay);

            Vector3 startPos = transform.localPosition;
            Vector3 startScale = transform.localScale;
            float t = 0f;
            while (t < DealDuration)
            {
                t += Time.deltaTime;
                float k = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(t / DealDuration));
                transform.localPosition = Vector3.Lerp(startPos, _slot, k);
                transform.localScale = Vector3.Lerp(startScale, Vector3.one, k);
                _group.alpha = Mathf.Lerp(0.2f, 1f, k);
                yield return null;
            }
            transform.localPosition = _slot;
            transform.localScale = Vector3.one;
            _group.alpha = 1f;
            _busy = false;
        }

        private IEnumerator DiscardRoutine(Vector3 target, Action onDone)
        {
            Vector3 startPos = transform.localPosition;
            Vector3 startScale = transform.localScale;
            Vector3 endScale = Vector3.one * 0.4f;
            float t = 0f;
            while (t < DiscardDuration)
            {
                t += Time.deltaTime;
                float k = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(t / DiscardDuration));
                transform.localPosition = Vector3.Lerp(startPos, target, k);
                transform.localScale = Vector3.Lerp(startScale, endScale, k);
                _group.alpha = 1f - k;
                yield return null;
            }
            _busy = false;
            if (onDone != null) onDone();
        }
    }
}
