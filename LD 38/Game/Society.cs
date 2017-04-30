using Gash;
using GashLibrary.Commands;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    class Society : IEnumerable<Lord>
    {
        public List<Lord> Lords = new List<Lord>();

        public Lord Earl = null;

        public int[] DefameCount = { 0, 0, 0, 0 };

        public static Keyword KEarl = Keyword.CreateSimpleFormatted("Earl", "{1} is king's official representative in the Underbury county.");
        public static Keyword KViscount = Keyword.CreateSimpleFormatted("Viscount", "{1} is a sheriff of Underbury county and Earl's deputy.");
        public static Keyword KBaron = Keyword.CreateSimpleFormatted("Baron", "{1} is a holder of land granted to them by monarch.");

        public Society()
        {
            Earl = new Lord(Lord.LordType.Earl);
            Lords.Add(Earl);
            Lords.Add(new Lord(Lord.LordType.Viscount));
            for (int i = 0; i < 3; i++)
            {
                bool found = false;
                Lord lord = null;
                while(found == false)
                {
                    lord = new Lord(Lord.LordType.Baron);
                    if (Lords.Exists(x => x.Name == lord.Name) == false)
                    {
                        found = true;
                        Lords.Add(lord);
                    }
                }                
            }

            for (int i = 0; i < 5; i++)
            {
                bool found = false;
                Lord lord = null;
                while (found == false)
                {
                    lord = new Lord(Lord.LordType.ImportantCitizen);
                    if (Lords.Exists(x => x.Name == lord.Name) == false)
                    {
                        found = true;
                        Lords.Add(lord);
                    }
                }
            }

            Lords.Sort((x, y) => ((int)x.Type).CompareTo((int)y.Type));
        }

        public IEnumerator<Lord> GetEnumerator()
        {
            return Lords.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Lords.GetEnumerator();
        }

        public void ListSociety()
        {
            GConsole.WriteLine(-1.0f, "Underbury society consists of (in ascending order of importance):");
            foreach (var lord in Lords)
            {
                lord.ListSociety();
            }

            GConsole.WriteLine(-1.0f, " ");
        }

        public List<string> GetNames()
        {
            return (from l in Lords
                    select l.Name).ToList();
        }

        public Lord RandomNeigbour(Lord target)
        {
            if (target.Type == Lord.LordType.Earl) return null;
            if (target.Type == Lord.LordType.Viscount) return null;

            var candidates = Lords.FindAll(x => x != target && x.Type == target.Type);
            if(candidates.Count <= 0) return null;
            int index = GameManager.Instance.RNG.Next(0, candidates.Count);
            return candidates[index];
        }

        public Lord RandomBoss(Lord target)
        {
            if (target.Type == Lord.LordType.Earl) return null;
            var candidates = Lords.FindAll(x => x.Type == target.Type+1);
            if (candidates.Count <= 0) return null;
            int index = GameManager.Instance.RNG.Next(0, candidates.Count);
            return candidates[index];
        }

        public void Defame(Lord lord)
        {
            lord.Defame();
            DefameCount[(int)lord.Type]++;

            if(lord.Type == Lord.LordType.ImportantCitizen)
            {
                if(DefameCount[(int)lord.Type] >= 3)
                {
                    DefameCount[(int)lord.Type] -= 3;
                    Lord boss = RandomBoss(lord);
                    GConsole.WriteLine("As a result of repeated hits to subordinates reputations, {0} was defamed as well.", 
                        boss.ColoredName);
                    RandomBoss(lord).Defame();
                }
            }
            else if(lord.Type == Lord.LordType.Baron)
            {
                if (DefameCount[(int)lord.Type] >= 2)
                {
                    DefameCount[(int)lord.Type] -= 2;
                    Lord boss = RandomBoss(lord);
                    GConsole.WriteLine("As a result of repeated hits to subordinates reputations, {0} was defamed as well.",
                        boss.ColoredName);
                    RandomBoss(lord).Defame();
                }
            }
            else if(lord.Type == Lord.LordType.Viscount)
            {
                if (DefameCount[(int)lord.Type] >= 2)
                {
                    DefameCount[(int)lord.Type] -= 2;
                    Lord boss = RandomBoss(lord);
                    GConsole.WriteLine("As a result of repeated hits to subordinates reputations, {0} was defamed as well.",
                        boss.ColoredName);
                    RandomBoss(lord).Defame();
                }
            }
        }

        public void Kill(Lord lord)
        {
            lord.Alive = false;

            if(lord.Type == Lord.LordType.Earl)
            {
                GameManager.Instance.Outro();
            }
        }
    }
}
