﻿using Gash;
using Gash.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Commands
{
    class EndDay : ICommand
    {
        List<BoolFlag> Flags = new List<BoolFlag>();

        public AutoCompletionResult AutoComplete(string line)
        {
            return ParsingHelpers.AutoCompleteCommandBody(line, this);
        }

        public bool Available()
        {
            return true;
        }

        public IEnumerable<BoolFlag> GetFlags()
        {
            return Flags;
        }

        public string Name()
        {
            return "endday";
        }

        public ParsingResult Parse(string line)
        {
            var result = ParsingHelpers.ParseSimpleCommand(line, this);

            if (result.Type == ParsingResultType.Success)
            {
                GameManager.Instance.EndTurn();
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
            GConsole.WriteLine(-1.0f, "\t{0}",
                GConsole.ColorifyText(0, Name()));
            GConsole.WriteLine(-1.0f, GConsole.ColorifyText(1, Resources.text.ManHeaderDescription));
            GConsole.WriteLine(-1.0f,
                GConsole.ColorifyText(1, "Ends the current day. Missions are evaluated at the start of the next day."));
        }
    }
}
