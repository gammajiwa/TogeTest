namespace Toge.Interfaces
{
    public interface IDamageable
    {
        int CurrentHealth { get; }
        bool IsAlive { get; }
        void TakeDamage(int amount);
    }
}
