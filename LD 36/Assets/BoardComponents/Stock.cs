using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using CardGame.CardComponents;

namespace CardGame
{
    public class Stock : MonoBehaviour
    {
        public List<Card> Cards { get { return _cards; } private set { _cards = value; } }
        private List<Card> _cards;


        public bool Player_stock = true;

        void Awake()
        {
            _cards = new List<Card>();
        }

        public void Fill()
        {
            List<string> cards = Player_stock ? CardAtlas.Instance.GetPlayerDeck() : CardAtlas.Instance.GetEnemyDeck();

            for (int i = 0; i < cards.Count; i++)
            {
                Card card = CardAtlas.Instance.CreateCard(cards[i]);
                Vector3 wanted_pos = gameObject.transform.position;
                wanted_pos.y = i * Card.StockHeight;

                card.GetComponent<MoveableCard>().Move(wanted_pos, true);

                card.gameObject.transform.parent = gameObject.transform;

                card.Player_owned = Player_stock;

                _cards.Add(card);
            }
        }

        public Card Draw()
        {
            Card card = null;

            if (_cards.Count > 0)
            {
                card = _cards.Last();
                _cards.RemoveAt(_cards.Count - 1);

                card.gameObject.transform.parent = null;
            }

            return card;
        }

        public void Draw(Card specific)
        {
            int index = _cards.FindIndex(x => x == specific);
            if (index > 0 && index < _cards.Count)
            {
                _cards.RemoveAt(index);

                specific.gameObject.transform.parent = null;
            }
        }
    }
}
