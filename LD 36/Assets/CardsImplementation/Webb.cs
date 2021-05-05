using CardGame;
using CardGame.CardComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Cards
{
    class Webb : MonoBehaviour
    {
        private Card _card;

        void Awake()
        {
            _card = GetComponent<Card>();
            _card.GetComponent<CardMinion>().EDroppedOnBoard += new CardMinion.DroppedOnBoardHandler(DroppedOnBoard);
        }

        void DroppedOnBoard(Board board)
        {
            StartCoroutine(Discover.DiscoverCard(_card.Player_owned, x => x.Definition.Type == CardType.Spell));
        }
    }
}
