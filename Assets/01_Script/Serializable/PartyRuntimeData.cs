using System.Collections.Generic;

namespace Toge.Save
{
    /// <summary>Party state carried between the overworld and battle, and persisted on save.</summary>
    [System.Serializable]
    public class PartyRuntimeData
    {
        public int saveVersion = 1;
        public List<PartyMemberData> members = new();
        public int gold;

        public bool IsWiped() => members.TrueForAll(m => !m.IsAlive);

        public void AddMember(PartyMemberData member) => members.Add(member);
    }
}
