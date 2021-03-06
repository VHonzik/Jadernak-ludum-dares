﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Gash
{
    public class HighlightType
    {
        public ConsoleColor Foreground;
        public ConsoleColor Background;

        public HighlightType()
        {
            Foreground = ConsoleColor.Gray;
            Foreground = ConsoleColor.Black;
        }
    }
    /// <summary>
    /// Settings class for Gash framework.
    /// </summary>
    public class Settings
    {
        /// <summary>
        /// Default speed of typing in seconds per character.
        /// </summary>
        public float TypingSpeed = 1.0f / 40.0f;

        /// <summary>
        /// Number of frames per second the game loop runs.
        /// </summary>
        public float FPS = 60.0f;

        /// <summary>
        /// When two exactly same lines follow each other, instead of printing it again,
        /// up to three dots are appended to the previous lines when this is true.
        /// Used to prevent message spam.
        /// The dots cycle 1 - 2 - 3 - 0 - 1 - 2 - 3 etc.
        /// </summary>
        public bool SameLinesProduceDots = false;

        /// <summary>
        /// Default foreground color for highliting commands and keywords in text.
        /// </summary>
        public List<HighlightType> Higlights = new List<HighlightType>()
        {
            new HighlightType() { Foreground=ConsoleColor.Cyan, Background=ConsoleColor.Black },
            new HighlightType() { Foreground=ConsoleColor.DarkGray, Background=ConsoleColor.Black }
        };
    }
}
