using Gash;
using GashLibrary.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using Troschuetz.Random;
using System.Linq;

namespace Game
{
    class Lord : IKeyword
    {
        private string RawName = "placeholder";

        public bool Alive = true;

        public bool TargetOfaMission = false;

        public int DefameCount = 0;

        public enum LordType { ImportantCitizen, Baron, Viscount, Earl };
        public string[] Titles = {
            "is an important citizen",
            String.Format("is one of the {0}s", Society.KBaron.ColoredName),
            String.Format("is {0}",Society.KViscount.ColoredName),
            String.Format("is {0}",Society.KEarl.ColoredName)
        };

        public LordType Type;

        public int[] ActionResistances;
        public List<bool> ActionResistancesRevealed;
        public float[] ResistanceEffect = {0.0f, 0.1f, 0.25f, 0.5f, 0.8f, 2.0f };
        public float[] AverageResistance =
        {
            //ImportantCitizen
            0.5f,
            //Baron
            2.5f,
            //Viscount
            4,
            //Earl
            5
        };

        public string Name => RawName;

        public Lord(LordType type)
        {
            Type = type;
            TRandom rng = GameManager.Instance.RNG;
            switch (type)
            {
                case LordType.ImportantCitizen:
                    RawName = "Sir " + RandomStrings.RandomImportantNames();
                    ActionResistancesRevealed = new List<bool> { false, false, false, false, false, false, false };
                    ActionResistances = new int[]
                    {
                        // hustle   steal           eavesdrop       infiltrate      blackmail       beat            kill
                        0,       rng.Next(-2, 3),  rng.Next(-2, 3),  rng.Next(-2, 3),  rng.Next(-2, 3),  rng.Next(-2, 3),  rng.Next(-2, 3)
                    };
                    break;
                case LordType.Baron:
                    RawName = "Lord " + RandomStrings.RandomBaronNames();
                    ActionResistancesRevealed = new List<bool> { false, false, false, false, false, false, false };
                    ActionResistances = new int[]
                    {
                    // hustle   steal           eavesdrop       infiltrate      blackmail       beat            kill
                       0,       rng.Next(1,4),  rng.Next(1,4),  rng.Next(1,4),  rng.Next(1,4),  rng.Next(1,4),  rng.Next(1,4)
                    };
                    break;
                case LordType.Viscount:
                    RawName = "Lord " + RandomStrings.RandomViscountNames();
                    ActionResistancesRevealed = new List<bool> { false, false, false, false, false, false, false };
                    ActionResistances = new int[]
                    {
                        // hustle   steal   eavesdrop   infiltrate  blackmail   beat    kill
                        0,          4,      4,          4,          4,          4,      5
                    };
                    break;
                case LordType.Earl:
                    RawName = "Lord " + RandomStrings.RandomEarlNames();
                    ActionResistancesRevealed = new List<bool> { false, false, false, false, false, false, false };
                    ActionResistances = new int[]
                    {
                    // hustle   steal   eavesdrop   infiltrate  blackmail   beat    kill
                       0,       5,      5,          5,          5,          5,      5
                    };
                    break;
            }
        }

        public string ColoredName
        {
            get => GConsole.ColorifyText(2, RawName);
        }

        public bool CompletelyRevealed
        {
            get
            {
                return ActionResistancesRevealed.Count(x => x == false) == 0;
            }
        }

        public void PrintManPage()
        {
            GConsole.WriteLine(-1.0f, "{0} {1} of Underbury.", ColoredName, Titles[(int)Type]);
            if(Alive == false)
            {
                GConsole.WriteLine(-1.0f, "He was assasinated by your crew.");
            }
            else
            {
                GConsole.WriteLine(-1.0f, "Resistances:");
                for (int i = 1; i < ActionResistances.Length; i++)
                {
                    if (ActionResistancesRevealed[i] == true && ActionResistances[i] != 0)
                    {
                        bool positive = ActionResistances[i] > 0;
                        string resistance = new String(positive ? '-' : '+', Math.Abs(ActionResistances[i]));
                        GConsole.WriteLine(-1.0f, "\t{0}{1}",
                            GameManager.Instance.Actions.ActionTypes[i].ColoredName,
                            GConsole.ColorifyText(positive ? ConsoleColor.Red : ConsoleColor.Green, ConsoleColor.Black,
                            resistance));
                    }
                }
            }

        }

