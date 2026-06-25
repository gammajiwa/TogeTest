using UnityEngine;
using Toge.Interfaces;
using Toge.Interactive;

namespace Toge.Entities
{
    public class PlayerInteractor : MonoBehaviour
    {
        [SerializeField] private float _range = 2f;
        [SerializeField] private InteractionPrompt _prompt;

        private readonly Collider[] _hits = new Collider[8];
        private IInteractable _nearest;
        private Collider _nearestCollider;

        private void Update() => RefreshNearest();

        public bool TryInteract()
        {
            if (_nearest == null) return false;

            _nearest.Interact();
            return true;
        }

        private void RefreshNearest()
        {
            int count = Physics.OverlapSphereNonAlloc(transform.position, _range, _hits);
            IInteractable best = null;
            Collider bestCollider = null;
            float bestSqr = float.MaxValue;

            for (int i = 0; i < count; i++)
            {
                if (!_hits[i].TryGetComponent(out IInteractable interactable)) continue;

                float sqr = (_hits[i].transform.position - transform.position).sqrMagnitude;
                if (sqr < bestSqr)
                {
                    bestSqr = sqr;
                    best = interactable;
                    bestCollider = _hits[i];
                }
            }

            _nearest = best;
            UpdatePrompt(bestCollider);
        }

        private void UpdatePrompt(Collider target)
        {
            if (target == _nearestCollider) return;
            _nearestCollider = target;

            if (_prompt == null) return;

            if (target != null && _nearest != null)
            {
                Bounds bounds = target.bounds;
                Vector3 head = new Vector3(bounds.center.x, bounds.max.y, bounds.center.z);
                _prompt.Show(head, _nearest.InteractionPrompt);
            }
            else
            {
                _prompt.Hide();
            }
        }
    }
}
