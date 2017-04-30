using Gash;
using System;
using System.Collections.Generic;
using System.Text;

namespace GashLibrary.Commands
{
    public class Keyword : IKeyword
    {
        public static Keyword CreateSimpleFormatted(string name, string manPage)
        {
            Keyword result = new Keyword();
            result.Highlight = GConsole.Settings.Higlights[0];
            result.RawName = name;
            result.ManFormat = manPage;
            return result;
        }

        private HighlightType Highlight;

        private string ManFormat = "";

        private string RawName = "";
        public string Name
        {
            get => RawName;
        }

        public string ColoredName
        {
            get => GConsole.ColorifyText(Highlight, RawName);
        }

        public void PrintManPage()
        {
            GConsole.WriteLine(-1.0f, ManFormat, RawName, ColoredName);
            GConsole.WriteLine(" ");
        }
    }
}
