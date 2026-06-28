using UnityEngine;
using UnityEngine.UI;
using TMPro;
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

        private void OnEnable()
        {
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
            if (_resultObject != null) _resultObject.SetActive(false);
            Refresh();
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

            RebuildHand();
        }

        private void RebuildHand()
        {
            if (_cardHand == null) return;

            for (int i = _cardHand.childCount - 1; i >= 0; i--)
                Destroy(_cardHand.GetChild(i).gameObject);

            if (!_battle.IsPlayerTurn) return;

            foreach (CardInstance card in _battle.Hand)
                CreateCardButton(card);
        }

        private void CreateCardButton(CardInstance card)
        {
            CardSO data = card.Data;
            bool affordable = _battle.Energy >= data.cost;

            var go = new GameObject("Card_" + data.cardName, typeof(RectTransform));
            go.transform.SetParent(_cardHand, false);
            go.GetComponent<RectTransform>().sizeDelta = new Vector2(150f, 200f);

            var image = go.AddComponent<Image>();
            Color tint = CardSO.RarityColor(data.rarity);
            image.color = affordable ? tint : new Color(0.22f, 0.22f, 0.24f, 0.95f);

            var button = go.AddComponent<Button>();
            button.interactable = affordable;
            button.onClick.AddListener(() => PlayCard(card));

            AddCost(go.transform, data.cost);
            AddLabel(go.transform, data);
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
            RectTransform rt = text.rectTransform;
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = new Vector2(8f, 8f);
            rt.offsetMax = new Vector2(-8f, -26f);
        }

        private void PlayCard(CardInstance card)
        {
            if (_battle == null) return;
            _battle.PlayCard(card, _battle.FirstLivingEnemy());
        }

        private void OnBattleEnded(BattleResult result)
        {
            RebuildHand();
            if (_resultObject != null) _resultObject.SetActive(true);
            if (_resultText != null) _resultText.text = result == BattleResult.Win ? "VICTORY!" : "DEFEAT";
        }
    }
}
