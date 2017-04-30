using System;
using Gash;

namespace Game
{
    class Program
    {
        static void Main(string[] args)
        {
                GameManager.Instance.Setup();
                GameManager.Instance.Intro();
                GameManager.Instance.StartTurn();
                GConsole.Start();
        }
    }
}