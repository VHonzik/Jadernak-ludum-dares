using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Events
{

    public class EventCost {
        private ResourceType[] resources;
        private int[] values;

        public EventCost(ResourceType res, int value)
        {
            resources = new ResourceType[] { res };
            values = new int[] { value };
        }

        public EventCost()
        {
            resources = new ResourceType[] { };
            values = new int[] { };
        }

        public EventCost(ResourceType[] res, int[] vals)
        {
            resources = res;
            values = vals;
        }

        public string Text()
        {
            string result = "";

            if (CanAfford())
            {

                if (resources.Length > 0)
                {
                    result += "(";
                }

                for (int i = 0; i < resources.Length; i++)
                {
                    result += "-" + values[i] + " " + ResourcesManager.names[(int)resources[i]];
                    if (resources.Length > 1 && i < resources.Length - 1)
                    {
                        result += ",";
                    }
                }

                if (resources.Length > 0)
                {
                    result += ")";
                }
            }
            else
            {
                result += "(not enough ";
                bool foundone = false;
                for (int i = 0; i < resources.Length; i++)
                {
                    if (values[i] > ResourcesManager.Instance.GetResource(resources[i]))
                    {
                        if(foundone)
                        {
                            result += ", ";
                        }
                        result += ResourcesManager.names[(int)resources[i]];
                        foundone = true;
                    }
                }
                result += ")";
            }

            return result;
        }

        public void Do()
        {
            for (int i = 0; i < resources.Length; i++)
            {
                ResourcesManager.Instance.RemoveResource(resources[i], values[i]);
            }
        }

        public bool CanAfford()
        {
            for (int i = 0; i < resources.Length; i++)
            {
                if (values[i] > ResourcesManager.Instance.GetResource(resources[i])) return false;
            }

            return true;
        }
    }
    public class RewardEvent
    {
        private EventCost cost;
        private RandomReward[] randomRewards = null;
        private string textKey;

        public RewardEvent(RandomReward[] rewards, string txtKey)
        {
            cost = new EventCost(ResourceType.Supplies, 1);
            randomRewards = rewards;
            textKey = txtKey;
        }

        public RewardEvent(RandomReward[] rewards, string txtKey, EventCost c)
        {
            cost = c;
            randomRewards = rewards;
            textKey = txtKey;
        }

        public string GetText()
        {
            return StringTable.GetRandomText(textKey) + cost.Text();
        }

        public bool Affordable()
        {
            return cost.CanAfford();
        }

        public bool HasAnyRewards()
        {
            return randomRewards.Length > 0;
        }

        public List<string> Do()
        {
            cost.Do();

            List<string> result = new List<string>();

            foreach(var reward in randomRewards)
            {
                result.Add(reward.Give());
            }

            return result;
        }


    }

}

