using System.Collections.Generic;
using UnityEngine;
using Toge.Battle;

namespace Toge.AI
{
    public abstract class EnemyStrategySO : ScriptableObject
    {
        public abstract BattleAction DecideAction(BattleUnit self, List<BattleUnit> targets);
    }
}
