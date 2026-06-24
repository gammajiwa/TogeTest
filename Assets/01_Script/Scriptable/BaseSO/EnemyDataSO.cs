using UnityEngine;

namespace Toge.Data
{
    [CreateAssetMenu(menuName = "Toge/Data/Enemy Data", fileName = "NewEnemyData")]
    public class EnemyDataSO : EntityDataSO
    {
        [Header("Rewards")]
        public int expReward = 10;
        public int goldReward = 5;
    }
}
