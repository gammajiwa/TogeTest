using System.Collections.Generic;
using UnityEngine;
using Toge.Battle;

namespace Toge.AI
{
    [CreateAssetMenu(menuName = "Toge/AI/Aggressive Strategy", fileName = "AggressiveStrategy")]
    public class AggressiveStrategySO : EnemyStrategySO
    {
        public override BattleAction DecideAction(BattleUnit self, List<BattleUnit> targets)
        {
            BattleUnit weakest = null;
            int lowestHealth = int.MaxValue;

            foreach (var target in targets)
            {
                if (!target.IsAlive || target.CurrentHealth >= lowestHealth) continue;
                lowestHealth = target.CurrentHealth;
                weakest = target;
            }

            return weakest != null ? new BattleAction(BattleActionType.Attack, self, weakest) : null;
        }
    }
}
