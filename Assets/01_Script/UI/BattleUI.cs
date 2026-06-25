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
        [SerializeField] private BattleUnit _player;
        [SerializeField] private BattleUnit _enemy;

        [Header("HP Bars")]
        [SerializeField] private Image _playerHpFill;
        [SerializeField] private Image _enemyHpFill;
        [SerializeField] private TextMeshProUGUI _playerHpText;
        [SerializeField] private TextMeshProUGUI _enemyHpText;

        [Header("Cards")]
        [SerializeField] private RectTransform _cardHand;
        [SerializeField] private TMP_FontAsset _font;

        [Header("Result")]
        [SerializeField] private GameObject _resultObject;
        [SerializeField] private TextMeshProUGUI _resultText;

        private void OnEnable()
        {
            if (_battle == null) return;
            _battle.StateChanged += Refresh;
            _battle.TurnStarted += OnTurnStarted;
            _battle.BattleEnded += OnBattleEnded;
        }

        private void OnDisable()
        {
            if (_battle == null) return;
            _battle.StateChanged -= Refresh;
            _battle.TurnStarted -= OnTurnStarted;
            _battle.BattleEnded -= OnBattleEnded;
        }

        private void Start()
        {
            ShowHand(false);
            if (_resultObject != null) _resultObject.SetActive(false);
        }

        public void Bind(BattleUnit player, BattleUnit enemy)
        {
            _player = player;
            _enemy = enemy;
            BuildHand();
            Refresh();
            ShowHand(false);
        }

        private void BuildHand()
        {
            if (_cardHand == null) return;
            PlayerDataSO data = _player != null ? _player.Data as PlayerDataSO : null;
            if (data == null) return;

            for (int i = _cardHand.childCount - 1; i >= 0; i--)
                Destroy(_cardHand.GetChild(i).gameObject);

            foreach (CardSO card in data.cards)
            {
                if (card == null) continue;
                CreateCardButton(card);
            }
        }

        private void CreateCardButton(CardSO card)
        {
            var go = new GameObject("Card_" + card.cardName, typeof(RectTransform));
            go.transform.SetParent(_cardHand, false);
            go.GetComponent<RectTransform>().sizeDelta = new Vector2(160, 210);

            var image = go.AddComponent<Image>();
            image.color = card.type == CardType.Defend ? new Color(0.28f, 0.45f, 0.62f) : new Color(0.5f, 0.3f, 0.52f);

            var button = go.AddComponent<Button>();
            button.onClick.AddListener(() => PlayCard(card));

            var labelGO = new GameObject("Label", typeof(RectTransform));
            labelGO.transform.SetParent(go.transform, false);
            var label = labelGO.AddComponent<TextMeshProUGUI>();
            label.font = _font;
            label.text = card.cardName + "\n\n" + (card.type == CardType.Defend ? "GUARD" : "PWR " + card.power);
            label.fontSize = 22;
            label.alignment = TextAlignmentOptions.Top;
            label.color = Color.white;
            var lrt = label.GetComponent<RectTransform>();
            lrt.anchorMin = Vector2.zero;
            lrt.anchorMax = Vector2.one;
            lrt.offsetMin = new Vector2(10, 10);
            lrt.offsetMax = new Vector2(-10, -10);
        }

        private void PlayCard(CardSO card)
        {
            if (_battle == null || _battle.ActingUnit == null) return;

            BattleActionType type = card.type == CardType.Defend ? BattleActionType.Defend : BattleActionType.Card;
            BattleUnit target = _battle.FirstLivingEnemy();
            _battle.SubmitAction(new BattleAction(type, _battle.ActingUnit, target, card));
            ShowHand(false);
        }

        private void OnTurnStarted(BattleUnit unit) => ShowHand(unit.Team == BattleTeam.Player);

        private void ShowHand(bool show)
        {
            if (_cardHand != null) _cardHand.gameObject.SetActive(show);
        }

        private void Refresh()
        {
            SetBar(_playerHpFill, _playerHpText, _player);
            SetBar(_enemyHpFill, _enemyHpText, _enemy);
        }

        private static void SetBar(Image fill, TextMeshProUGUI text, BattleUnit unit)
        {
            if (unit == null) return;
            if (fill != null) fill.fillAmount = unit.MaxHealth > 0 ? (float)unit.CurrentHealth / unit.MaxHealth : 0f;
            if (text != null) text.text = $"{unit.CurrentHealth}/{unit.MaxHealth}";
        }

        private void OnBattleEnded(BattleResult result)
        {
            ShowHand(false);
            if (_resultObject != null) _resultObject.SetActive(true);
            if (_resultText != null) _resultText.text = result == BattleResult.Win ? "VICTORY!" : "DEFEAT";
        }
    }
}
