using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Spine.Unity;
using Toge.Data;
using Toge.Enums;

namespace Toge.Battle
{
    public class BattleUI : MonoBehaviour
    {
        [SerializeField] private BattleManager _battle;
        [SerializeField] private RectTransform _cardHand;
        [SerializeField] private TMP_FontAsset _font;

        [Header("Status")]
        [SerializeField] private TextMeshProUGUI _energyText;
        [SerializeField] private TextMeshProUGUI _blockText;
        [SerializeField] private TextMeshProUGUI _drawText;
        [SerializeField] private TextMeshProUGUI _discardText;
        [SerializeField] private Button _endTurnButton;

        [Header("Result")]
        [SerializeField] private GameObject _resultObject;
        [SerializeField] private TextMeshProUGUI _resultText;

        private const float CardWidth = 150f;
        private const float CardSpacing = 16f;
        private const float DealStagger = 0.06f;

        private Camera _camera;
        private RectTransform _arrow;
        private BattleUnit _highlighted;
        private readonly Dictionary<CardInstance, BattleCardView> _liveCards = new();
        private readonly List<CardInstance> _removal = new();
        private RectTransform _deckPile;
        private RectTransform _discardPile;

        private void OnEnable()
        {
            ClearHand();
            if (_battle == null) return;
            _battle.StateChanged += Refresh;
            _battle.PlayerTurnChanged += OnPlayerTurnChanged;
            _battle.BattleEnded += OnBattleEnded;
            if (_endTurnButton != null) _endTurnButton.onClick.AddListener(OnEndTurn);
        }

        private void OnDisable()
        {
            if (_battle == null) return;
            _battle.StateChanged -= Refresh;
            _battle.PlayerTurnChanged -= OnPlayerTurnChanged;
            _battle.BattleEnded -= OnBattleEnded;
            if (_endTurnButton != null) _endTurnButton.onClick.RemoveListener(OnEndTurn);
        }

        private void Start()
        {
            _camera = GetComponent<Canvas>() != null ? GetComponent<Canvas>().worldCamera : null;
            if (_camera == null) _camera = Camera.main;

            if (_cardHand != null)
            {
                var layout = _cardHand.GetComponent<HorizontalLayoutGroup>();
                if (layout != null) layout.enabled = false;
            }
            if (_drawText != null) _deckPile = _drawText.transform.parent as RectTransform;
            if (_discardText != null) _discardPile = _discardText.transform.parent as RectTransform;

            CreateArrow();
            if (_resultObject != null) _resultObject.SetActive(false);
            Refresh();
        }

        private void CreateArrow()
        {
            var go = new GameObject("TargetArrow", typeof(RectTransform));
            go.transform.SetParent(transform, false);
            var img = go.AddComponent<Image>();
            img.color = new Color(1f, 0.82f, 0.3f, 0.85f);
            img.raycastTarget = false;
            _arrow = (RectTransform)go.transform;
            _arrow.anchorMin = _arrow.anchorMax = new Vector2(0.5f, 0.5f);
            _arrow.pivot = new Vector2(0f, 0.5f);
            _arrow.gameObject.SetActive(false);
        }

        public void BeginDrag(BattleCardView view)
        {
            if (view.Card.Data.type != CardType.Defend && _arrow != null)
                _arrow.gameObject.SetActive(true);
        }

        public void UpdateDrag(BattleCardView view, Vector2 screenPos)
        {
            if (view.Card.Data.type == CardType.Defend) return;

            Vector2 cardScreen = RectTransformUtility.WorldToScreenPoint(_camera, ((RectTransform)view.transform).position);
            UpdateArrow(cardScreen, screenPos);
            Highlight(NearestEnemy(screenPos));
        }

        public void EndDrag(BattleCardView view, Vector2 screenPos)
        {
            if (_arrow != null) _arrow.gameObject.SetActive(false);
            BattleUnit target = _highlighted;
            ClearHighlight();

            bool played = false;
            if (view.Card.Data.type == CardType.Defend)
            {
                if (screenPos.y > Screen.height * 0.32f) played = _battle.PlayCard(view.Card, null);
            }
            else if (target != null)
            {
                played = _battle.PlayCard(view.Card, target);
            }

            if (!played) view.ReturnToSlot();
        }

