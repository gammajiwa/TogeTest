using UnityEngine;
using Toge.Data;
using Toge.Variables;

namespace Toge.Core
{
    public class DestructibleObject : MonoBehaviour
    {
        [SerializeField] private int _hitPoints = 1;
        [SerializeField] private LootTableSO _loot;
        [SerializeField] private RuntimeDeckSO _deck;
        [SerializeField] private GameObject _visual;
        [SerializeField] private Vector3 _labelOffset = new Vector3(0f, 1.6f, 0f);

        private int _hp;
        private bool _broken;

        private void Awake() => _hp = Mathf.Max(1, _hitPoints);

        public void Hit(int amount)
        {
            if (_broken) return;

            _hp -= Mathf.Max(1, amount);
            if (_hp <= 0) Break();
        }

        private void Break()
        {
            _broken = true;

            CardSO drop = _loot != null ? _loot.Roll() : null;
            if (drop != null)
            {
                if (_deck != null) _deck.Add(drop);
                FloatingLabel.Spawn(transform.position + _labelOffset, "+ " + drop.cardName, CardSO.RarityColor(drop.rarity));
            }

            if (_visual != null) _visual.SetActive(false);
            else gameObject.SetActive(false);
        }
    }
}