        public float ResistanceChanceChange(Action.ActionType type)
        {
            int resistance = ActionResistances[(int)Type];
            return (resistance < 0 ? +1 : -1) * ResistanceEffect[Math.Abs(resistance)];
        }

        public float KnownResistanceChanceChange(Action.ActionType type)
        {
            if(ActionResistancesRevealed[(int)Type] == true)
            {
                int resistance = ActionResistances[(int)Type];
                return (resistance < 0 ? +1 : -1) * ResistanceEffect[Math.Abs(resistance)];
            }
            else
            {
                return 0.0f;
            }
        }

        public int KnownWeakness(int action)
        {
            if (ActionResistancesRevealed[action] == true)
            {
                return ActionResistances[action];
            }
            else
            {
                return 0;
            }
        }

        public string ResistancePrint(int action)
        {
            if(ActionResistances[action] == 0)
            {
                return "0";
            }
            else
            {
                bool positive = ActionResistances[action] > 0;
                string resistance = new String(positive ? '-' : '+', Math.Abs(ActionResistances[action]));
                resistance = GConsole.ColorifyText(positive ? ConsoleColor.Red : ConsoleColor.Green, ConsoleColor.Black,
                            resistance);
                return resistance;
            }

        }

        internal void ListSociety()
        {
            string specialText = "";
            if(Type == LordType.Baron)
            {
                specialText = String.Format(", {0} of Underbury", Society.KBaron.ColoredName);
            }
            else if (Type == LordType.Viscount)
            {
                specialText = String.Format(", {0} of Underbury", Society.KViscount.ColoredName);
            }
            else if (Type == LordType.Earl)
            {
                specialText = String.Format(", {0} of Underbury", Society.KEarl.ColoredName);
            }

            GConsole.WriteLine(-1.0f, "{0}{1}{2}", ColoredName, specialText, Alive == false ? " deceased" : "");

            if(Alive == true)
            {

                int maxResistancePerLine = 6;
                List<string> line = new List<string>();

                for (int i = 1; i < ActionResistances.Length; i++)
                {
                    if (ActionResistancesRevealed[i] == true)
                    {
                        line.Add(String.Format("{0}{1}",
                            GameManager.Instance.Actions.ActionTypes[i].ColoredName,
                            ResistancePrint(i)));

                        if (line.Count == maxResistancePerLine)
                        {
                            GConsole.WriteLine(-1.0f,"\t{0}", String.Join(", ", line));
                            line.Clear();
                        }
                    }
                }

                if (line.Count > 0) GConsole.WriteLine(-1.0f, "\t{0}", String.Join(", ", line));
            }


        }

        public void CompletelyReveal()
        {
            for (int i = 0; i < ActionResistancesRevealed.Count; i++)
            {
                if(ActionResistancesRevealed[i] == false)
                {
                    ActionResistancesRevealed[i] = true;
                    PrintReveal(i);
                }
            }
        }

        private void PrintReveal(int action)
        {
            if (ActionResistances[action] == 0)
            {
                GConsole.WriteLine("You found out that {0} is neither resistant nor weak to {1}.",
                       ColoredName, GameManager.Instance.Actions.ActionTypes[action].ColoredName);
            }
            else if (ActionResistances[action] > 0)
            {
                GConsole.WriteLine("You found out that {0} has {1} resistance to {2}.",
                    ColoredName,
                    ResistancePrint(action),
                    GameManager.Instance.Actions.ActionTypes[action].ColoredName);
            }
            else
            {
                GConsole.WriteLine("You found out that {0} has {1} weakness to {2}.",
                    ColoredName,
                    ResistancePrint(action),
                    GameManager.Instance.Actions.ActionTypes[action].ColoredName);
            }
        }

