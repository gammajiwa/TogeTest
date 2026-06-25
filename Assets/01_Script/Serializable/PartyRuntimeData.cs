using System.Collections.Generic;

namespace Toge.Save
{
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
