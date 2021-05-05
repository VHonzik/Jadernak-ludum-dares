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
    class EvilEgg : MonoBehaviour
    {
        private Card _card;
        private GameQueue.MinionDiedHandler _died_handler;
        private GameQueue.StartOfTurnHandler _start_handler;

        private int turn_count;

        void Awake()
        {
            _card = GetComponent<Card>();
            _card.GetComponent<CardMinion>().EDroppedOnBoard += new CardMinion.DroppedOnBoardHandler(DroppedOnBoard);
            _died_handler = new GameQueue.MinionDiedHandler(OnMinionDied);
            _start_handler = new GameQueue.StartOfTurnHandler(OnTurnStart);
        }

        void DroppedOnBoard(Board board)
        {
            GameManager.GetInstance().Game_Queue.EMinionDied += _died_handler;
            GameManager.GetInstance().Game_Queue.EStartOfTurn += _start_handler;
            turn_count = 0;
        }

        private void OnMinionDied(CardMinion minion, Card killer)
        {
            if (minion == GetComponent<CardMinion>())
            {
                GameManager.GetInstance().Game_Queue.EMinionDied -= _died_handler;
                GameManager.GetInstance().Game_Queue.EStartOfTurn -= _start_handler;
            }
        }

        void OnTurnStart(bool player_turn)
        {
            if(player_turn == _card.Player_owned)
            {
                _card.GetComponent<CardWithHP>().AddPernament(1);
                _card.GetComponent<CardWithAttack>().AddPernament(1);
                turn_count += 1;
            }

            if(turn_count > 10)
            {
                GameManager.GetInstance().Game_Queue.Destroy(_card.GetComponent<CardMinion>(), _card);
                if (!GameManager.GetInstance().AncientEvilSummoned)
                {
                    GameManager.GetInstance().AncientEvilSummoned = true;
                    GameManager.GetInstance().Game_Queue.SummonMinion("ancientevil", _card.Player_owned);
                }
            }
        }
    }
}
