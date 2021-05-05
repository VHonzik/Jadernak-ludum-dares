using CardGame;
using CardGame.CardComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Cards
{
    //[RequireComponent(typeof(Card), typeof(CardMinion))]
    class AncientEvil : MonoBehaviour
    {
        private Card _card;

        void Awake()
        {
            _card = GetComponent<Card>();
            _card.GetComponent<CardMinion>().EDroppedOnBoard += new CardMinion.DroppedOnBoardHandler(DroppedOnBoard);
        }

        void DroppedOnBoard(Board board)
        {
            _card.GetComponent<AttackCapableMinion>().Exhausted -= 1;
        }

    }
}
