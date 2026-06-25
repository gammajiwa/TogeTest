using Toge.Data;

namespace Toge.Battle
{
    public class CardInstance
    {
        public CardSO Data { get; }

        public CardInstance(CardSO data) => Data = data;
    }
}
