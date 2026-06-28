using UnityEngine;
using UnityEngine.EventSystems;

namespace Toge.Battle
{
    public class BattleCardView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public CardInstance Card { get; private set; }

        private BattleUI _ui;
        private Vector3 _home;

        public void Setup(CardInstance card, BattleUI ui)
        {
            Card = card;
            _ui = ui;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _home = transform.localPosition;
            _ui.BeginDrag(this);
        }

        public void OnDrag(PointerEventData eventData)
        {
            transform.localPosition = _home + new Vector3(0f, 40f, 0f);
            _ui.UpdateDrag(this, eventData.position);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            transform.localPosition = _home;
            _ui.EndDrag(this, eventData.position);
        }
    }
}
