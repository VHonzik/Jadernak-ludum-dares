using Gash;
using Gash.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Commands
{
    class Defame : ICommand
    {
        List<BoolFlag> Flags = new List<BoolFlag>() { new BoolFlag("info", false) };

        public AutoCompletionResult AutoComplete(string line)
        {
            var result = ParsingHelpers.AutoCompleteCommandBody(line, this);
            if (result.WasSuccessful == AutoCompletionResultType.FailureAlreadyComplete)
            {
                var secondResult = ParsingHelpers.AutoCompleteStringList(line.Substring(result.RemainderStartPosition-1),
                    GameManager.Instance.Society.GetNames(), true);

                if (secondResult.WasSuccessful == AutoCompletionResultType.SuccessOneOption)
                {
                    var finalResult = new AutoCompletionResult();
                    finalResult.WasSuccessful = AutoCompletionResultType.SuccessOneOption;
                    finalResult.Results.Add(line.Substring(0, result.RemainderStartPosition - 1) + " " + secondResult.Results[0]);
                    return finalResult;

                }

                return secondResult;
            }

            return result;
        }

        public bool Available()
        {
            return GameManager.Instance.Assets.Slander > 0;
        }

        public IEnumerable<BoolFlag> GetFlags()
        {
            return Flags;
        }

        public string Name()
        {
            return "defame";
        }

        public ParsingResult Parse(string line)
        {
            var result = ParsingHelpers.ParseCommand(line, this, true);

            if (result.Type == ParsingResultType.Success)
            {
                string lordname = String.Join(" ", result.Parameters);
                var lord = GameManager.Instance.Society.Lords.Find(m => m.Name == lordname);

                if (lord == null)
                {
                    GConsole.WriteLine("{0} {1}",
                        GConsole.ColorifyText(1, "Unknown society member. See"),
                        GConsole.ColorifyText(0, "man defame"));
                    result.Type = ParsingResultType.ParsingFailure;
                    return result;
                }

                if(lord.Alive == false)
                {
                    GConsole.WriteLine("{0} is dead, let him rest in pease.", lord.ColoredName);
                    return result;
                }

                if (result.Flags.Count > 0)
                {
                    lord.PrintEstimateDefamePrice();                    
                }
                else
                {
                    float price = lord.DefamePrice();
                    if(price <= GameManager.Instance.Assets.Slander)
                    {
                        GConsole.WriteLine("You have succesfully damaged {0}'s reputation", lord.ColoredName);
                        GConsole.WriteLine("It cost you {0} {1}.", Math.Ceiling(price).ToString("N0"), Assets.KSlander.ColoredName);
                        GameManager.Instance.Assets.Slander -= price;
                        GameManager.Instance.Society.Defame(lord);
                    }
                    else
                    {
                        GConsole.WriteLine("An attempt to defame {0} failed due not insufficient slander.", lord.ColoredName);
                        GConsole.WriteLine("All you slander is gone.");
                        GameManager.Instance.Assets.Slander = 0;
                    }
                }

                GConsole.WriteLine(-1.0f, " ");
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
            GConsole.WriteLine(-1.0f, "\t{0} {1} -info",
                GConsole.ColorifyText(0, Name()),
                GConsole.ColorifyText(ConsoleColor.Black, GConsole.Settings.Higlights[2].Foreground, "(society member)"));
            GConsole.WriteLine(-1.0f, "\t{0} {1}",
                GConsole.ColorifyText(0, Name()),
                GConsole.ColorifyText(ConsoleColor.Black, GConsole.Settings.Higlights[2].Foreground, "(society member)"));

            GConsole.WriteLine(-1.0f, GConsole.ColorifyText(1, Resources.text.ManHeaderDescription));

            GConsole.WriteLine(-1.0f,
                GConsole.ColorifyText(1, "Use collected slander to damage the good reputation of a society member."));
            GConsole.WriteLine(-1.0f,
                GConsole.ColorifyText(1, "Decrease resistances of the society member to various missions."));
            GConsole.WriteLine(-1.0f,
                GConsole.ColorifyText(1, "When -info flag is used the command only estimate the amount of slander needed."));
            GConsole.WriteLine(-1.0f,
                GConsole.ColorifyText(1, "Note that the higher post the target holds the more expensive defaming is."));
            GConsole.WriteLine(-1.0f,
                GConsole.ColorifyText(1, "On the other hand, defaming subordinates hurts the direct leaders as well."));
        }
    }
}
