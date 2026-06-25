using Toge.Data;

namespace Toge.Battle
{
    public enum BattleActionType { Attack, Defend, Card }

    public class BattleAction
    {
        public BattleActionType Type { get; }
        public BattleUnit Actor { get; }
        public BattleUnit Target { get; }
        public CardSO Card { get; }

        public BattleAction(BattleActionType type, BattleUnit actor, BattleUnit target, CardSO card = null)
        {
            Type = type;
            Actor = actor;
            Target = target;
            Card = card;
        }
    }
}
