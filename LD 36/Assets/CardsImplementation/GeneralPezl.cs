using CardGame;
using CardGame.CardComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Cards
{
    class GeneralPezl : MonoBehaviour
    {
        private Card _card;
        private GameQueue.MinionDiedHandler handler;

        void Awake()
        {
            _card = GetComponent<Card>();
            _card.GetComponent<CardMinion>().EDroppedOnBoard += new CardMinion.DroppedOnBoardHandler(DroppedOnBoard);
            handler = new GameQueue.MinionDiedHandler(OnDeath);
        }

        void DroppedOnBoard(Board board)
        {
            GameManager.GetInstance().Game_Queue.EMinionDied += handler;
        }

        void OnDeath(CardMinion minion, Card killer)
        {
            if(minion == GetComponent<CardMinion>())
            {
                GameManager.GetInstance().Game_Queue.EMinionDied -= handler;
                GameManager.GetInstance().Game_Queue.SummonMinion("nuke", _card.Player_owned);
            }
        }
    }
}
