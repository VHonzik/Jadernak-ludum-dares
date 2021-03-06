﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml.Linq;

namespace Gash.Output
{
    internal class OutputManager : IGameLooped
    {
        private Queue<OutputLine> LineQueue = new Queue<OutputLine>();
        private OutputLine CurrentLine;
        private OutputLine PreviousLine;
        private float CurrentLineTimer;

        private bool Paused = false;

        private ConsoleAccess ConsoleAccess { get; set; }

        public int[] ConsolePosition
        {
            get
            {
                if (ConsoleAccess != null) return ConsoleAccess.ConsolePosition;
                return new int[2] { 0, 0 };
            }
        }

        public void WriteLine(string line)
        {
            if(Paused == false)
                LineQueue.Enqueue(new OutputLine() { Line = line, Speed = GConsole.Settings.TypingSpeed });
        }

        public void WriteLine(string line, float speed)
        {
            if (Paused == false)
                LineQueue.Enqueue(new OutputLine() { Line = line, Speed = speed });
        }

        public void Wait(float time)
        {
            if (Paused == false)
                LineQueue.Enqueue(new OutputLine() { Line = "\0", Speed = time });
        }

        private void LineQueuePop()
        {
            CurrentLine = LineQueue.Dequeue();
            CurrentLine.LineIndex = 0;
            CurrentLine.RealCharLineIndex = 0;
            CurrentLineTimer = 0.0f;
        }

        private void LineQueueProcessColor()
        {
            ConsoleColor foregroundColor = (ConsoleColor)Convert.ToInt32(CurrentLine.Line.Substring(CurrentLine.LineIndex + 1, 2));
            ConsoleColor backgroundColor = (ConsoleColor)Convert.ToInt32(CurrentLine.Line.Substring(CurrentLine.LineIndex + 3, 2));
            ConsoleAccess.ForegroundColor = foregroundColor;
            ConsoleAccess.BackgroundColor = backgroundColor;
            CurrentLine.SpecialColor = true;
            CurrentLine.LineIndex += 5;

            CurrentLine.LastCharForeground = foregroundColor;
            CurrentLine.LastCharBackground = backgroundColor;
        }

        internal void Start()
        {
            ConsoleAccess = GConsole.Instance.RequestAccess();
        }

        private void LineQueueRestoreColor()
        {
            CurrentLine.SpecialColor = false;
            ConsoleAccess.ResetColor();
            CurrentLine.LineIndex++;

            if (CurrentLine.LineIndex < CurrentLine.Line.Length)
            {
                CurrentLine.LastCharForeground = ConsoleAccess.ForegroundColor;
                CurrentLine.LastCharBackground = ConsoleAccess.BackgroundColor;
            }
        }

        private void LineQueueEndOfSameLine()
        {
            CurrentLine.SpecialColor = false;
            ConsoleAccess.ResetColor();
            PreviousLine = CurrentLine;
            CurrentLine = null;
        }

        private void LineQueueEndOfLine()
        {
            LineQueueEndOfSameLine();
            ConsoleAccess.Write("\n");
        }

        private void TimedLine()
        {
            ConsoleAccess.Lock();
            if (CurrentLine.ConsolePos == null) CurrentLine.ConsolePos = new int[2] { Console.CursorLeft, Console.CursorTop };
            if (CurrentLineTimer >= CurrentLine.Speed)
            {
                if (CurrentLine.Line[CurrentLine.LineIndex] != '$')
                {
                    ConsoleAccess.Write(CurrentLine.Line[CurrentLine.LineIndex]);
                    CurrentLine.LineIndex++;
                    CurrentLine.RealCharLineIndex++;
                    CurrentLineTimer -= CurrentLine.Speed;
                }
                else if (CurrentLine.SpecialColor == false)
                {
                    LineQueueProcessColor();
                }
                else
                {
                    LineQueueRestoreColor();
                }
            }
            ConsoleAccess.Unlock();
        }

        private void FinishedTyping()
        {
            GConsole.Instance.Input.Buffer.PostprocessApply();
        }

        private void InstantLine()
        {
            ConsoleAccess.Lock();
            CurrentLine.ConsolePos = new int[2] { Console.CursorLeft, Console.CursorTop };
            while (CurrentLine.LineIndex < CurrentLine.Line.Length)
            {
                if (CurrentLine.Line[CurrentLine.LineIndex] != '$')
                {
                    Console.Write(CurrentLine.Line[CurrentLine.LineIndex]);
                    CurrentLine.LineIndex++;
                    CurrentLine.RealCharLineIndex++;
                }
                else if (CurrentLine.SpecialColor == false)
                {
                    LineQueueProcessColor();
                }
                else
                {
                    LineQueueRestoreColor();
                }
            }

            LineQueueEndOfLine();
            ConsoleAccess.Unlock();

            if (LineQueue.Count > 0)
            {
                LineQueuePop();
                if (CurrentLine.Speed <= 0.0f)
                {
                    InstantLine();
                }
            }
            else
            {
                FinishedTyping();
            }
        }

        private void SameLineDot()
        {
            ConsoleAccess.Lock();
            var OrigConsolePos = new int[2] { Console.CursorLeft, Console.CursorTop };

            ConsoleAccess.ForegroundColor = PreviousLine.LastCharForeground;
            ConsoleAccess.BackgroundColor = PreviousLine.LastCharBackground;

            if (PreviousLine.DotCount < 3)
            {
                ConsoleAccess.SetCursorPosition(
                    PreviousLine.ConsolePos[0] + PreviousLine.RealCharLineIndex + PreviousLine.DotCount,
                    PreviousLine.ConsolePos[1]);
                PreviousLine.DotCount++;
                ConsoleAccess.Write(".");
            }
            else
            {
                ConsoleAccess.SetCursorPosition(
                    PreviousLine.ConsolePos[0] + PreviousLine.RealCharLineIndex,
                    PreviousLine.ConsolePos[1]);
                ConsoleAccess.Write(".  ");
                PreviousLine.DotCount = 1;
            }

            ConsoleAccess.ResetColor();

            ConsoleAccess.ConsolePosition = OrigConsolePos;
            CurrentLine = PreviousLine;
            LineQueueEndOfSameLine();
            ConsoleAccess.Unlock();

            FinishedTyping();
        }

        public void Update(float deltaTime)
        {
            if (CurrentLine == null && LineQueue.Count > 0)
            {
                LineQueuePop();

                GConsole.Instance.Input.ClearCurrentLine();

                if (PreviousLine != null && PreviousLine.Line == CurrentLine.Line &&
                    GConsole.Settings.SameLinesProduceDots == true)
                {
                    SameLineDot();
                }
            }
            else if (CurrentLine != null && CurrentLine.LineIndex < CurrentLine.Line.Length)
            {
                if(CurrentLine.ConsolePos != null && CurrentLine.ConsolePos[1] == Console.WindowTop + Console.WindowHeight - 2)
                {
                    Console.SetWindowPosition(Console.WindowLeft, Console.WindowTop + 1);
                }

                if (CurrentLine.Speed <= 0.0f)
                {
                    InstantLine();
                }
                else
                {
                    CurrentLineTimer += deltaTime;
                    TimedLine();
                }
            }
            else if (CurrentLine != null && CurrentLine.LineIndex >= CurrentLine.Line.Length)
            {
                ConsoleAccess.Lock();
                LineQueueEndOfLine();
                ConsoleAccess.Unlock();
                FinishedTyping();
            }

        }

        public void ClearQueuedOutput()
        {
            LineQueue.Clear();
        }

        public void PauseOutput()
        {
            Paused = true;
        }
    }
}

    
