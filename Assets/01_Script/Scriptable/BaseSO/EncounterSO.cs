using System.Collections.Generic;
using UnityEngine;

namespace Toge.Data
{
    /// <summary>The set of enemies that make up a single battle.</summary>
    [CreateAssetMenu(menuName = "Toge/Data/Encounter", fileName = "NewEncounter")]
    public class EncounterSO : ScriptableObject
    {
        public string displayName;
        public List<EnemyDataSO> enemies = new();
    }
}
