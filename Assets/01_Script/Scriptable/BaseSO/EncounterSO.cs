using System.Collections.Generic;
using UnityEngine;

namespace Toge.Data
{
    [CreateAssetMenu(menuName = "Toge/Data/Encounter", fileName = "NewEncounter")]
    public class EncounterSO : ScriptableObject
    {
        public string displayName;
        public List<EnemyDataSO> enemies = new();
    }
}