        public void Reveal(int targetReveals)
        {
            if (targetReveals <= 0) return;

            var potentialReveals = ActionResistancesRevealed.Select((item, index) => new { Item = item, Index = index })
                  .Where(r => r.Item == false && r.Index != 0)
                  .Select(r => r.Index).ToList();

            for (int i=0; i < targetReveals; i++)
            {
                if (potentialReveals.Count <= 0) return;
                int randomIndex = GameManager.Instance.RNG.Next(0, potentialReveals.Count);
                int action = potentialReveals[randomIndex];
                ActionResistancesRevealed[action] = true;
                PrintReveal(action);
                potentialReveals.RemoveAt(randomIndex);
            }
        }

        public void PrintEstimateDefamePrice()
        {
            if(CompletelyRevealed == true)
            {
                GConsole.WriteLine("Defaming {0} will cost exactly {1} {2}",
                    ColoredName, Math.Ceiling(DefamePrice()).ToString("N0"), Assets.KSlander.ColoredName);
            }
            else
            {
                float price = 50.0f;

                var knownResistance = ActionResistances.Select((item, index) => new { Item = item, Index = index })
                    .Where(r => ActionResistancesRevealed[r.Index] == true);
                int knownSum = knownResistance.Sum(r => r.Item);

                float unkwownSum = AverageResistance[(int)Type] * (ActionResistances.Length - knownResistance.Count());

                price += ResistanceFunction(knownSum + unkwownSum);

                // Make it really hard to defame Earl
                if (Type == LordType.Earl)
                {
                    price += 500.0f;
                }

                // Make it harder to defame multiple times
                price += DefameCount * 50.0f;

                GConsole.WriteLine("Defaming {0} will cost approximately {1} {2}",
                    ColoredName, Math.Ceiling(price).ToString("N0"), Assets.KSlander.ColoredName);

            }
        }

        private float ResistanceFunction(float reistanceSum)
        {
            // Fitted to expontial curve high resistance
            return (float)(-9.889988 - (-4.491515 / -0.1075324) * (1 - Math.Exp(+0.1075324 * reistanceSum)));
        }

        public float DefamePrice()
        {
            // Base
            float price = 50.0f;

            // Every resistance makes it more expsive, every weakness cheaper
            int sum = ActionResistances.Sum();
            
            price += ResistanceFunction(sum);

            // Make it really hard to defame Earl
            if (Type == LordType.Earl)
            {
                price += 500.0f;
            }

            // Make it harder to defame multiple times
            price += DefameCount * 50.0f;

            return price;
        }

        public void Defame()
        {
            int count = 1 + GameManager.Instance.RNG.Binomial(0.25, 2);
            for(int i=0; i < count; i++)
            {
                var validIndexes = ActionResistances.Select((item, index) => new { Item = item, Index = index }).
                    Where(x => x.Item > -5 && x.Index > 0).Select(x => x.Index).ToList();

                if (validIndexes.Count == 0) return;

                int randomIndex = GameManager.Instance.RNG.Next(0, validIndexes.Count);
                int wantedActionIndex = validIndexes[randomIndex];
                ActionResistances[wantedActionIndex] -= 1;
                ActionResistancesRevealed[wantedActionIndex] = true;
                if(ActionResistances[wantedActionIndex] != 0)
                {
                    GConsole.WriteLine("{0} resistance to {1} has decreased to {2}",
                        ColoredName, GameManager.Instance.Actions.ActionTypes[wantedActionIndex].ColoredName,
                        ResistancePrint(wantedActionIndex));
                }
                else
                {
                    GConsole.WriteLine("{0} is now neither resistant neither weak to {1}",
                        ColoredName, GameManager.Instance.Actions.ActionTypes[wantedActionIndex].ColoredName);
                }

            }
        }
    }
}
