using Gash;
using GashLibrary.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game
{
    class Trait : IKeyword
    {
        public enum TraitType
        {
            // Bad ones
            Clumsy, Asocial, Ugly, Frightful, Selfish, Restless, Delicate, Gambler, Lame, Shortsighted, Stinks, Slowlearner,
            // Good ones
            Brute, Pretty, Exconvict, Inconspicuous, Painless
        }

        private string[] Names = { "clumsy", "asocial", "ugly", "frightful", "selfish", "restless",
        "delicate", "gambler", "lame", "shortsighted", "stinks", "slowlearner", "brute", "pretty", "exconvict",
            "inconspicuous", "paintolerant" };

        private float[] Probabilities = { 0.4f, 0.3f, 0.65f, 0.25f, 0.7f, 0.4f,
        0.2f, 0.6f, 0.3f, 0.2f, 0.25f, 0.3f, 0.1f, 0.15f, 0.1f,
            0.1f, 0.1f };

        public string Name => Names[(int)Type];

        public string ColoredName => GConsole.ColorifyText(0,Name);

        public void PrintManPage()
        {
            switch (Type)
            {
                case TraitType.Asocial:
                   GConsole.WriteLine("{0}, hates most poeple and dealing with them.", ColoredName);
                    break;
                case TraitType.Brute:
                    GConsole.WriteLine("{0}, mean son of a bitch, using violence as a universal tool.", ColoredName);
                    break;
                case TraitType.Clumsy:
                    GConsole.WriteLine("{0}, struggles with anything requiring any sort of coordination.", ColoredName);
                    break;
                case TraitType.Delicate:
                    GConsole.WriteLine("{0}, not very well built for physical efforts.", ColoredName);
                    break;
                case TraitType.Exconvict:
                    GConsole.WriteLine("{0}, seen and done terrible deeds in past. Likely to repeat them.", ColoredName);
                    break;
                case TraitType.Frightful:
                    GConsole.WriteLine("{0}, easily scared, lacking guts.", ColoredName);
                    break;
                case TraitType.Gambler:
                    GConsole.WriteLine("{0}, tends to be distracted and loose money in doing so.", ColoredName);
                    break;
                case TraitType.Inconspicuous:
                    GConsole.WriteLine("{0}, easily overlooked and quickly forgotten by other people.", ColoredName);
                    break;
                case TraitType.Lame:
                    GConsole.WriteLine("{0}, has trouble walking and generally arouses pity.", ColoredName);
                    break;
                case TraitType.Painless:
                    GConsole.WriteLine("{0}, has a strong tolerance for pain.", ColoredName);
                    break;
                case TraitType.Pretty:
                    GConsole.WriteLine("{0}, has a pretty likeable face.", ColoredName);
                    break;
                case TraitType.Selfish:
                    GConsole.WriteLine("{0}, puts self-being in front of everything else.", ColoredName);
                    break;
                case TraitType.Restless:
                    GConsole.WriteLine("{0}, does stupid shit when bored.", ColoredName);
                    break;
                case TraitType.Shortsighted:
                    GConsole.WriteLine("{0}, can hardly see.", ColoredName);
                    break;
                case TraitType.Stinks:
                    GConsole.WriteLine("{0}, emits foul odor.", ColoredName);
                    break;
                case TraitType.Ugly:
                    GConsole.WriteLine("{0}, not pleasent to look at.", ColoredName);
                    break;
                case TraitType.Slowlearner:
                    GConsole.WriteLine("{0}, has hard time picking up any skills.", ColoredName);
                    break;
                default:
                    GConsole.WriteLine("Unknown trait. Ups...");
                    break;
            }

            GConsole.WriteLine(" ");
        }

        public TraitType Type;

        public Trait()
        {
            bool found = false;
            while(found == false)
            {
                int traitIndex = GameManager.Instance.RNG.Next(Names.Length);
                double diceRoll = GameManager.Instance.RNG.NextDouble();
                if(diceRoll <= Probabilities[traitIndex])
                {
                    found = true;
                    Type = (TraitType)traitIndex;
                }
            }
        }

        public float[,] SkillMatrix =
        {
// Hustle
//  Clumsy  Asocial Ugly    Frightful   Selfish Restless    Delicate    Gambler Lame    Shortsighted    Stinks  Slowlearner Brute   Pretty  Exconvict   Inconspicuous   Painless
{   0.0f,   0.0f,   0.0f,   0.0f,       0.0f,   0.0f,       0.0f,       0.05f,  0.1f,   0.1f,           -0.05f, 0.0f,       0.15f,   0.1f,  -0.1f,      0.3f,           0.0f},

// Steal
//  Clumsy  Asocial Ugly    Frightful   Selfish Restless    Delicate    Gambler Lame    Shortsighted    Stinks  Slowlearner Brute   Pretty  Exconvict   Inconspicuous   Painless
{   -0.4f,  0.0f,   0.0f,   -0.1f,      0.05f,  0.0f,       0.0f,       0.05f,  -0.1f,  -0.3f,          -0.4f,  0.0f,       -0.3f,  0.1f,  -0.1f,       0.15f,           0.0f},

// Eavesdrop 
//  Clumsy  Asocial Ugly    Frightful   Selfish Restless    Delicate    Gambler Lame    Shortsighted    Stinks  Slowlearner Brute   Pretty  Exconvict   Inconspicuous   Painless
{   -0.3f,  -0.1f,  0.0f,   0.0f,       -0.1f,  -0.2f,      0.0f,       0.0f,   0.0f,   0.0f,           -0.4f,  0.0f,       -0.4f,  -0.1f,  0.0f,       0.15f,           0.0f},

// Infiltrate
//  Clumsy  Asocial Ugly    Frightful   Selfish Restless    Delicate    Gambler Lame    Shortsighted    Stinks  Slowlearner Brute   Pretty  Exconvict   Inconspicuous   Painless
{   -0.2f,  -0.4f,  -0.1f,  0.0f,       -0.2f,  0.0f,       0.0f,       -0.1f,  -0.1f,  0.0f,           -0.4f,  -0.2f,      -0.4f,  0.15f,   0.0f,       -0.05f,          0.0f},

// Blackmail
//  Clumsy  Asocial Ugly    Frightful   Selfish Restless    Delicate    Gambler Lame    Shortsighted    Stinks  Slowlearner Brute   Pretty  Exconvict   Inconspicuous   Painless
{   0.0f,   +0.05f,  +0.1f,  -0.4f,     +0.1f,  0.0f,       -0.2f,      0.0f,   -0.4f,  -0.05f,         +0.05f,  0.0f,      +0.1f,  0.0f,   +0.1f,      -0.4f,          0.0f},

// Beatup
//  Clumsy  Asocial Ugly    Frightful   Selfish Restless    Delicate    Gambler Lame    Shortsighted    Stinks  Slowlearner Brute   Pretty  Exconvict   Inconspicuous   Painless
{   -0.1f,  +0.25f,  0.0f,   -0.3f,      0.0f,   0.0f,       -0.4f,      0.0f,   -0.4f,  0.0f,           +0.0f,  0.0f,       +0.15f,  0.0f,   +0.1f,      0.0f,           0.15f},

//Kill
//  Clumsy  Asocial Ugly    Frightful   Selfish Restless    Delicate    Gambler Lame    Shortsighted    Stinks  Slowlearner Brute   Pretty  Exconvict   Inconspicuous   Painless
{   -0.4f,  +0.05f, 0.0f,   -0.5f,      0.05f,  0.0f,       -0.5f,      0.0f,   -0.3f,  0.0f,           0.0f,   0.0f,       +0.1f,  0.0f,   +0.15f,      0.0f,           0.1f},
        };

        public void ModifySkillValue(Action.ActionType skill, ref float value)
        {
            value += SkillMatrix[(int)skill, (int)Type];
        }
    }
}
