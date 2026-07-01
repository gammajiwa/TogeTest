using System.Collections.Generic;
using UnityEngine;
using Toge.Data;

namespace Toge.Variables
{
    [CreateAssetMenu(menuName = "Toge/Variables/Encounter Anchor", fileName = "EncounterAnchor")]
    public class EncounterAnchorSO : ScriptableObject
    {
        [System.NonSerialized] private EncounterSO _value;
        [System.NonSerialized] private readonly HashSet<EncounterSO> _cleared = new();

        public EncounterSO Value => _value;
        public bool IsSet => _value != null;

        public void Provide(EncounterSO value) => _value = value;
        public void Unset() => _value = null;

        public bool IsCleared(EncounterSO value) => value != null && _cleared.Contains(value);
        public void MarkCleared(EncounterSO value) { if (value != null) _cleared.Add(value); }
    }
}
