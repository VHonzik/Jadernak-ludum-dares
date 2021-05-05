using CardGame;
using CardGame.CardComponents;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Cards
{
    class Discover
    {
        public static Vector3 DiscoverPosition = new Vector3(0, 6.84f, 0f);
        public static IEnumerator DiscoverCard(bool player, Predicate<Card> predicate)
        {
            GameManager.GetInstance().ManipulationEnabled(false);
            GameManager.GetInstance().InspectionEnabled(false);

            Stock wanted_stock = player ? GameManager.GetInstance().Player_stock : GameManager.GetInstance().Enemy_stock;
            List<Card> cards = wanted_stock.Cards.FindAll(predicate);
            cards.Shuffle();
            cards = cards.Take(3).ToList();

            if (cards.Count > 0 )
            {
                Vector3 start = DiscoverPosition;
                float step = -1.2f;

                for (int i=0; i < cards.Count; i++)
                {
                    Vector3 wanted_pos = start;
                    wanted_pos.x = ((cards.Count % 2 == 0) ? (i - ((cards.Count / 2) - 0.5f)) : (i - (cards.Count / 2))) * step;

                    GameManager.GetInstance().StartCoroutine(cards[i].GetComponent<DiscoverableCard>().Discover(wanted_pos));
                }

                
                while(true)
                {
                    Card chosen = cards.Find(x => x.GetComponent<DiscoverableCard>().Chosen == true);
                    if (chosen != null)
                    {
                        foreach( var card in cards.FindAll(x => x != chosen))
                        {
                            GameManager.GetInstance().StartCoroutine(card.GetComponent<DiscoverableCard>().ReturnToDeck());
                        }
                        break;
                    }
                    yield return null;
                }

            }

            GameManager.GetInstance().ManipulationEnabled(true);
            GameManager.GetInstance().InspectionEnabled(true);

        }
    }
}
