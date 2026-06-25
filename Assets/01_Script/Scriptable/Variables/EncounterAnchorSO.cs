using UnityEngine;
using Toge.Data;

namespace Toge.Variables
{
    [CreateAssetMenu(menuName = "Toge/Variables/Encounter Anchor", fileName = "EncounterAnchor")]
    public class EncounterAnchorSO : ScriptableObject
    {
        [System.NonSerialized] private EncounterSO _value;

        public EncounterSO Value => _value;
        public bool IsSet => _value != null;

        public void Provide(EncounterSO value) => _value = value;
        public void Unset() => _value = null;
    }
}
