using CardGame;
using CardGame.CardComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Cards
{
    class TreasureHunter : MonoBehaviour
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
                if (_card.Player_owned) GameManager.GetInstance().Game_Queue.PlayerDrawCard();
                else GameManager.GetInstance().Game_Queue.EnemyDrawCard();
            }
        }
    }
}