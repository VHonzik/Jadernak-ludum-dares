using CardGame;
using CardGame.CardComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Cards
{
    class CrazyDave : MonoBehaviour
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
            if (minion == GetComponent<CardMinion>())
            {
                GameManager.GetInstance().Game_Queue.EMinionDied -= handler;

                if(killer && !killer.GetComponent<CardMinion>().IsBeingDestroyed && killer.On_board && 
                    killer.GetComponent<AncientGate>() == null)
                {
                    GameManager.GetInstance().Game_Queue.Destroy(killer.GetComponent<CardMinion>(), _card);
                }
            }
        }
    }
}