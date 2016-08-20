using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Events
{
    class StringTable
    {
        private static Random rnd = new Random();
        private static Dictionary<string, string[]> dict = new Dictionary<string, string[]>
        {
            { "PARTY_MEMBERS", new string[] {
                "Oliver","Jack","Noah","Jacob","Charlie","Harry","Joshua","James","Ethan","Thomas","William","Henry","Oscar-9",
                "Daniel","Max","Leo","George","Alfie","Alexander","Lucas","Logan","Dylan","Adam","Isaac","Finley","Samuel","Benjamin",
                "Theo","Liam","Freddie","Joseph"
                }
            },
            { "SKIP_EVENT", new string[] {
                "Move on.",
                "There is no time to waste time.",
                "Continue.",
                "Hit the road."
                }
            },
            { "END_GAME", new string[] {
                "You have pushed yourself too hard and now have to pay the price for it. Brotherhood won't be pleased.",
                }
            },
            { "GENERIC_VILLAGE_TEXT", new string[] {
                "Brother Joseph grew up in a village just like this one. I could tell just from looking at him.",
                "You could probably count population of this place on the fingers of one hand.",
                "I've alway had little interest in agricultural life."
                }
            },
            { "GENERIC_LAKE_TEXT", new string[] {
                "The surface of this lake is almost a perfect mirror. In your current state I would advise not getting too close to it.",
                "Many lakes has at least one legend about monstrous fish. The reality are monstrous ferry tolls.",
                }
            },
            { "GENERIC_GRASSLAND_TEXT", new string[] {
                "Endless green pastures and golden fields are common theme in poems.",
                "Brother Brick claims he is allergic to green after spending a year on west coast grasslands.",
                "A saying goes that the grass movement in the wind is a cousin of sea waves."
                }
            },            
            { "GENERIC_FOREST_TEXT", new string[] {
                "For many brothers forests are their second home.",
                "Forest can be both exciting and scary place to be.",
                "You gladly breath in all the fresh forest air."
                }
            },
            { "GENERIC_HILLS_TEXT", new string[] {
                "Imagine a land without any hills. It would be so plain and boring.",
                "You have seen enough battles to feel safer on the top of a hill. ",
                "Brother Liam saw women parts in pretty much anything. He would giggle at the sight of these hills. "
                }
            },
            { "GENERIC_DESERT_TEXT", new string[] {
                "Nothing but dirt and dust as far as eye can see.",
                "Even for the prepared ones, desert is a harsh land.",
                "Not many folks travel these lands and for a reason."
                }
            },
            { "GENERIC_SWAMP_TEXT", new string[] {
                "Swamps has just the right amount of water that you can't walk and just enough ground that you can't sail.",
                "The smell is the worst when it comes to swamps.",
                }
            },
            { "GENERIC_MOUNTAINS_TEXT", new string[] {
                "Mountains, the more you have to work for something the more you will like it.",
                "How does it feel to touch the sky?",
                "Sometimes going down can be harder than going up."
                }
            },
            { "THREE_QUEENS_TEXT", new string[] {
                "The beauty and majesty of the Three Queens lakes is known across the world.",
                }
            },
            { "TOWERING_WELLS_TEXT", new string[] {
                "The major source of water for the continent, this mountains range works like a rain magnet.",
                }
            },
            { "MEANFORK_TEXT", new string[] {
                "Meanfork is infamous for attracting bad sort of crowd. This vital crossroads is a place to get quickly rich or death.",
                }
            },
            { "TWOBRIDGE_TEXT", new string[] {
                "Twobridge is the continent oldest city. Wealthy conservatives dynasties has a long tradition and considerable power here.",
                }
            },
            { "BOWTOWN_TEXT", new string[] {
                "Bowtown has long produced the realms finest hunters and archers. Not by coincidence Brotherhood heavily focuses on recruiting in Bowtown.",
                }
            },
            { "POETSHOUSE_TEXT", new string[] {
                "Officials of the Poets house tend to present it as the cultural center of the realm. The fact is you can find a place to get drunk any hour of the day.",
                }
            },
            { "COINCITY_TEXT", new string[] {
                "Discovery of silver in the Green pouch caused a population boom in Coin city. The city is still recovering from it.",
                }
            },
            { "FARREACH_TEXT", new string[] {
                "Thanks to its remoteness of Farreach is more liberal than most of the realm but at the same time hosts largest ministry prisons and labour camps.",
                }
            },
            { "CEASINGSPOT_TEXT", new string[] {
                "Sleepy and secluded village of Ceasing spot offers little attractions. Brother Micheal says if the end of the world existed it would be there.",
                }
            },
            { "CITY_OF_SMOKES_TEXT", new string[] {
                "Many wonder how the mist-filled City of smokes can prosper considering its unpleasant location. Nevertheless it does.",
                }
            },
            { "BISHOPS_SEAT_TEXT", new string[] {
                "Governmental center of the realm, Bishops's seat is also the base of operation for the Brotherhood and Ministry.",
                }
            },
            { "LAST_RESPITE_TEXT", new string[] {
                "Place of great religious importance, Last respite is well known but rarely visited.",
                }
            }, 
            { "GRASPING_LEDGE_TEXT", new string[] {
                "Famously, walls of Grasping ledge were never breached. The city is relatively prosperous and of great military importance.",
                }
            },
            { "GENERIC_BUY_FOOD_EVENT", new string[] {
                "Buy food at local market.",
                "Replenish food reserves in a pub.",

                }
            },
            { "GENERIC_BUY_FOOD_REWARD", new string[] {
                "These few cabbages would last a year for brother Nick. Mostly because he hates cabbages.",
                "Nothing like a good chunk of meat, aye?",
                "If there anything I don't mind carrying, it's food."
                }
            },
            { "GENERIC_BUY_SUPPLIES_EVENT", new string[] {
                "Buy supplies.",
                "Look around for equipment.",
                }
            },
            { "GENERIC_BUY_SUPPLIES_REWARD", new string[] {                
                "Ropes. Check. Torches. Checks. Repair kits. Check.",
                "You were born prepared.",
                "Fortunes favors the prepared."
                }
            },
            { "DESERT_MONEY_REWARD", new string[] {
                "A sun bleached skeletons have a little use of money. Inspirational, isn't it?",
                "Coins are easy to spot under scorching sun."
                }
            },
            { "GENERIC_SCAVANGE_EVENT", new string[] {
                "Explore remains of caravan.",
                "Search abandoned hut."
                }
            },
            { "GENERIC_SCAVANGE_FAIL", new string[] {
                "They say it's about the journey not about the loot, right?.",
                "I used to be an explorer like you. I wonder if that's why I'm such pessimists.",
                "Just like it is often the case in Poet's house, everything was stripped naked.",
                "One man trash is most likely other man trash."
                }
            },
            { "GENERIC_SCAVANGE_SUPPLIES_REWARD", new string[] {
                "For a crafty man such as yourself, a lot of junk items has a use.",
                "How did you manage to find that in there is beyond my imagination.",
                "I guess in a big enough pile of trash there is bound to be one useful thing."
                }
            },
            { "GENERIC_HUNT_EVENT", new string[] {
                "Hunt for food.",
                "Lay animal traps"
                }
            },
            { "GENERIC_HUNT_FOOD_REWARD", new string[] {
                "Skilled hunter can be only one glimpse away from a dinner.",
                "Today you eat like kings!",
                "Gods smile upon you and your bow."
                }
            },
            { "GENERIC_HUNT_FOOD_FAIL", new string[] {
                "Judging by your arrow, today's dinner is gonna be boiled tree bark?",
                "That was ... a miss.",
                "There is always another arrow."
                }
            },
            { "GENERIC_COLLECT_FOOD_EVENT", new string[] {
                "Look around for food.",
                "Collect edible plants."

                }
            },
            { "GENERIC_COLLECT_FOOD_REWARD", new string[] {
                "You've stumbled upon a field of corn. Yummy.",
                "The hours and hours spent on the herbarium finally pay off."
                }
            },
            { "GENERIC_COLLECT_FOOD_FAIL", new string[] {
                "You've only found grass and you are not cow.",
                "Everything you've discovered either had thorns or was poisonous."
                }
            },
            { "GENERIC_FISHING_EVENT", new string[] {
                "Go fishing.",
                }
            },
            { "GENERIC_FISHING_REWARD", new string[] {
                "If this whole brotherhood business does not work out you can always be fisherman.",
                "That's a 20 pounds of fishiness right there.",
                "The fish fought bravely."
                }
            },
            { "GENERIC_FISHING_FAIL", new string[] {
                "Your improves rod is not as effective as you have hoped.",
                "You swear the float hasn't moved an inch.",
                "That oily bastard escaped at the last second."
                }
            },
            { "MOUNTAINS_EXPLORE_EVENT", new string[] {
                "Explorer caves.",
                "Descend into a crevice"
                }
            },
            { "MOUNTAINS_GOLD_REWARD", new string[] {
                "Value of the mineral you found is anything between a hot supper and a new sword.",
                "Even tiny gold vein can make you rich.",
                "Some unfortunate souls took a flying lesson. On top of his life it now costs him his things."
                }
            },
            { "MOUNTAINS_EXPLORE_FAIL", new string[] {
                "It was dark, damp and overall unpleasant experience.",
                "One could spent days down there.",
                "You almost fell to your death and it wasn't even worth it."
                }
            }, 
            { "MOUNTAINS_SUPPLIES_REWARD", new string[] {
                "The ropes you've \"burrowed\" from down there might become handy.",
                "Dark places attract torches aplenty.",
                }
            },


        };
        public static string GetRandomText(string key)
        {
            if(dict.ContainsKey(key))
            {
                int index = rnd.Next(dict[key].Length);
                return dict[key][index];
            }
            else
            {
                return key;
            }
        }
    }
}
