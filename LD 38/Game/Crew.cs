using Gash;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    class Crew : IEnumerable<CrewMember>
    {
        public List<CrewMember> Members = new List<CrewMember>();

        public int HireCount = 0;

        public Crew()
        {
            // Start with 4 random crew members
            Members.Add(new CrewMember());
            Members.Add(new CrewMember());
            Members.Add(new CrewMember());
            Members.Add(new CrewMember());
        }

        public void PrintFreeAndRecoveringMembers()
        {
            var freeMembers = Members.FindAll(m => m.IsBusy == false);
            if (freeMembers.Count > 0)
            {
                GConsole.WriteLine(-1.0f, "{0} member(s) of your crew are sitting on their asses{1}.",
                freeMembers.Count,
                freeMembers.Count <= 0 ? "" :
                ": " + String.Join(", ", freeMembers.Select(m => new string(m.ColoredName.ToCharArray())).ToList()));
            }

            var recoveringMembers = Members.FindAll(m => m.IndisposedTurnCounter > 0);
            if(recoveringMembers.Count > 0)
            {
                GConsole.WriteLine(-1.0f, "{0} member(s) of your crew are recovering from failed missions{1}",
                    recoveringMembers.Count,
                    recoveringMembers.Count <= 0 ? "" :
                    ": " + String.Join(", ", recoveringMembers.Select(m => new string(m.ColoredName.ToCharArray())).ToList()));
            }
        }

        public float HirePrice()
        {
           return (float)(-99.50231 + 277.084 * Math.Exp(+0.6759646 * HireCount));
        }

        internal void Hire()
        {
            if(HirePrice() > GameManager.Instance.Assets.Money)
            {
                GConsole.WriteLine("Insufficient funds to hire a new person. {0} {1} needed", 
                    HirePrice().ToString("N0"), Assets.KPounds.ColoredName);
            }
            else
            {
                CrewMember member = new CrewMember();
                GConsole.WriteLine("{0} joined your crew.", member.ColoredName);
                HireCount++;
                Members.Add(member);
                GConsole.RegisterKeyword(member);
            }
        }

        public bool AnyoneAvailableForMission => Members.Count(m => m.IsBusy == false) > 0;

        public IEnumerator<CrewMember> GetEnumerator()
        {
            return Members.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Members.GetEnumerator();
        }

        public List<string> GetNames()
        {
            return (from m in Members
                    select m.Name).ToList();                   
        }

        public void PrintMembersOnMission()
        {
            var onMission = Members.FindAll(m => m.IsOnMission == true);
            foreach(var m in onMission)
            {
                GConsole.WriteLine(-1.0f, "{0} is on {1} mission and will be for {2} more day(s).", 
                    m.ColoredName, m.MissionType.ColoredName, m.MissionTurnCounter);
            }
        }
    }
}
