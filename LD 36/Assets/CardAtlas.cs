using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using CardGame.CardComponents;

static class MyExtensions
{
    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = GameManager.GetInstance().Random.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}

namespace CardGame
{
    public enum CardType { Minion, Spell, TargetedSpell }

    public class CardDefinition
    {
        public CardType Type { get; set; }
        public int Cost { get; set; }
        public int Health { get; set; }
        public int Attack { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string PortraitTexture { get; set; }
        public Type ImplClass { get; set; }
    }

    public class Tuple<T1, T2>
    {
        public T1 First { get; private set; }
        public T2 Second { get; private set; }
        internal Tuple(T1 first, T2 second)
        {
            First = first;
            Second = second;
        }
    }

    class CardAtlas
    {
        private Dictionary<string, CardDefinition> _dict;
        private Dictionary<string, GameObject> _cards;

        private List<Tuple<string, int>> _player_deck;
        private List<Tuple<string, int>> _enemy_deck;

        static CardAtlas the_one_and_only;

        public static CardAtlas Instance
        {
            get
            {
                if (the_one_and_only == null)
                {
                    the_one_and_only = new CardAtlas();
                }
                return the_one_and_only;
            }
        }

        public Card CreateRandom()
        {
            string rnd_key = _cards.ElementAt(GameManager.GetInstance().Random.Next(0, _cards.Count)).Key;
            return CreateCard(rnd_key);
        }

        // Actual card in game
        public Card CreateCard(string id)
        {
            GameObject handle = GameObject.Instantiate(_cards[id]) as GameObject;
            handle.SetActive(true);
            Card card = handle.GetComponent<Card>();
            card.Definition = _dict[id];
            card.Name = id;

            card.GetComponent<CardWithCost>().Init();

            if (card.GetComponent<CardWithHP>())
            {
                card.GetComponent<CardWithHP>().Init();
            }

            if (card.GetComponent<CardWithAttack>())
            {
                card.GetComponent<CardWithAttack>().Init();
            }

            return handle.GetComponent<Card>();
        }

        // "Prefab" card for optimization
        private GameObject CreateCard(CardDefinition definition)
        {
            GameObject card_handle = (GameObject)GameObject.Instantiate(Resources.Load("CardHandle"), Vector3.zero, Quaternion.identity);
            GameObject detach_handle = card_handle.transform.GetChild(0).gameObject;

            Card card = card_handle.AddComponent<Card>();
            card_handle.AddComponent<PhysicalCard>().Create(detach_handle, definition);
            card_handle.AddComponent<CardWithPortrait>().Create(detach_handle, definition);

            card_handle.AddComponent<MouseOverableCard>();
            card_handle.AddComponent<DetachableCard>();
            card_handle.AddComponent<HighlightableCard>();
            card_handle.AddComponent<DraggeableCard>();
            card_handle.AddComponent<MoveableCard>();
            card_handle.AddComponent<CardWithCollider>();
            card_handle.AddComponent<CardWithOutline>();
            card_handle.AddComponent<DiscoverableCard>();
            card_handle.AddComponent<CardWithTargetingLines>();

            card_handle.AddComponent<CardWithCost>().Create(detach_handle);

            switch (definition.Type)
            {
                case CardType.Minion:
                    card_handle.AddComponent<CardWithHP>().Create(detach_handle);
                    card_handle.AddComponent<CardWithAttack>().Create(detach_handle);
                    card_handle.AddComponent<CardMinion>();         
                    break;
                case CardType.Spell:
                    card_handle.AddComponent<CardSpell>();
                    break;
                case CardType.TargetedSpell:
                    card_handle.AddComponent<CardTargetedSpell>();
                    break;
            }

            card_handle.AddComponent(definition.ImplClass);

            card.ManipulationEnable(false);

            card_handle.SetActive(false);

            return card_handle;
        }

        private CardAtlas()
        {
            FillDict();
            CreateCards();
            FillDecks();
        }

        private void CreateCards()
        {
            _cards = new Dictionary<string, GameObject>();

            foreach(var pair in _dict)
            {
                _cards.Add(pair.Key, CreateCard(pair.Value));
            }
        }

        public List<string> GetEnemyDeck()
        {
            List<string> result = new List<string>();

            foreach (var entry in _enemy_deck)
            {
                for (int i = 0; i < entry.Second; i++)
                {
                    result.Add(entry.First);
                }
            }

            result.Shuffle();

            return result;
        }

        public List<string> GetPlayerDeck()
        {
            List<string> result = new List<string>();

            foreach (var entry in _player_deck)
            {
                for(int i=0; i < entry.Second; i++ )
                {
                    result.Add(entry.First);
                }                
            }

            result.Shuffle();
            
            return result;
        }

