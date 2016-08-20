using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Events
{
    public struct RandomValue
    {
        public double probablity;
        public int value;

        public RandomValue(double p, int v )
        {
            probablity = p;
            value = v;
        }
    };

    public class RandomReward
    {
        private ResourceType resource;
        private RandomValue[] randomRewards;
        private string sucessId;
        private string failId;

        private static Random rndGenerator = new Random();

        public RandomReward(ResourceType res, RandomValue[] rewards, string sId, string fId)
        {
            randomRewards = rewards;

            // Compute cumulative
            for(int i=1; i < randomRewards.Length; i++)
            {
                randomRewards[i].probablity += randomRewards[i - 1].probablity;
            }

            Debug.Assert(randomRewards.Last().probablity <= 1.0);

            resource = res;
            sucessId = sId;
            failId = fId;
        }

        // Evaluate, update resources and return result text
        public string Give()
        {
            if(randomRewards.Length > 0)
            {
                int value = 0;
                double dice = rndGenerator.NextDouble();
                for (int i = 0; i < randomRewards.Length; i++)
                {
                    if (dice < randomRewards[i].probablity)
                    {
                        value = randomRewards[i].value;
                        break;
                    }
                }

                if(value > 0)
                {
                    ResourcesManager.Instance.AddResource(resource, value);
                    return StringTable.GetRandomText(sucessId) + " (+" + Convert.ToString(value) +
                        " " + ResourcesManager.names[(int)resource] + ")";
                }
                else
                {
                    ResourcesManager.Instance.RemoveResource(resource, value);
                    return StringTable.GetRandomText(failId) + (value<0 ? " (" + Convert.ToString(value) +
    " " + ResourcesManager.names[(int)resource] + ")" : "");
                }                
            }

                return "";
           
        }

    }
}