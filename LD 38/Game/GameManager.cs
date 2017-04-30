using Game.Commands;
using Gash;
using GashLibrary.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using Troschuetz.Random;

namespace Game
{
    class GameManager
    {
        private static GameManager TheOneAndOnly;
        public static GameManager Instance
        {
            get
            {
                if (TheOneAndOnly == null) TheOneAndOnly = new GameManager();
                return TheOneAndOnly;
            }
        }

        private TRandom rng = null;
        public TRandom RNG
        {
            get
            {
#if DEBUG
                if (rng == null) rng = new TRandom(10);
#else
                if (rng == null) rng = new TRandom();
#endif
                return rng;
            }
        }

        public Assets Assets = null;
        public Crew Crew = null;
        public Society Society = null;

        public Actions Actions = null;

        public int Turn = 0;

        public void Intro()
        {
            float introSpeed = 1.0f / 18.0f;

            GConsole.WriteLine(introSpeed, "Not long ago you were a rising star of Honorary West Underbury Company.");
            GConsole.WriteLine(introSpeed, "But envy and spite do like to dally with young gentlemen of a poor origin.");
            GConsole.WriteLine(introSpeed, "Some say it was {0}{1} himself, who developed a dislike for you.",
                Society.Earl.ColoredName, String.Format(", {0} of Underbury", Society.KEarl.ColoredName));
            GConsole.WriteLine(introSpeed, "Nevertheless, you were quickly stripped of all your honors and casted away from the society.");
            GConsole.WriteLine(introSpeed, "With only scraps of your wealth left, you did what any sane man would do in your place.");
            GConsole.WriteLine(1.0f, " ");
            GConsole.WriteLine(introSpeed, "You turned to crime.");

            GConsole.WriteLine(1.0f, " ");

            GConsole.WriteLine(1.0f / 4.0f, "{0} WORLD OF {1}",
                GConsole.ColorifyText(ConsoleColor.Green, ConsoleColor.Black, "small"),
                GConsole.ColorifyText(ConsoleColor.DarkBlue, ConsoleColor.Gray, "UNDERBURY"));

            GConsole.WriteLine(2.0f, " ");

            GConsole.WriteLine(GConsole.ColorifyText(1,
                "This is game is a console game, meaning you will mostly just need a keyboard."));
            GConsole.WriteLine(GConsole.ColorifyText(1,
                "You interact with the game through typing a command and hitting enter."));
            GConsole.WriteLine(GConsole.ColorifyText(1, 
                "To get you started you only need two commands: 'man' and 'list'"));
            GConsole.WriteLine(GConsole.ColorifyText(1,
                "I was toying with idea to just leave you with those instructions."));
            GConsole.WriteLine(GConsole.ColorifyText(1,
                "But since one tend to spend under a minute for LD game which he/she finds confusing,"));
            GConsole.WriteLine(GConsole.ColorifyText(1,
                "let me give your more instructions for this slightly complicated game."));
            GConsole.WriteLine(GConsole.ColorifyText(1,
                "If you are up to the challenge, feel free to skip to the Day 1."));

            GConsole.WriteLine(
                GConsole.ColorifyText(1, "Pretty much any colored text can be used with 'man 'command to display a manual page for it."));

            GConsole.WriteLine(
                GConsole.ColorifyText(1, "The first thing you should type and Enter is 'man man', a manual page for man command ;)"));

            GConsole.WriteLine(GConsole.ColorifyText(1,
                "Your goal is to kill the Earl. If you do so, you win."));
            GConsole.WriteLine(GConsole.ColorifyText(1,
                "However Earl is pretty much untouchable unless you weaken the ones below him."));
            GConsole.WriteLine(GConsole.ColorifyText(1, "You operate a crew of men and women and send them on missions."));
            GConsole.WriteLine(GConsole.ColorifyText(1, "See 'man mission' and 'reportstatus'."));
            GConsole.WriteLine(GConsole.ColorifyText(1,
                "Different type of missions provide, and potentially cost, different type of resources."));
            GConsole.WriteLine(GConsole.ColorifyText(1, "See 'man pound', 'man respect' and 'man slander'."));

            GConsole.WriteLine(GConsole.ColorifyText(1,
                "Mission success depends on crew member's skills, target resitances and a few other factors."));
            GConsole.WriteLine(GConsole.ColorifyText(1,
                "Going into missions blind is not advised. Gather intel by eavesdropping and/or infiltrating missions."));
            GConsole.WriteLine(GConsole.ColorifyText(1,
                "Game is turn based, you can end turn via 'endturn' command."));
            GConsole.WriteLine(GConsole.ColorifyText(1, 
                "Get faimilar with 'man', 'society', 'mission' commands, you will be typing those a lot."));            
            GConsole.WriteLine(GConsole.ColorifyText(1,
                "Oh and two more tips. Tab is your ally for auto-completing and Up/Down Arrows can repeat previous commands."));
            GConsole.WriteLine(GConsole.ColorifyText(1,
                "Good luck!"));
        }

