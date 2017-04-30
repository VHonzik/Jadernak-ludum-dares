using Gash;
using Gash.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Commands
{
    class StartMission : ICommand
    {
        List<BoolFlag> Flags = new List<BoolFlag>() { new BoolFlag("info", false) };

        public AutoCompletionResult AutoComplete(string line)
        {
            var result = ParsingHelpers.AutoCompleteCommandBody(line, this);
            if(result.WasSuccessful == AutoCompletionResultType.FailureAlreadyComplete)
            {
                var secondResult = ParsingHelpers.AutoCompleteStringList(line.Substring(result.RemainderStartPosition),
                    GameManager.Instance.Crew.GetNames());

                if(secondResult.WasSuccessful == AutoCompletionResultType.SuccessOneOption)
                {
                    var finalResult = new AutoCompletionResult();
                    finalResult.WasSuccessful = AutoCompletionResultType.SuccessOneOption;
                    finalResult.Results.Add(line.Substring(0, result.RemainderStartPosition - 1) + " " + secondResult.Results[0]);
                    return finalResult;

                }
                else if(secondResult.WasSuccessful == AutoCompletionResultType.FailureAlreadyComplete)
                {
                    var thirdResult = ParsingHelpers.AutoCompleteStringList(line.Substring(
                        result.RemainderStartPosition+secondResult.RemainderStartPosition),
                        new List<String>(Action.Names));

                    if(thirdResult.WasSuccessful == AutoCompletionResultType.SuccessOneOption)
                    {
                        var finalResult = new AutoCompletionResult();
                        finalResult.WasSuccessful = AutoCompletionResultType.SuccessOneOption;
                        finalResult.Results.Add(line.Substring(0, result.RemainderStartPosition + secondResult.RemainderStartPosition - 1) + 
                            " "+ thirdResult.Results[0]);
                        return finalResult;
                    }
                    else if(thirdResult.WasSuccessful == AutoCompletionResultType.FailureAlreadyComplete)
                    {
                        var fourthResult = ParsingHelpers.AutoCompleteStringList(line.Substring(
                            result.RemainderStartPosition + secondResult.RemainderStartPosition + thirdResult.RemainderStartPosition - 1),
                        GameManager.Instance.Society.GetNames(), true);

                        if (fourthResult.WasSuccessful == AutoCompletionResultType.SuccessOneOption)
                        {
                            var finalResult = new AutoCompletionResult();
                            finalResult.WasSuccessful = AutoCompletionResultType.SuccessOneOption;
                            finalResult.Results.Add(line.Substring(0,
                                result.RemainderStartPosition + secondResult.RemainderStartPosition + thirdResult.RemainderStartPosition - 1) +
                                " " + fourthResult.Results[0]);
                            return finalResult;
                        }

                        return fourthResult;
                    }

                    return thirdResult;
                }

                return secondResult;
            }

            return result;
        }

        public bool Available()
        {
            return GameManager.Instance.Crew.AnyoneAvailableForMission;
        }

        public IEnumerable<BoolFlag> GetFlags()
        {
            return Flags;
        }

        public string Name()
        {
            return "mission";
        }

        public ParsingResult Parse(string line)
        {
            var result = ParsingHelpers.ParseCommand(line, this, true);

            if (result.Type == ParsingResultType.Success)
            {
                if (result.Parameters.Count < 2)
                {
                    GConsole.WriteLine("{0} {1}.",
        GConsole.ColorifyText(1, "Not enough parameters. See"),
        GConsole.ColorifyText(0, "man mission"));
                    result.Type = ParsingResultType.ParsingFailure;
                    return result;
                }


                CrewMember crew = GameManager.Instance.Crew.Members.Find(m => m.Name == result.Parameters[0]);
                if(crew == null)
                {
                    GConsole.WriteLine("{0} {1}.",
                        GConsole.ColorifyText(1, "Unknown crew member. See"),
                        GConsole.ColorifyText(0, "man mission"));
                    result.Type = ParsingResultType.ParsingFailure;
                    return result;
                }

                if (crew.IsBusy == true)
                {
                    GConsole.WriteLine(GConsole.ColorifyText(1, "Crew member is already on mission or can't start a mission."));
                    result.Type = ParsingResultType.ParsingFailure;
                    return result;
                }

                List<string> actionNames = new List<string>(Action.Names);

                int actionIndex = actionNames.IndexOf(result.Parameters[1]);
                if (actionIndex < 0)
                {
                    GConsole.WriteLine("{0} {1}.",
                        GConsole.ColorifyText(1, "Unknown mission type to start. See"),
                        GConsole.ColorifyText(0, "man mission"));
                    result.Type = ParsingResultType.ParsingFailure;
                    return result;
                }

                Action action = GameManager.Instance.Actions.ActionTypes[actionIndex];

                Lord lord = null;

                if(action.Type != Action.ActionType.Hustle)
                {
                    if (result.Parameters.Count < 3)
                    {
                        GConsole.WriteLine("{0} {1}",
            GConsole.ColorifyText(1, "Missing target from society. See"),
            GConsole.ColorifyText(0, "man mission"));
                        result.Type = ParsingResultType.ParsingFailure;
                        return result;
                    }

                    string lordname = String.Join(" ", result.Parameters.GetRange(2, result.Parameters.Count-2));
                    lord = GameManager.Instance.Society.Lords.Find(m => m.Name == lordname);

                    if (lord == null)
                    {
                        GConsole.WriteLine("{0} {1}",
                            GConsole.ColorifyText(1, "Unknown society member. See"),
                            GConsole.ColorifyText(0, "man mission"));
                        result.Type = ParsingResultType.ParsingFailure;
                        return result;
                    }

                    if (lord.Alive == false)
                    {
                        GConsole.WriteLine("{0} is dead, let him rest in peace.", lord.ColoredName);
                        return result;
                    }

                    if (lord.TargetOfaMission == true)
                    {
                        GConsole.WriteLine("{0} is already target of another mission.", lord.ColoredName);
                        return result;
                    }
                }

                if (result.Flags.Count > 0)
                {
                    float chance = action.EstimateSucccesChance(crew, lord);
                    string chanceName = "";
                    if(chance < 0.1)
                    {
                        chanceName = "very low";
                    }
                    else if( chance < 0.4)
                    {
                        chanceName = "low";
                    }
                    else if (chance < 0.7)
                    {
                        chanceName = "medium";
                    }
                    else if (chance < 0.9)
                    {
                        chanceName = "high";
                    }
                    else
                    {
                        chanceName = "very high";
                    }

                    GConsole.WriteLine("The chances of {0} succeeding in {1}{2} are {3}.",
                        crew.ColoredName,
                        action.ColoredName,
                        lord != null ? String.Format(" targeting {0}", lord.ColoredName) : "",
                        chanceName);

                    return result;
                }
                else
                {
                    GameManager.Instance.StartMission(action, crew, lord);
                    return result;
                }
            }

            return result;
        }

        public void PrintManPage()
        {
            GConsole.WriteLine(-1.0f, "{0} {1}",
               GConsole.ColorifyText(1, Resources.text.ManHeaderIntro),
               GConsole.ColorifyText(0, Name()));
            GConsole.WriteLine(-1.0f, GConsole.ColorifyText(1, Resources.text.ManHeaderName));
            GConsole.WriteLine(-1.0f, "\t{0}", GConsole.ColorifyText(0, Name()));
            GConsole.WriteLine(-1.0f, GConsole.ColorifyText(1, Resources.text.ManHeaderSynopsis));
            GConsole.WriteLine(-1.0f, "\t{0} {1} {2} -info",
                GConsole.ColorifyText(0, Name()),
                GConsole.ColorifyText(ConsoleColor.Black, GConsole.Settings.Higlights[2].Foreground, "(crew member name)"),
                GConsole.ColorifyText(ConsoleColor.Black, GConsole.Settings.Higlights[3].Foreground, "(mission type)")
            );
            GConsole.WriteLine(-1.0f, "\t{0} {1} {2} {3} -info",
                GConsole.ColorifyText(0, Name()),
                GConsole.ColorifyText(ConsoleColor.Black, GConsole.Settings.Higlights[2].Foreground, "(crew member name)"),
                GConsole.ColorifyText(ConsoleColor.Black, GConsole.Settings.Higlights[3].Foreground, "(mission type)"),
                GConsole.ColorifyText(ConsoleColor.Black, GConsole.Settings.Higlights[2].Foreground, "(society member)")
            );
            GConsole.WriteLine(-1.0f, "\t{0} {1} {2}",
                GConsole.ColorifyText(0, Name()),
                GConsole.ColorifyText(ConsoleColor.Black, GConsole.Settings.Higlights[2].Foreground, "(crew member name)"),
                GConsole.ColorifyText(3, "hustle")
            );
            GConsole.WriteLine(-1.0f, "\t{0} {1} {2} -info",
                GConsole.ColorifyText(0, Name()),
                GConsole.ColorifyText(ConsoleColor.Black, GConsole.Settings.Higlights[2].Foreground, "(crew member name)"),
                GConsole.ColorifyText(3, "hustle")
            );
            GConsole.WriteLine(-1.0f, "\t{0} {1} {2} {3}",
                GConsole.ColorifyText(0, Name()),
                GConsole.ColorifyText(ConsoleColor.Black,GConsole.Settings.Higlights[2].Foreground, "(crew member name)"),
                GConsole.ColorifyText(ConsoleColor.Black, GConsole.Settings.Higlights[3].Foreground, "(mission type)"),
                GConsole.ColorifyText(ConsoleColor.Black, GConsole.Settings.Higlights[2].Foreground, "(society member)")
            );
            GConsole.WriteLine(-1.0f, GConsole.ColorifyText(1, Resources.text.ManHeaderDescription));           

            GConsole.WriteLine(-1.0f,
                GConsole.ColorifyText(1, "Sends a crew member on a mission or estimates chances of success."));
            GConsole.WriteLine(-1.0f,
    GConsole.ColorifyText(1, "If -info flag is present instead of sending on mission, chances of success are printed."));
            GConsole.WriteLine(-1.0f,
                GConsole.ColorifyText(1, "There are following mission types:"));

            foreach(var action in GameManager.Instance.Actions)
            {
                GConsole.WriteLine(-1.0f,"\t{0}", action.ColoredName);
            }

            GConsole.WriteLine(-1.0f, "{0} {1} {2}",
                GConsole.ColorifyText(1, "To find out if your crew members are ready you can use"),
                GConsole.ColorifyText(0, "reportstatus"),
                GConsole.ColorifyText(1, "command."));

            GConsole.WriteLine(-1.0f, "{0} {1} {2}",
                GConsole.ColorifyText(1, "To find out more about mission targets - society members - you can use"),
                GConsole.ColorifyText(0, "society"),
                GConsole.ColorifyText(1, "command."));

            GConsole.WriteLine(-1.0f,
    GConsole.ColorifyText(1, "Note a society member can only be target of one mission."));

            GConsole.WriteLine(" ");
        }
    }
}
