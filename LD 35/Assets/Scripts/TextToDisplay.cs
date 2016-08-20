using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UI

{
    public static class MyExtensions
    {
        public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }
    };

    public enum UIState
    {
        Off,
        Revealing,
        On,
        Hiding
    }

    public class TextToDisplay
    {
        public string text;
        public List<Events.RewardEvent> choices;
        public bool isStory;
        public DoTask task;

        public delegate void DoTask();

        public TextToDisplay(string txt, List<Events.RewardEvent> chcs, bool story = false)
        {
            choices = chcs;
            text = txt;
            isStory = story;
        }

        public TextToDisplay(DoTask tsk)
        {
            task = tsk;
            isStory = false;
        }


    };
}
