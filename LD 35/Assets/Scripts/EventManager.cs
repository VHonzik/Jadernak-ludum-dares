using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Events
{
    public class EventManager : MonoBehaviour
    {

        private static EventManager _instance;
        public static EventManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GameObject.FindObjectOfType<EventManager>();
                }
                return _instance;
            }
        }

        void Awake()
        {
            _instance = this;         
        }

        public void EnteredNode(NodeGraph node)
        {

            List<RewardEvent> events = EventCollection.GetEvents(node);
            node.Visited = true;
            events.Add(new RewardEvent(new RandomReward[] { }, "SKIP_EVENT", new EventCost(ResourceType.Food, 1)));

            bool canAffordOne = false;
            foreach (var rewevent in events)
            {
                if (rewevent.Affordable()) { canAffordOne = true; break; }
            }
            if(canAffordOne)
            {
                UI.MapRenderer.Instance.QueueText(new UI.TextToDisplay(StringTable.GetRandomText(node.TextKey), events));
            }
            else
            {
                StoryText("END_GAME", "Exit");
                UI.MapRenderer.Instance.QueueText(new UI.TextToDisplay(() => Application.Quit()));
            }
            
        }

        public void StoryText(string text, string continueText)
        {
            List<RewardEvent> events = new List<RewardEvent>();
            events.Add(new RewardEvent(new RandomReward[] { }, continueText, new EventCost()));
            UI.MapRenderer.Instance.QueueText(new UI.TextToDisplay(StringTable.GetRandomText(text), events, true));
        }

        public void ResultOfEvent(RewardEvent result)
        {
            string result_text = String.Join("\n", result.Do().ToArray());
            if (result_text.Length > 0)
            {
                List<RewardEvent> events = new List<RewardEvent>();
                EventCost cost = result.HasAnyRewards() ? new EventCost(ResourceType.Food, 1) : new EventCost();
                if(cost.CanAfford())
                {
                    events.Add(new RewardEvent(new RandomReward[] { }, "Continue", cost));
                    UI.MapRenderer.Instance.QueueResultText(
                        new UI.TextToDisplay(result_text, events));
                }
                else
                {
                    StoryText(result_text, "Continue");
                    StoryText("END_GAME", "Exit");
                    UI.MapRenderer.Instance.QueueText(new UI.TextToDisplay(() => Application.Quit()));
                }

            }
            UI.MapRenderer.Instance.DeleteCurrentText();
        }
    }
}