        private void UpdateArrow(Vector2 fromScreen, Vector2 toScreen)
        {
            if (_arrow == null) return;
            var canvasRect = (RectTransform)_arrow.parent;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, fromScreen, _camera, out Vector2 from);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, toScreen, _camera, out Vector2 to);

            Vector2 dir = to - from;
            _arrow.anchoredPosition = from;
            _arrow.sizeDelta = new Vector2(dir.magnitude, 14f);
            _arrow.localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
        }

        private BattleUnit NearestEnemy(Vector2 screenPos)
        {
            BattleUnit best = null;
            float bestDistance = 200f;
            foreach (BattleUnit enemy in _battle.Enemies)
            {
                if (!enemy.IsAlive) continue;
                Vector3 sp = _camera.WorldToScreenPoint(enemy.transform.position + Vector3.up * 1.6f);
                if (sp.z < 0f) continue;
                float distance = Vector2.Distance(screenPos, sp);
                if (distance < bestDistance) { bestDistance = distance; best = enemy; }
            }
            return best;
        }

        private void Highlight(BattleUnit enemy)
        {
            if (_highlighted == enemy) return;
            ClearHighlight();
            _highlighted = enemy;
            Tint(enemy, 1f, 1f, 0.45f);
        }

        private void ClearHighlight()
        {
            if (_highlighted != null) Tint(_highlighted, 1f, 1f, 1f);
            _highlighted = null;
        }

        private static void Tint(BattleUnit unit, float r, float g, float b)
        {
            if (unit == null) return;
            SkeletonAnimation sa = unit.GetComponent<SkeletonAnimation>();
            if (sa != null && sa.Skeleton != null)
            {
                sa.Skeleton.R = r;
                sa.Skeleton.G = g;
                sa.Skeleton.B = b;
            }
        }

        private void OnEndTurn() => _battle?.EndTurn();

        private void OnPlayerTurnChanged(bool playerTurn)
        {
            if (_endTurnButton != null) _endTurnButton.interactable = playerTurn;
            if (playerTurn && _resultObject != null) _resultObject.SetActive(false);
            Refresh();
        }

        private void Refresh()
        {
            if (_battle == null) return;
            if (_energyText != null) _energyText.text = $"{_battle.Energy}/{_battle.MaxEnergy}";
            if (_blockText != null) _blockText.text = _battle.Block > 0 ? $"BLOCK {_battle.Block}" : string.Empty;
            if (_drawText != null) _drawText.text = _battle.DrawCount.ToString();
            if (_discardText != null) _discardText.text = _battle.DiscardCount.ToString();
            ReconcileHand();
        }

        private void ReconcileHand()
        {
            if (_cardHand == null) return;

            bool playerTurn = _battle.IsPlayerTurn;

            _removal.Clear();
            foreach (var pair in _liveCards)
                if (!playerTurn || !HandContains(pair.Key)) _removal.Add(pair.Key);

            foreach (CardInstance card in _removal)
            {
                BattleCardView leaving = _liveCards[card];
                _liveCards.Remove(card);
                if (leaving == null) continue;
                leaving.PlayDiscard(HandLocal(_discardPile), () => { if (leaving != null) Destroy(leaving.gameObject); });
            }

            if (!playerTurn) return;

            int count = _battle.Hand.Count;
            int dealt = 0;
            for (int i = 0; i < count; i++)
            {
                CardInstance card = _battle.Hand[i];
                Vector3 slot = SlotPosition(i, count);
                bool affordable = _battle.Energy >= card.Data.cost;

                BattleCardView view;
                if (_liveCards.TryGetValue(card, out view))
                {
                    view.SetSlot(slot, true);
                    view.SetAffordable(affordable);
                }
                else
                {
                    view = CreateCard(card, slot);
                    _liveCards[card] = view;
                    view.SetAffordable(affordable);
                    view.PlayDeal(HandLocal(_deckPile), slot, dealt * DealStagger);
                    dealt++;
                }
            }
        }

        private bool HandContains(CardInstance card)
        {
            IReadOnlyList<CardInstance> hand = _battle.Hand;
            for (int i = 0; i < hand.Count; i++)
                if (hand[i] == card) return true;
            return false;
        }

        private void ClearHand()
        {
            _liveCards.Clear();
            if (_cardHand == null) return;
            for (int i = _cardHand.childCount - 1; i >= 0; i--)
                Destroy(_cardHand.GetChild(i).gameObject);
        }

        private BattleCardView CreateCard(CardInstance card, Vector3 slot)
        {
            CardSO data = card.Data;

            var go = new GameObject("Card_" + data.cardName, typeof(RectTransform));
            go.transform.SetParent(_cardHand, false);
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = new Vector2(CardWidth, 200f);
            rt.localPosition = slot;

            go.AddComponent<CanvasGroup>();
            go.AddComponent<Image>();

            var view = go.AddComponent<BattleCardView>();
            view.Setup(card, this);

            AddCost(go.transform, data.cost);
            AddLabel(go.transform, data);
            return view;
        }

        private Vector3 SlotPosition(int index, int count)
        {
            float pitch = CardWidth + CardSpacing;
            float totalWidth = count * CardWidth + (count - 1) * CardSpacing;
            float firstX = -(totalWidth - CardWidth) * 0.5f;
            return new Vector3(firstX + index * pitch, SlotCenterY(), 0f);
        }

        private Vector3 HandLocal(RectTransform pile)
        {
            if (pile == null || _cardHand == null) return new Vector3(0f, SlotCenterY(), 0f);
            Vector3 local = _cardHand.InverseTransformPoint(pile.position);
            local.z = 0f;
            return local;
        }

        private float SlotCenterY()
        {
            if (_cardHand == null) return 0f;
            float height = _cardHand.rect.height;
            if (height <= 1f) height = _cardHand.sizeDelta.y;
            return height * (0.5f - _cardHand.pivot.y);
        }

        private void AddCost(Transform parent, int cost)
        {
            var go = new GameObject("Cost", typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var text = go.AddComponent<TextMeshProUGUI>();
            text.font = _font;
            text.text = cost.ToString();
            text.fontSize = 30f;
            text.color = new Color(1f, 0.88f, 0.4f);
            text.alignment = TextAlignmentOptions.Center;
            text.raycastTarget = false;
            RectTransform rt = text.rectTransform;
            rt.anchorMin = new Vector2(0f, 1f);
            rt.anchorMax = new Vector2(0f, 1f);
            rt.pivot = new Vector2(0f, 1f);
            rt.sizeDelta = new Vector2(40f, 40f);
            rt.anchoredPosition = new Vector2(6f, -6f);
        }

        private void AddLabel(Transform parent, CardSO data)
        {
            var go = new GameObject("Label", typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var text = go.AddComponent<TextMeshProUGUI>();
            text.font = _font;
            string effect = data.type == CardType.Defend ? $"Block {data.power}" : $"Deal {data.power}";
            text.text = $"{data.cardName}\n\n{effect}";
            text.fontSize = 20f;
            text.alignment = TextAlignmentOptions.Center;
            text.color = Color.white;
            text.raycastTarget = false;
            RectTransform rt = text.rectTransform;
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = new Vector2(8f, 8f);
            rt.offsetMax = new Vector2(-8f, -26f);
        }

        private void OnBattleEnded(BattleResult result)
        {
            ClearHighlight();
            if (_arrow != null) _arrow.gameObject.SetActive(false);
            ClearHand();
            if (_resultObject != null) _resultObject.SetActive(true);
            if (_resultText != null) _resultText.text = result == BattleResult.Win ? "VICTORY!" : "DEFEAT";
        }
    }
}
