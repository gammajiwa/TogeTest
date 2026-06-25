using System.Collections.Generic;
using UnityEngine;

namespace Toge.Data
{
    [CreateAssetMenu(menuName = "Toge/Data/Party Config", fileName = "PartyConfig")]
    public class PartyConfigSO : ScriptableObject
    {
        public List<PlayerDataSO> members = new();
    }
}
