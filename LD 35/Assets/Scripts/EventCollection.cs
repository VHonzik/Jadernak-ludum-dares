using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Events
{
    public enum BiomeType
    {
        Desert = 0,
        Forest,
        Grassland,
        Hills,
        Lake,
        Mountain,
        Swamp,
        Town,
        Village,
    };

    class EventCollection
    {
        static System.Random rnd = new System.Random();
        static int story_phase = 0;
        private static readonly RewardEvent[][] generic_random_biome_events = new RewardEvent[][] { 

            //Desert
            new RewardEvent[] {
                new RewardEvent( new RandomReward[] {
                    new RandomReward(ResourceType.Money, new RandomValue[] {
                            new RandomValue(0.3, 1), new RandomValue(0.1, 2), new RandomValue(0.6, 0) // EV 0.5
                        }, "DESERT_MONEY_REWARD", "GENERIC_SCAVANGE_FAIL" ),

                }, "GENERIC_SCAVANGE_EVENT" )
            },
            //Forest
            new RewardEvent[] {
                new RewardEvent( new RandomReward[] {
                    new RandomReward(ResourceType.Food, new RandomValue[] {
                            new RandomValue(0.4, 0), new RandomValue(0.2, 2), new RandomValue(0.2, 3), new RandomValue(0.2, 5) // EV 2
                        }, "GENERIC_HUNT_FOOD_REWARD", "GENERIC_HUNT_FOOD_FAIL" ),

                }, "GENERIC_HUNT_EVENT" )
            },
            //Grassland
            new RewardEvent[] {
                new RewardEvent( new RandomReward[] {
                    new RandomReward(ResourceType.Food, new RandomValue[] {
                            new RandomValue(0.5, 0), new RandomValue(0.5, 3) // EV 1.5
                        }, "GENERIC_COLLECT_FOOD_REWARD", "GENERIC_COLLECT_FOOD_FAIL" ),

                }, "GENERIC_COLLECT_FOOD_EVENT" )
            },
            //Hills
            new RewardEvent[] {
                 new RewardEvent( new RandomReward[] {
                    new RandomReward(ResourceType.Food, new RandomValue[] {
                            new RandomValue(0.3, 0), new RandomValue(0.3, 1), new RandomValue(0.2, 2), new RandomValue(0.2, 3), // EV 1.3
                        }, "GENERIC_HUNT_FOOD_REWARD", "GENERIC_HUNT_FOOD_FAIL" ),

                }, "GENERIC_HUNT_EVENT" ),
                new RewardEvent( new RandomReward[] {
                    new RandomReward(ResourceType.Supplies, new RandomValue[] {
                            new RandomValue(0.4, 0), new RandomValue(0.2, 1), new RandomValue(0.4, 3) // EV 1.4
                        }, "GENERIC_SCAVANGE_SUPPLIES_REWARD", "GENERIC_SCAVANGE_FAIL" ),

                }, "GENERIC_SCAVANGE_EVENT" )
            },
            //Lake
            new RewardEvent[] {
                new RewardEvent( new RandomReward[] {
                    new RandomReward(ResourceType.Food, new RandomValue[] {
                            new RandomValue(0.4, 0), new RandomValue(0.1, 1), new RandomValue(0.3, 3), new RandomValue(0.2, 4), // EV 1.8
                        }, "GENERIC_FISHING_REWARD", "GENERIC_FISHING_FAIL" ),

                }, "GENERIC_FISHING_EVENT" )
            },
            //Mountain
            new RewardEvent[] {
                new RewardEvent( new RandomReward[] {
                    new RandomReward(ResourceType.Money, new RandomValue[] {
                            new RandomValue(0.5, 0), new RandomValue(0.3, 1), new RandomValue(0.1, 2), new RandomValue(0.1, 3),
                        }, "MOUNTAINS_GOLD_REWARD", "MOUNTAINS_EXPLORE_FAIL" ),
                    new RandomReward(ResourceType.Supplies, new RandomValue[] {
                            new RandomValue(0.4, 0), new RandomValue(0.2, 1), new RandomValue(0.4, 2) // EV 1
                        }, "MOUNTAINS_SUPPLIES_REWARD", "MOUNTAINS_EXPLORE_FAIL" ),
                }, "MOUNTAINS_EXPLORE_EVENT" )
            },
            //Swamp
            new RewardEvent[] {
                new RewardEvent( new RandomReward[] {
                    new RandomReward(ResourceType.Supplies, new RandomValue[] {
                            new RandomValue(0.2, 0), new RandomValue(0.5, 1), new RandomValue(0.2, 2), new RandomValue(0.1, 3),  // EV 1.2
                        }, "GENERIC_SCAVANGE_SUPPLIES_REWARD", "GENERIC_SCAVANGE_FAIL" ),

                }, "GENERIC_SCAVANGE_EVENT" )
            },
            //Town
            new RewardEvent[] { },
            //Village
            new RewardEvent[] { }
        };

        private static readonly RewardEvent[][] guaranteed_biome_events = new RewardEvent[][] {

            //Desert
            new RewardEvent[] { },
            //Forest
            new RewardEvent[] { },
            //Grassland
            new RewardEvent[] { },
            //Hills
            new RewardEvent[] { },
            //Lake
            new RewardEvent[] { },
            //Mountain
            new RewardEvent[] { },
            //Swamp
            new RewardEvent[] { },
            //Town
            new RewardEvent[] {
                new RewardEvent( new RandomReward[] {
                    new RandomReward(ResourceType.Food, new RandomValue[] { new RandomValue(1, 5) }, "GENERIC_BUY_FOOD_REWARD", ""),
                }, "GENERIC_BUY_FOOD_EVENT", new EventCost(ResourceType.Money, 1) ),
                new RewardEvent( new RandomReward[] {
                    new RandomReward(ResourceType.Supplies, new RandomValue[] { new RandomValue(1, 8) }, "GENERIC_BUY_SUPPLIES_REWARD", ""),
                }, "GENERIC_BUY_SUPPLIES_EVENT", new EventCost(ResourceType.Money, 2) )
            },
            //Village
            new RewardEvent[] {
                new RewardEvent( new RandomReward[] {
                    new RandomReward(ResourceType.Food, new RandomValue[] { new RandomValue(1, 4) }, "GENERIC_BUY_FOOD_REWARD", ""),
                }, "GENERIC_BUY_FOOD_EVENT", new EventCost(ResourceType.Money, 1) )
            },
        };

        private static void StoryEvents(BiomeType biome, int index, bool visited)
        {
            // Farreach first time
            if(index == 11 && !visited && story_phase==0)
            {
                EventManager.Instance.StoryText(@"Upon the arrival you contact the local ministry. In the mids of mumbling and prayers the only useful piece of information is the last contact with the heretics. North of Poet's house is your next destination.", "Continue");
                story_phase = 1;

            }

            // North of Poets house first
            if((index == 50 || index == 68) && story_phase>=1)
            {
                EventManager.Instance.StoryText(@"Locals complain about wolves from Green pouch being more daring lately. Must be a coincidence but it is your only lead right now.", "Continue");
                if(story_phase < 2) story_phase = 2;
            }

            if(index == 66 && story_phase==2)
            {                
                EventManager.Instance.StoryText(@"If you didn't know what to look for you would have never found the campsite. It is few days old.", "Continue");
                EventManager.Instance.StoryText(@"There is a lot of tracks and wet ground from a nearby stream. They must have been preparing for a journey east.", "Continue");
                if (story_phase < 3) story_phase = 3;
            }

            if((index == 52 || index == 53) && story_phase >= 3)
            {
                EventManager.Instance.StoryText(@"You meet a caravan. Its leader points out he met a group of strangle folks two days ago traveling north-east.", "Continue");
                if (story_phase < 4) story_phase = 4;
            }

            if ((index == 38) && story_phase == 4)
            {
                EventManager.Instance.StoryText(@"When you enter the village square a few individuals exchange glances and promptly leave.", "Follow them.");
                EventManager.Instance.StoryText(@"They split in a path fork.", "Follow the closer pair.");
                EventManager.Instance.StoryText(@"They've started to run.", "Follow.");
                EventManager.Instance.StoryText(@"You manage to trap them in a dead end.", "Continue.");
                EventManager.Instance.StoryText(@"The smaller male grips a quarterstaff and assumes defensive stance in front of the other man.", "Fight.");
                EventManager.Instance.StoryText(@"Good reach keeps you at bay for a while but your skills are superior.", "Press harder.");
                EventManager.Instance.StoryText(@"The one behind is sitting still with closed eyes and his lips are silently moving.", "Continue.");
                EventManager.Instance.StoryText(@"You score a cut on your opponent's arm. His grip weakens.", "Use you advantage.");
                EventManager.Instance.StoryText(@"Something is happening behind the heretic, but he is blocking your view.", "Fight on.");
                EventManager.Instance.StoryText(@"You hear a terrible growl and the small male evades to the side.", "Continue.");
                EventManager.Instance.StoryText(@"A bear jumps at you at pins you to the ground. He mauls and throws you feet away.", "Lose conscience.");
                EventManager.Instance.StoryText(@"You wake up in a medic house.", "Continue.");

                story_phase = 5;
            }

            if ((index == 14 || index == 36 || index == 60) && story_phase == 5)
            {
                EventManager.Instance.StoryText(@"The trails runs cold. Best continue south to Bishop's seat and report your encounter.", "Embark south..");
                story_phase = 6;
            }

            if ((index == 15) && story_phase == 6)
            {
                EventManager.Instance.StoryText(@"A messenger catches your when your are about to enter the city. The heretics killed a brother in Sky fondlers mountains to the west.", "Pursue and leave nothing to chance.");
                story_phase = 7;
            }


            if ((index == 16) && story_phase == 7)
            {
                EventManager.Instance.StoryText(@"You enter Last respite village. You quickly recognize the youngster who fought you before.", "Holler at him.");
                EventManager.Instance.StoryText(@"He recognizes you too and sounds an alarm.", "Continue.");
                EventManager.Instance.StoryText(@"Wolves, bears, dogs emerge from the village and charge at you.", "Signal your brothers.");
                EventManager.Instance.StoryText(@"In the brief moment before the battle you think to yourself. ""With all those animals around, it's a good thing I have brought the finest hunters in the realm.""", "Join the battle.");
                EventManager.Instance.StoryText(@"Thanks for playing!", "End.");
                UI.MapRenderer.Instance.QueueText(new UI.TextToDisplay(() => Application.Quit()));
            }

        }

        public static List<RewardEvent> GetEvents(NodeGraph node)
        {
            BiomeType biome = node.BiomeType;

            var result = new List<RewardEvent>();
            if (generic_random_biome_events[(int)biome].Length > 0)
            {
                if(generic_random_biome_events[(int)biome].Length == 1)
                {
                    result.Add(generic_random_biome_events[(int)biome][0]);
                }
                else if(generic_random_biome_events[(int)biome].Length == 2)
                {
                    var dice = rnd.NextDouble();
                    //25% for both
                    if( dice < 0.25)
                    {
                        result.Add(generic_random_biome_events[(int)biome][0]);
                        result.Add(generic_random_biome_events[(int)biome][1]);
                    }
                    else
                    {
                        result.Add(generic_random_biome_events[(int)biome][rnd.Next(2)]);
                    }
                }
                else
                {
                    var dice = rnd.NextDouble();
                    var randomArray = generic_random_biome_events[(int)biome].OrderBy(x => rnd.Next()).ToArray();
                    // 10% for three
                    if (dice < 0.1)
                    {                        
                        result.Add(randomArray[0]);
                        result.Add(randomArray[1]);
                        result.Add(randomArray[2]);
                    }
                    //25% for two
                    else if(dice < 0.35)
                    {
                        result.Add(randomArray[1]);
                        result.Add(randomArray[1]);
                    }
                    else
                    {
                        result.Add(randomArray[0]);
                    }
                }                
            }

            if(guaranteed_biome_events[(int)biome].Length > 0)
            {
                result.AddRange(guaranteed_biome_events[(int)biome]);
            }

            StoryEvents(biome, node.Index, node.Visited);

            return result;
        }
    }
}
