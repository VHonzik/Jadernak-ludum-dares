using UnityEngine;
using System.Collections;

namespace Events
{
    public enum ResourceType
    {
        Supplies = 0,
        Food,
        Money,
        Diplomacy,
        Strenght,
        Tracking

    }

    public class ResourcesManager : MonoBehaviour
    {
        private int[] resources = new int[] { 6, 4, 2};
        public static string[] names = new string[] {"supplies", "food", "money"};

        private static ResourcesManager _instance;
        public static ResourcesManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GameObject.FindObjectOfType<ResourcesManager>();
                }
                return _instance;
            }
        }

        void Awake()
        {
            _instance = this;
        }

        public void AddResource(ResourceType res, int amount)
        {
            resources[(int)res] += amount;
        }

        public bool RemoveResource(ResourceType res, int amount)
        {
            if (resources[(int)res] < amount) return false;
            resources[(int)res] -= amount;
            return true;
        }

        public int GetResource(ResourceType res)
        {
            return resources[(int)res];
        }

        public string CurrentState()
        {
            string result = "You have ";
            for(int i=0; i < 3; ++i )
            {
                result += resources[i] + " " + names[i];
                if(i < resources.Length-1)
                {
                    result += ", ";
                }
            }

            result += ".";
            return result;
        }

    }

}
