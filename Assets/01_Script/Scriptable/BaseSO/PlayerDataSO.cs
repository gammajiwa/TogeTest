using UnityEngine;

namespace Toge.Data
{
    [CreateAssetMenu(menuName = "Toge/Data/Player Data", fileName = "NewPlayerData")]
    public class PlayerDataSO : EntityDataSO
    {
        [Header("Player")]
        public int startingLevel = 1;
    }
}
