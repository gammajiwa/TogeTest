namespace Toge.Save
{
    /// <summary>Mutable runtime/save state for one party member.</summary>
    [System.Serializable]
    public class PartyMemberData
    {
        public string dataId;
        public int level = 1;
        public int maxHealth;
        public int currentHealth;
        public int currentSp;

        public bool IsAlive => currentHealth > 0;

        public PartyMemberData() { }

        public PartyMemberData(string dataId, int level, int maxHealth, int currentSp)
        {
            this.dataId = dataId;
            this.level = level;
            this.maxHealth = maxHealth;
            this.currentHealth = maxHealth;
            this.currentSp = currentSp;
        }
    }
}
