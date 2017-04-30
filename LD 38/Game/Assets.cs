using Gash;
using GashLibrary.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game
{
    class Assets
    {
        public float Money = 200;
        public float Respect = 0;
        public float Slander = 0;

        public static Keyword KPounds = Keyword.CreateSimpleFormatted("pounds", 
            "{1} are The Currency of the modern world. Required to run your operations and hire crew members.");

        public static Keyword KRespect = Keyword.CreateSimpleFormatted("respect",
            "Men of {1} have great influence on the opinions of lesser men. Increases chances of all your operations.");

        public static Keyword KSlander = Keyword.CreateSimpleFormatted("slander",
            "You amongst all know very well the power of words. Can be spent to weaken society members.");

        public void PrintAssets()
        {
            GConsole.WriteLine(-1.0f, "You have {0} {1}, {2} {3} and {4} {5}.",
                Convert.ToInt32(Money),
                KPounds.ColoredName,
                Convert.ToInt32(Respect),
                KRespect.ColoredName,
                Convert.ToInt32(Slander),
                KSlander.ColoredName);
        }
    }
}
