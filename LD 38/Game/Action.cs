using Gash;
using GashLibrary.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game
{ 
    class Action : IKeyword
    {
        private string RawName = "placeholder";

        public enum ActionType { Hustle, Steal, Eavesdrop, Infiltrate, Blackmail, BeatUp, Kill };

        public enum ActionResultType { CriticalFailure, Failure, Succes, MajorSuccess };

        public ActionType Type;

        public static string[] Names =
        {
            "hustle", "steal", "eavesdrop", "infiltrate", "blackmail", "beat", "kill"
        };

        public static int[] DefaultDurations =
        {
// hustle   steal   eavesdrop   infiltrate  blackmail   beat    kill
   1,       3,      1,          3,          4,          3,      5
        };

        public static float[] DefaultChance =
        {
// hustle   steal   eavesdrop   infiltrate  blackmail   beat    kill
   0.8f,    0.5f,   0.7f,       0.5f,       0.2f,       0.15f,  0.1f
        };

        public static float[] Price =
        {
// hustle   steal   eavesdrop   infiltrate  blackmail   beat    kill
   0.0f,    2.0f,   0.0f,       25.0f,      10.0f,      2.0f,   50.0f
        };

        private static string[] Descriptions =
        {
            "Ain't no rest for the wicked, so one has to {1}. Provides small amount of money and trains in weaknesses.",
            "{1}ing, the bread and butter of any crime operation. Provides medium amount of money.",
            "{1}ing might not be the most exciting job but knowledge is power. Provides slander and valuable information.",
            "Big brother of eavesdropping, {1} takes time but provides greater rewards.",
            "Everyone has their secrets and {1} can turn them into considerable profit.",
            "Nothing sends a message to a weakling like a good {1} up. However that message has many recipients.",
            "{1}, a powerful move that can end one life but surely attracts a lot of attention."
        };

        private static string[,] ResultStrings =
        {
            // hustle
            //  critfailure
            //  failure
            //  success
            //  majorsucess
            {   "{0} completely wasted the day on the streets.",
                "{0} got distracted when strolling the streets.",
                "{0} time on the street proved useful.",
                "Yesterday was a good day for {0}."
            },
            // steal
            //  critfailure
            //  failure
            //  success
            //  majorsucess
            {   "{0} got caught stealing from {1}.",
                "{0} failed to rob {1}.",
                "{0} robbed {1}.",
                "{0} pulled off major heist on {1}."
            },
            // eavesdrop
            //  critfailure
            //  failure
            //  success
            //  majorsucess
            {   "{0} was noticed when following {1}.",
                "{0} did not overheard anything useful when following {1}",
                "{0} overheard valuable information when following {1}.",
                "{0} heard and remembered every word {1} uttered that day."
            },
            // infiltrate
            //  critfailure
            //  failure
            //  success
            //  majorsucess
            {   "{0} was kicked out of {1}'s house and told to never return.",
                "{0} did not pass the screening to be employed by {1}.",
                "{0} got hired into the service of {1}. Many things were learned.",
                "{0} quickly became favourite servant of {1}. Many secrets were reveled."
            },
            // blackmail
            //  critfailure
            //  failure
            //  success
            //  majorsucess
            {   "{0} got laughed at when attempting to blackmail {1}.",
                "{0} demands for {1} were refused.",
                "{0} blackmailed {1}",
                "{1} almost pissed his pants when {0} confronted him."
            },
            // beat
            //  critfailure
            //  failure
            //  success
            //  majorsucess
            {   "{0} not only failed to beat up {1} but was beaten back.",
                "{0} and {1} brawl ended with no clear winner.",
                "{1} was beaten by {0}",
                "{1} was on the bring of death after {0} peformance."
            },
            // kill
            //  critfailure
            //  failure
            //  success
            //  majorsucess
            {   "{0} got caught attempting to murder {1}.",
                "{1} escaped by {0}'s assasination attempt.",
                "{1} was assasinated by {0}.",
                "Chances are, {0} murder of {1} won't ever be solved"
            }            
        };

        private static float[,,] MoneyReward =
        {
// hustle
// critfailure      failure         success         majorsucess
{ {0.0f, 0.0f},     {1.0f, 0.0f},   {5.0f, 0.5f},   {15.0f, 2.0f} },
// steal
// critfailure      failure         success         majorsucess
{ {0.0f, 0.0f},     {0.0f, 0.0f},   {30.0f, 4.0f},  {50.0f, 4.0f} },
// eavesdrop
// critfailure      failure         success         majorsucess
{ {0.0f, 0.0f},     {0.0f, 0.0f},   {0.0f, 0.0f},   {0.0f, 0.0f} },
// infiltrate
// critfailure      failure         success         majorsucess
{ {0.0f, 0.0f},     {0.0f, 0.0f},   {0.0f, 0.0f},   {0.0f, 0.0f} },
// blackmail
// critfailure      failure         success         majorsucess
{ {0.0f, 0.0f},     {0.0f, 0.0f},   {70.0f, 5.0f},  {120.0f, 10.0f} },
// beat 
// critfailure      failure         success         majorsucess
{ {0.0f, 0.0f},     {0.0f, 0.0f},   {5.0f, 0.5f},   {5.0f, 0.5f} },
// kill
// critfailure      failure         success         majorsucess
{ {0.0f, 0.0f},     {0.0f, 0.0f},   {10.0f, 0.5f},   {10.0f, 0.5f} },
        };

        private static float[,,] SlanderReward =
        {
// hustle
// critfailure      failure         success         majorsucess
{ {0.0f, 0.0f},     {0.0f, 0.0f},   {1.0f, 0.0f},   {3.0f, 0.5f} },
// steal
// critfailure      failure         success         majorsucess
{ {0.0f, 0.0f},     {0.0f, 0.0f},   {0.0f, 0.0f},  {5.0f, 0.5f} },
// eavesdrop
// critfailure      failure         success         majorsucess
{ {0.0f, 0.0f},     {0.0f, 0.0f},   {10.0f, 1.0f},   {50.0f, 4.0f} },
// infiltrate
// critfailure      failure         success         majorsucess
{ {0.0f, 0.0f},     {0.0f, 0.0f},   {100.0f, 5.0f},   {200.0f, 10.0f} },
// blackmail
// critfailure      failure         success         majorsucess
{ {0.0f, 0.0f},     {0.0f, 0.0f},   {25.0f, 1.0f},   {50.0f, 3.0f} },
// beat 
// critfailure      failure         success         majorsucess
{ {0.0f, 0.0f},     {0.0f, 0.0f},   {0.0f, 0.0f},   {0.0f, 0.0f} },
// kill
// critfailure      failure         success         majorsucess
{ {0.0f, 0.0f},     {0.0f, 0.0f},   {0.0f, 0.0f},   {0.0f, 0.0f} },
        };

        private static float[,,] RespectReward =
        {
// hustle
// critfailure      failure         success         majorsucess
{ {0.0f, 0.0f},     {0.0f, 0.0f},   {0.01f, 0.0f},  {0.02f, 0.0f} },
// steal
// critfailure      failure         success         majorsucess
{ {-0.1f, 0.0f},     {0.0f, 0.0f},   {0.1f, 0.0f},  {0.2f, 0.0f} },
// eavesdrop
// critfailure      failure         success         majorsucess
{ {0.0f, 0.0f},     {0.0f, 0.0f},   {0.0f, 0.0f},   {0.0f, 0.0f} },
// infiltrate
// critfailure      failure         success         majorsucess
{ {0.0f, 0.0f},     {0.0f, 0.0f},   {0.0f, 0.0f},   {0.0f, 0.0f} },
// blackmail
// critfailure      failure         success         majorsucess
{ {-0.4f, 0.0f},     {0.0f, 0.0f},   {0.2f, 0.0f},   {0.4f, 0.0f} },
// beat 
// critfailure      failure         success         majorsucess
{ {-0.5f, 0.0f},     {0.0f, 0.0f},   {0.5f, 0.0f},   {1.0f, 0.0f} },
// kill
// critfailure      failure         success         majorsucess
{ {-1.0f, 0.0f},     {0.0f, 0.0f},   {2.0f, 0.0f},   {3.0f, 0.0f} },
        };

        public string Name => RawName;

        public string ColoredName => GConsole.ColorifyText(3, RawName);

        public Action(ActionType type)
        {
            RawName = Names[(int)type];
            Type = type;
        }

        private float CrewBasedSuccessChance(CrewMember crew)
        {
            return crew.SkillsValue[(int)Type];
        }

        private float RespectSuccessBonus()
        {
            float control = GameManager.Instance.Assets.Respect / 10.0f;
            return control * 0.4f;
        }

        public float SuccessChance(CrewMember crew, Lord target)
        {
            float result = CrewBasedSuccessChance(crew);
            result += RespectSuccessBonus();
            if (target != null) result += target.ResistanceChanceChange(Type);
            result = Math.Max(Math.Min(1.0f, result), 0.0f);
            return result;
        }

        public float EstimateSucccesChance(CrewMember crew, Lord target)
        {
            float result = CrewBasedSuccessChance(crew);
            result += RespectSuccessBonus();
            if (target != null) result += target.KnownResistanceChanceChange(Type);
            result = Math.Max(Math.Min(1.0f, result), 0.0f);
            return result;
        }

        public void Evaluate(CrewMember crew, Lord target)
        {
            ActionResultType result;

            if (target != null) target.TargetOfaMission = false;

            float chance = SuccessChance(crew, target);
            double diceRoll = GameManager.Instance.RNG.NextDouble();

            if(diceRoll <= chance)
            {
                // Is it major success?
                if(diceRoll <= 0.1f * chance)
                {
                    result = ActionResultType.MajorSuccess;
                }
                else
                {
                    result = ActionResultType.Succes;
                }
            }
            else
            {
                // Is it critical failure?
                if (diceRoll >= 1.0f - (0.1f * chance))
                {
                    result = ActionResultType.CriticalFailure;
                }
                else
                {
                    result = ActionResultType.Failure;
                }
            }

            float moneyReward = 0.0f;

            float mean = MoneyReward[(int)Type, (int)result, 0];
            float var = MoneyReward[(int)Type, (int)result, 1];
            if(var > 0.0f)
            {
                moneyReward = (float)GameManager.Instance.RNG.Normal(mean, var);
            }
            else
            {
                moneyReward = mean;
            }

            GameManager.Instance.Assets.Money += moneyReward;

            float slanderReward = 0.0f;

            mean = SlanderReward[(int)Type, (int)result, 0];
            var = SlanderReward[(int)Type, (int)result, 1];
            if (var > 0.0f)
            {
                slanderReward = (float)GameManager.Instance.RNG.Normal(mean, var);
            }
            else
            {
                slanderReward = mean;
            }

            GameManager.Instance.Assets.Slander += slanderReward;

            float respectReward = RespectReward[(int)Type, (int)result, 0];
            float previousRespect = (float)Math.Floor((double)GameManager.Instance.Assets.Respect);
            GameManager.Instance.Assets.Respect += respectReward;
            GameManager.Instance.Assets.Respect = Math.Min(GameManager.Instance.Assets.Respect, 10.0f);

            GConsole.WriteLine(ResultStrings[(int)Type, (int)result],
                crew.ColoredName, target != null ? target.ColoredName : "");

            // Getting caught
            if (Type == ActionType.Steal && result == ActionResultType.CriticalFailure)
            {
                GConsole.WriteLine("{0} will spend 3 days in prison because of that.", crew.ColoredName);
                crew.IndisposedTurnCounter += 3;
            }
            else if(Type == ActionType.Eavesdrop && result == ActionResultType.CriticalFailure)
            {
                GConsole.WriteLine("{0} has to lay low for 1 day because of that.", crew.ColoredName);
                crew.IndisposedTurnCounter += 1;
            }
            else if (Type == ActionType.BeatUp && result == ActionResultType.CriticalFailure)
            {
                GConsole.WriteLine("{0} will be out for 5 days because of that.", crew.ColoredName);
                crew.IndisposedTurnCounter += 5;
            }
            else if (Type == ActionType.Kill && result == ActionResultType.CriticalFailure)
            {
                GConsole.WriteLine("{0} will spend 20 days in prison because of that.", crew.ColoredName);
                crew.IndisposedTurnCounter += 20;
            }

            if(Type == ActionType.Infiltrate && result == ActionResultType.CriticalFailure)
            {
                GConsole.WriteLine("{0} can't start infiltration mission for following 10 days because of that.", crew.ColoredName);
                crew.InflirtationBanCounter += 10;
            }

            if (Math.Abs(moneyReward) > 1.0f)
            {
                GConsole.WriteLine("You {0} {1} {2}.",
                    moneyReward > 0.0f ? "gained" : "lost", (Math.Abs(moneyReward)).ToString("N0"), Assets.KPounds.ColoredName);
            }

            if (Math.Abs(slanderReward) > 1.0f)
            {
                GConsole.WriteLine("You {0} {1} {2}.",
                    slanderReward > 0.0f ? "gained" : "lost", (Math.Abs(slanderReward)).ToString("N0"), Assets.KSlander.ColoredName);
            }

            float newRespect = (float)Math.Floor((double)GameManager.Instance.Assets.Respect);
            if(Math.Abs(newRespect-previousRespect) >= 1.0f)
            {
                GConsole.WriteLine("Your {2} in society has {0} to {1}.",
    (newRespect - previousRespect) > 0.0f ? "increased" : "decresed", newRespect.ToString("N0"), Assets.KRespect.ColoredName);
            }

            Trait slowlearner = crew.Traits.Find(t => t.Type == Trait.TraitType.Slowlearner);
            if (slowlearner != null)
            {
                if (result <= ActionResultType.Failure)
                {
                    GConsole.WriteLine("{0} got a tiny ({1} trait) better at {2}.", crew.ColoredName, slowlearner.ColoredName, ColoredName);
                    crew.IncreseSkill(Type,0.025f);
                }
                else
                {
                    GConsole.WriteLine("{0} got mildly ({1} trait) better at {2}.", crew.ColoredName, slowlearner.ColoredName, ColoredName);
                    crew.IncreseSkill(Type, 0.04f);
                }
            }
            else
            {
                if (result <= ActionResultType.Failure)
                {
                    GConsole.WriteLine("{0} got a little better at {1}.", crew.ColoredName, ColoredName);
                    crew.IncreseSkill(Type, 0.05f);
                }
                else
                {
                    GConsole.WriteLine("{0} got better at {1}.", crew.ColoredName, ColoredName);
                    crew.IncreseSkill(Type, 0.15f);
                }
            }




            // Reveals
            if (target != null)
            {                
                if (result == ActionResultType.Succes && Type == ActionType.Eavesdrop)
                {
                    if(target.CompletelyRevealed == false)
                    {
                        // Target 1-3 
                        int targetReveals = 1 + GameManager.Instance.RNG.Binomial(0.25, 2);
                        target.Reveal(targetReveals);

                        // Same level lord 0-2
                        Lord neighbour = GameManager.Instance.Society.RandomNeigbour(target);
                        int neighbourdReveals = GameManager.Instance.RNG.Binomial(0.25, 2);
                        if(neighbour != null) neighbour.Reveal(neighbourdReveals);
                    }
                    else
                    {
                        // Same level lord 1-2
                        Lord neighbour = GameManager.Instance.Society.RandomNeigbour(target);
                        int neighbourdReveals = 1 + GameManager.Instance.RNG.Binomial(0.25, 1);
                        if (neighbour != null) neighbour.Reveal(neighbourdReveals);
                    }
                }
                else if(result == ActionResultType.MajorSuccess && Type == ActionType.Eavesdrop)
                {
                    if (target.CompletelyRevealed == false)
                    {
                        // Target 2-4 
                        int targetReveals = 2 + GameManager.Instance.RNG.Binomial(0.25, 2);
                        target.Reveal(targetReveals);

                        // 1 same level lord 1-3
                        for(int i = 0; i < 1; i++)
                        {
                            Lord neighbour = GameManager.Instance.Society.RandomNeigbour(target);
                            int neighbourdReveals = 1 + GameManager.Instance.RNG.Binomial(0.25, 2);
                            if (neighbour != null) neighbour.Reveal(neighbourdReveals);
                        }
                    }
                    else
                    {
                        // 1 same level lord 2-4
                        for (int i = 0; i < 1; i++)
                        {
                            Lord neighbour = GameManager.Instance.Society.RandomNeigbour(target);
                            int neighbourdReveals = 2 + GameManager.Instance.RNG.Binomial(0.25, 2);
                            if (neighbour != null) neighbour.Reveal(neighbourdReveals);
                        }
                    }
                }
                else if(result == ActionResultType.Succes && Type == ActionType.Infiltrate)
                {
                    if (target.CompletelyRevealed == false)
                    {
                        target.CompletelyReveal();

                        // 2 same level lord 1-3
                        for (int i = 0; i < 2; i++)
                        {
                            Lord neighbour = GameManager.Instance.Society.RandomNeigbour(target);
                            int neighbourdReveals = 1 + GameManager.Instance.RNG.Binomial(0.25, 2);
                            if (neighbour != null) neighbour.Reveal(neighbourdReveals);
                        }

                        // Random boss 0-1
                        Lord boss = GameManager.Instance.Society.RandomBoss(target);
                        int reveals = GameManager.Instance.RNG.Binomial(0.5, 1);
                        if(boss != null) boss.Reveal(reveals);
                    }
                    else
                    {
                        // 2 same level lord 2-4
                        for (int i = 0; i < 2; i++)
                        {
                            Lord neighbour = GameManager.Instance.Society.RandomNeigbour(target);
                            int neighbourdReveals = 2 + GameManager.Instance.RNG.Binomial(0.25, 2);
                            if (neighbour != null) neighbour.Reveal(neighbourdReveals);
                        }

                        // Random boss 0-1
                        Lord boss = GameManager.Instance.Society.RandomBoss(target);
                        int reveals = GameManager.Instance.RNG.Binomial(0.75, 1);
                        if (boss != null) boss.Reveal(reveals);
                    }
                }
                else if (result == ActionResultType.MajorSuccess && Type == ActionType.Infiltrate)
                {
                    if (target.CompletelyRevealed == false)
                    {
                        target.CompletelyReveal();

                        // 2 same level lord 2-4
                        for (int i = 0; i < 2; i++)
                        {
                            Lord neighbour = GameManager.Instance.Society.RandomNeigbour(target);
                            int neighbourdReveals = 2 + GameManager.Instance.RNG.Binomial(0.5, 2);
                            if (neighbour != null) neighbour.Reveal(neighbourdReveals);
                        }

                        // Random boss 1
                        Lord boss = GameManager.Instance.Society.RandomBoss(target);
                        if (boss != null) boss.Reveal(1);
                    }
                    else
                    {
                        // 2 same level lord 2-4
                        for (int i = 0; i < 2; i++)
                        {
                            Lord neighbour = GameManager.Instance.Society.RandomNeigbour(target);
                            int neighbourdReveals = 2 + GameManager.Instance.RNG.Binomial(0.5, 2);
                            if (neighbour != null) neighbour.Reveal(neighbourdReveals);
                        }

                        // Random boss 1
                        Lord boss = GameManager.Instance.Society.RandomBoss(target);
                        int reveals = GameManager.Instance.RNG.Binomial(0.75, 1);
                        if (boss != null) boss.Reveal(1);
                    }
                }
            }

            if(Type == ActionType.Hustle && result > ActionResultType.Failure)
            {
                bool anyChange = false;
                for(int i=1; i < crew.SkillsValue.Count; i++)
                {
                    if(crew.SkillToInt(i) < 0)
                    {
                        anyChange = true;
                        break;
                    }
                }

                if(anyChange)
                {
                    GConsole.WriteLine("Succesful hustling improved {0} weaknesses.", crew.ColoredName);

                    for (int i = 1; i < crew.SkillsValue.Count; i++)
                    {
                        if (crew.SkillToInt(i) < 0)
                        {
                            crew.IncreseSkill((Action.ActionType)i, 0.1f);
                        }
                    }
                }
                else
                {
                    GConsole.WriteLine("{0} is already trained in the art of the street and no longer improves his weakness.",
                        crew.ColoredName);
                }
            }

            // Kill
            if (Type == ActionType.Kill && result > ActionResultType.Failure)
            {
                GameManager.Instance.Society.Kill(target);
            }

            GConsole.WriteLine(" ");
        }

        public void PrintManPage()
        {
            GConsole.WriteLine(-1.0f, Descriptions[(int)Type], Name, ColoredName);
            foreach(var crew in GameManager.Instance.Crew)
            {
                if(crew.SkillToInt((int)Type) > 0)
                {
                    GConsole.WriteLine("\t{0} is {1} good at it.", crew.ColoredName, crew.SkillToString((int)Type));
                }
                else if(crew.SkillToInt((int)Type) < 0)
                {
                    GConsole.WriteLine("\t{0} is {1} bad at it.", crew.ColoredName, crew.SkillToString((int)Type));
                }
            }
            foreach (var lord in GameManager.Instance.Society)
            {
                if (lord.Alive == true && lord.KnownWeakness((int)Type) < 0)
                {
                    GConsole.WriteLine("\t{0} is {1} vulnerable to it.", lord.ColoredName, lord.ResistancePrint((int)Type));
                }
            }
            GConsole.WriteLine(-1.0f, " ");
        }        
    }
}