        private void FillDecks()
        {
            _player_deck = new List<Tuple<string, int>>()
            {
                new Tuple<string, int>("ancientartifact", 1),
                new Tuple<string, int>("professorwebb", 1),
                new Tuple<string, int>("runetablet", 1),
                new Tuple<string, int>("generalpezl", 1),
                new Tuple<string, int>("rory", 1),
                new Tuple<string, int>("treasurehunter", 2),
                new Tuple<string, int>("urn", 2),
            };

            _enemy_deck = new List<Tuple<string, int>>()
            {
                new Tuple<string, int>("todo", 30)
            };
        }

        private void FillDict()
        {
            _dict = new Dictionary<string, CardDefinition>();

            _dict.Add("todo", new CardDefinition
            {
                Type = CardType.Minion,
                Cost = 1,
                Health = 3,
                Attack = 2,
                Title = "To do",
                Description = "Random text",
                PortraitTexture = "None",
                ImplClass = typeof(Cards.ToDo)
            });

            _dict.Add("ancientartifact", new CardDefinition
            {
                Type = CardType.Spell,
                Cost = 20,
                Health = 0,
                Attack = 0,
                Title = "Ancient artifact",
                PortraitTexture = "AncientArtifact",
                Description = "Powerful energy emanates\nfrom the artifact.\nHow to use it?",
                ImplClass = typeof(Cards.AncientArtifact)
            });

            _dict.Add("ancientgate", new CardDefinition
            {
                Type = CardType.Minion,
                Cost = 1,
                Health = 10,
                Attack = 0,
                Title = "Ancient gate",
                PortraitTexture = "AncientGate",
                Description = "Immune to damage.\nProtects adjacent\nminions.",
                ImplClass = typeof(Cards.AncientGate)
            });

            _dict.Add("evilegg", new CardDefinition
            {
                Type = CardType.Minion,
                Cost = 0,
                Health = 1,
                Attack = 1,
                Title = "Ominous egg",
                PortraitTexture = "Egg",
                Description = "Gains +1/+1 at start\n of the turn. Hatches\nin 10 turns.",
                ImplClass = typeof(Cards.EvilEgg)
            });

            _dict.Add("ancientevil", new CardDefinition
            {
                Type = CardType.Minion,
                Cost = 0,
                Health = 50,
                Attack = 50,
                Title = "Ancient evil",
                PortraitTexture = "AncientEvil",
                Description = "If you see this,\n you are already dead.",
                ImplClass = typeof(Cards.AncientEvil)
            });

            _dict.Add("professorwebb", new CardDefinition
            {
                Type = CardType.Minion,
                Cost = 4,
                Health = 3,
                Attack = 4,
                Title = "Profesor Webb",
                PortraitTexture = "Webb",
                Description = "Discover a technology.",
                ImplClass = typeof(Cards.Webb)
            });

            _dict.Add("runetablet", new CardDefinition
            {
                Type = CardType.Spell,
                Cost = 6,
                Health = 0,
                Attack = 0,
                Title = "Rune Tablet",
                PortraitTexture = "RuneTablet",
                Description = "Reduce the cost\nof ancient artifact\n by 10.",
                ImplClass = typeof(Cards.RuneTablet)
            });

            _dict.Add("generalpezl", new CardDefinition
            {
                Type = CardType.Minion,
                Cost = 7,
                Health = 5,
                Attack = 5,
                Title = "General Pezl",
                PortraitTexture = "General",
                Description = "On death\nsummons a nuke.",
                ImplClass = typeof(Cards.GeneralPezl)
            });

            _dict.Add("nuke", new CardDefinition
            {
                Type = CardType.Minion,
                Cost = 10,
                Health = 2,
                Attack = 11,
                Title = "Nuke",
                PortraitTexture = "Nuke",
                Description = "",
                ImplClass = typeof(Cards.Nothing)
            });

            _dict.Add("rory", new CardDefinition
            {
                Type = CardType.Minion,
                Cost = 3,
                Health = 4,
                Attack = 2,
                Title = "Headhunter Rory",
                PortraitTexture = "Rory",
                Description = "Discover a minion.",
                ImplClass = typeof(Cards.Rory)
            });

            _dict.Add("treasurehunter", new CardDefinition
            {
                Type = CardType.Minion,
                Cost = 2,
                Health = 1,
                Attack = 2,
                Title = "Treasure hunter",
                PortraitTexture = "TreasureHunter",
                Description = "On death,\ndraws a card.",
                ImplClass = typeof(Cards.TreasureHunter)
            });

            _dict.Add("urn", new CardDefinition
            {
                Type = CardType.TargetedSpell,
                Cost = 2,
                Health = 0,
                Attack = 0,
                Title = "Ancient Urn",
                PortraitTexture = "Urn",
                Description = "Deals 3 damaged\nto enemy minion.",
                ImplClass = typeof(Cards.Urn)
            });

            _dict.Add("crazydave", new CardDefinition
            {
                Type = CardType.Minion,
                Cost = 2,
                Health = 1,
                Attack = 1,
                Title = "Crazy Dave",
                PortraitTexture = "BombHat",
                Description = "On death\ntakes his opponent\nwith him.",
                ImplClass = typeof(Cards.CrazyDave)
            });

        }
    }
}