        public void Outro()
        {
            GConsole.WriteLine(" ");

            GConsole.WriteLine("Revenge best served cold, cold as a lifeless body of {0}{1}.",
                Society.Earl.ColoredName, String.Format(", {0} of Underbury", Society.KEarl.ColoredName));
            GConsole.WriteLine("Once a rising star, now a full star.");

            GConsole.WriteLine("The {0} WORLD OF {1} is now too small for you.",
                GConsole.ColorifyText(ConsoleColor.Green, ConsoleColor.Black, "small"),
                GConsole.ColorifyText(ConsoleColor.DarkBlue, ConsoleColor.Gray, "UNDERBURY"));

            GConsole.WriteLine("Go on.");
            GConsole.WriteLine("Life won't wait on you.");

            GConsole.WriteLine("Thanks for playing!");

            GConsole.PauseOutput();
        }

        public void Setup()
        {
            GConsole.RegisterCommand(new GameState());
            GConsole.RegisterCommand(new Exit());
            GConsole.RegisterCommand(new ListSociety());
            GConsole.RegisterCommand(new StartMission());
            GConsole.RegisterCommand(new EndDay());
            GConsole.RegisterCommand(new Defame());
            GConsole.RegisterCommand(new Hire());

            GConsole.RegisterKeyword(Assets.KPounds);
            GConsole.RegisterKeyword(Assets.KSlander);
            GConsole.RegisterKeyword(Assets.KRespect);

            GConsole.RegisterKeyword(Society.KBaron);
            GConsole.RegisterKeyword(Society.KViscount);
            GConsole.RegisterKeyword(Society.KEarl);

            GConsole.Settings.Higlights.Add(new HighlightType() { Foreground = ConsoleColor.Magenta });
            GConsole.Settings.Higlights.Add(new HighlightType() { Foreground = ConsoleColor.Yellow });

            Assets = new Assets();
            Crew = new Crew();
            Society = new Society();
            Actions = new Actions();

            List<Trait.TraitType> knowTypes = new List<Trait.TraitType>();
            foreach (var crewmember in Crew)
            {
                foreach (var trait in crewmember.Traits)
                {
                    if (knowTypes.Exists(x => x == trait.Type) == false)
                    {
                        knowTypes.Add(trait.Type);
                        GConsole.RegisterKeyword(trait);
                    }
                }

                GConsole.RegisterKeyword(crewmember);
            }

            foreach (var lord in Society)
            {
                GConsole.RegisterKeyword(lord);
            }

            foreach (var action in Actions)
            {
                GConsole.RegisterKeyword(action);
            }
        }

        public void StartMission(Action action, CrewMember crew, Lord target)
        {
            if (Action.Price[(int)action.Type] > GameManager.Instance.Assets.Money)
            {
                GConsole.WriteLine("Insufficient funds to start {0}. You need atleast {1} {2}.",
                    action.ColoredName, Action.Price[(int)action.Type].ToString("N0"), Assets.KPounds.ColoredName);
            }
            else
            {
                if(target != null) target.TargetOfaMission = true;

                crew.StartMission(action, target);
                if(Action.Price[(int)action.Type] > 0)
                {
                    GConsole.WriteLine("Preparations for the mission will cost you {0} {1}.",
                    Action.Price[(int)action.Type].ToString("N0"), Assets.KPounds.ColoredName);
                }

                GConsole.WriteLine(-1.0f, " ");
                GameManager.Instance.Assets.Money -= Action.Price[(int)action.Type];

                if(Crew.AnyoneAvailableForMission == false)
                {
                    EndTurn();
                }
                
            }
        }

        public void PrintGameState()
        {
            Assets.PrintAssets();
            Crew.PrintFreeAndRecoveringMembers();
            Crew.PrintMembersOnMission();
            GConsole.WriteLine(-1.0f, " ");
        }

        public void StartTurn()
        {
            Turn++;
            GConsole.WriteLine("Day {0} has begun.", Turn);
            GConsole.WriteLine(-1.0f, " ");

            foreach (var m in Crew)
            {
                m.StartDay();
            }

            PrintGameState();

            if (Crew.AnyoneAvailableForMission == false)
            {
                GConsole.WriteLine(GConsole.ColorifyText(1,"The entire crew is occupied, ending the day automatically."));
                EndTurn();
            }
        }

        public void EndTurn()
        {
            foreach(var m in Crew)
            {
                m.EndDay();
            }

            StartTurn();
        }
    }
}
