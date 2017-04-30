using Gash;
using GashLibrary.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    class CrewMember : IKeyword
    {
        private string RawName = "placeholder";

        public bool IsOnMission = false;
        public Action MissionType = null;
        public Lord MissionTarget = null;
        public int MissionTurnCounter = 0;

        public int IndisposedTurnCounter = 0;
        public int InflirtationBanCounter = 0;

        public bool IsBusy {
            get
            {
                if (IsOnMission == true) return true;
                if (IndisposedTurnCounter > 0) return true;

                return false;
            }
        }


        public List<float> SkillsValue;

        public List<Trait> Traits = new List<Trait>();

        public CrewMember()
        {
            RawName = GameManager.Instance.RNG.NextBoolean() ? RandomStrings.RandomFirstFemale() : RandomStrings.RandomFirstMale();
            int traitCount = GameManager.Instance.RNG.Next(2, 5);
            for (int i=0; i < traitCount; i++)
            {
                Trait trait;
                bool found = false;
                while(found == false)
                {
                    trait = new Trait();
                    if(Traits.Exists(x => x.Type == trait.Type) == false)
                    {
                        Traits.Add(trait);
                        found = true;
                    }
                }                
            }

            SkillsValue = new List<float>() { 0, 0, 0, 0, 0, 0, 0 };

            for (int i=0; i < SkillsValue.Count; i++)
            {
                float center = Action.DefaultChance[i];
                foreach(var trait in Traits)
                {
                    trait.ModifySkillValue((Action.ActionType)i, ref center);
                }

                center = Math.Max(Math.Min(1.0f, center), 0.0f);
                SkillsValue[i] = (float)GameManager.Instance.RNG.Normal(center, 0.1f);
                SkillsValue[i] = Math.Max(Math.Min(1.0f, SkillsValue[i]), 0.0f);
            }
        }

        public string ColoredName
        {
            get => GConsole.ColorifyText(2, RawName);
        }

        public string Name => RawName;

        public int SkillToInt(int index)
        {
            int skillInt = 0;
            float absSkill = Math.Abs(SkillsValue[index] - 0.5f);
            if (absSkill > 0.45f)
            {
                skillInt = 3;
            }
            else if (absSkill > 0.3f)
            {
                skillInt = 2;
            }
            else if (absSkill > 0.2f)
            {
                skillInt = 1;
            }
            return Math.Sign(SkillsValue[index] - 0.5f) * skillInt;
        }

        public string SkillToString(int index)
        {
            var skillInt = Math.Abs(SkillToInt(index));
            if (skillInt == 0) return "";

            bool positive = SkillToInt(index) > 0;
            string bonus = new String(positive ? '+' : '-', skillInt);
            bonus = GConsole.ColorifyText(positive ? ConsoleColor.Green : ConsoleColor.Red, ConsoleColor.Black,
            bonus);

            return bonus;
        }

        public void PrintManPage()
        {
            string initialText = String.Format("{0} is one of your crew members,", ColoredName);
            if(IsOnMission == true)
            {
                initialText += String.Format(" on a {0} mission {1} and will be for {2} more day(s).", MissionType.ColoredName,
                    MissionTarget != null ? String.Format(" targeting {0}", MissionTarget.ColoredName) : "",
                    MissionTurnCounter);
            }
            else if(IndisposedTurnCounter > 0)
            {
                initialText += String.Format(" recovering from a failed mission for {1} more day(s)",
                    IndisposedTurnCounter);
            }
            else
            {
                initialText += " currently unoccupied.";
            }

            GConsole.WriteLine(-1.0f, initialText);


            GConsole.WriteLine("Traits: {0}", String.Join(", ", Traits.Select(x => x.ColoredName)));

            GConsole.WriteLine("Notable skills:");

            int maxSkillsPerLine = 6;
            List<string> line = new List<string>();

            for(int i=0; i < SkillsValue.Count; i++)
            {
                string skillString = SkillToString(i);
                if(skillString.Length > 0)
                {
                    line.Add(String.Format("{0}{1}", GameManager.Instance.Actions.ActionTypes[i].ColoredName, skillString));

                    if(line.Count == maxSkillsPerLine)
                    {
                        GConsole.WriteLine(-1.0f, "\t{0}", String.Join(", ",line));
                        line.Clear();
                    }
                }                
            }

            if(line.Count > 0) GConsole.WriteLine(-1.0f, "\t{0}", String.Join(", ", line));
        }

        public void StartMission(Action action, Lord target)
        {
            if(IsBusy == true)
            {
                GConsole.WriteLine("{0} is not ready to start a mission.", ColoredName);
            }
            else if(action.Type == Action.ActionType.Infiltrate && InflirtationBanCounter > 0)
            {
                GConsole.WriteLine("{0} can't start {1} mission for {2} more day(s).", ColoredName,
                    action.ColoredName, InflirtationBanCounter);
            }
            else
            {
                GConsole.WriteLine("{0} started a {1} mission{2}.", ColoredName, action.ColoredName,
                    target != null ? String.Format(" targeting {0}", target.ColoredName) : "");
                IsOnMission = true;
                MissionTarget = target;
                MissionTurnCounter = Action.DefaultDurations[(int)action.Type];
                MissionType = action;
            }
        }

        public void EndDay()
        {
            Trait restlessTrait = Traits.Find(t => t.Type == Trait.TraitType.Restless);
            if (IsBusy == false && restlessTrait != null)
            {
                GConsole.WriteLine("{0} has {1} trait and had nothing to do today.", ColoredName, restlessTrait.ColoredName);

                if(GameManager.Instance.RNG.NextBoolean() == true)
                {
                    float moneyLost = (float)GameManager.Instance.RNG.Normal(5.0f, 0.2f);
                    GConsole.WriteLine("You lost {0} {1} because of that.", moneyLost.ToString("N0"), Assets.KPounds.ColoredName);
                }
                else
                {
                    IndisposedTurnCounter += 1;
                    GConsole.WriteLine("There was a fight because of that and {0} will need 1 day to recuperate.", 
                       ColoredName);
                }
            }
        }

        public void StartDay()
        {
            if(IsOnMission == true)
            {
                MissionTurnCounter--;
                if(MissionTurnCounter == 0)
                {
                    IsOnMission = false;
                    MissionType.Evaluate(this, MissionTarget);
                    MissionType = null;
                }
            }
            else if(IndisposedTurnCounter > 0)
            {
                IndisposedTurnCounter--;
            }

            InflirtationBanCounter--;
        }

        public void IncreseSkill(Action.ActionType type, float value)
        {            
            int prevSkillToInt = SkillToInt((int)type);

            SkillsValue[(int)type] += value;

            int currentSkillToInt = SkillToInt((int)type);

            if (prevSkillToInt != currentSkillToInt)
            {
                if( currentSkillToInt == 0 )
                {
                    GConsole.WriteLine("{0} is now moderate at {1}ing.", ColoredName,
                        GameManager.Instance.Actions.ActionTypes[(int)type].ColoredName);
                }
                else
                {
                    GConsole.WriteLine("{0} is now {1} at {2}ing.", ColoredName, SkillToString((int)type),
                        GameManager.Instance.Actions.ActionTypes[(int)type].ColoredName);
                }

            }
        }
    }
}
