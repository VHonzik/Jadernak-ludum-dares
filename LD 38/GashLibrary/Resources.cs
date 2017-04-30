using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gash
{
    public class Resources
    {
        public class text
        {
            public static string UnknownCommand = "Unknown command {0}.";
            public static string UnknownCommandOrKeywordForMan = "Unrecognized command or keyword {0}, cannot display manual page.";
            public static string MissingParam
            {
                get => String.Format("{0} {1}{2}",
                    GConsole.ColorifyText(1,"Missing required parameter(s) for a command. See"),
                    GConsole.ColorifyText(0,"man {0}"),
                    GConsole.ColorifyText(1,"."));
            }

            public static string FailureParsingRequiredParam
            {
                get => String.Format("{0} {1}{2}",
                    GConsole.ColorifyText(1,"Failure parsing required parameter. See"),
                    GConsole.ColorifyText(1,"man {0}"),
                    GConsole.ColorifyText(1,"."));
            }

            public static string FailureParsingFlags {
                get => String.Format("{0} {1}{2}",
                    GConsole.ColorifyText(1,"Failure parsing flags \"{0}\". See"),
                    GConsole.ColorifyText(1,"man {1}"),
                    GConsole.ColorifyText(1,"."));
            }

            public static string UnknownFlags
            {
                get => String.Format("{0} {1}{2}",
                    GConsole.ColorifyText(1,"Unknown flags \"{0}\". See"),
                    GConsole.ColorifyText(1,"man {1}"),
                    GConsole.ColorifyText(1,"."));
            }
            public static string ManCommandName = "man";

            public static string ManHeaderIntro = "Manual page for command";
            public static string ManHeaderName = "NAME";
            public static string ManHeaderSynopsis = "SYNOPSIS";
            public static string ManHeaderDescription = "DESCRIPTION";
            public static string ManMan
            {
                get => String.Format(@"{0}
{1} {2} {3}
{4}",
                    GConsole.ColorifyText(1, "Display a manual page for a command or a game mechanic."),
                    GConsole.ColorifyText(1, "Any text in"),
                    String.Join(", ", 
                        from h in GConsole.Settings.Higlights
                        where h != GConsole.Settings.Higlights[1]
                        select String.Format("{0}", GConsole.ColorifyText(h, h.Foreground.ToString().ToLower()))),
                    GConsole.ColorifyText(1,"colors can be passed as a parameter."),
                    GConsole.ColorifyText(1,"The parameter accepts spaces and therefore it can be multiple words."));
            }

            public static string ListCommandName = "list";
            public static string ListMan
            {
                get => "Display list of currently available commands.";
            }


        }
    }
}
