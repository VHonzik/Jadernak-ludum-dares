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
    class AncientGate : MonoBehaviour
    {
        private Card _card;
        private CardMinion _minion_card;
        private GameQueue.MinionDiedHandler _died_handler;
        private GameQueue.MinionDamagedHandler _damaged_handler;

        private Card _adjacent_left;
        private Card _adjacent_right;

        void Awake()
        {
            _card = GetComponent<Card>();
            _minion_card = GetComponent<CardMinion>();
            _card.GetComponent<CardMinion>().EDroppedOnBoard += new CardMinion.DroppedOnBoardHandler(DroppedOnBoard);
            _died_handler = new GameQueue.MinionDiedHandler(OnMinionDied);
            _damaged_handler = new GameQueue.MinionDamagedHandler(OnMinionDamaged);
        }

        void DroppedOnBoard(Board board)
        {
            GameManager.GetInstance().Game_Queue.EMinionDied += _died_handler;
            GameManager.GetInstance().Game_Queue.EMinionDamaged += _damaged_handler;
        }

        private void OnMinionDied(CardMinion minion, Card killer)
        {
            // Remove status effects on adjacent minion when dying
            if(minion == _minion_card)
            {
                GameManager.GetInstance().Game_Queue.EMinionDied -= _died_handler;
                GameManager.GetInstance().Game_Queue.EMinionDamaged -= _damaged_handler;
                if (_adjacent_right) _adjacent_right.GetComponent<TargetableMinion>().DisableTargetingStatusExit(_card);
                if(_adjacent_left) _adjacent_left.GetComponent<TargetableMinion>().DisableTargetingStatusExit(_card);
            }
        }

        private void OnMinionDamaged(CardMinion minion, int damage)
        {
            // Immune to damage
            if (minion == _minion_card)
            {
                minion.ChangeHP(+damage);
            }
        }

        void Update()
        {
            if(_card.On_board && !_card.GetComponent<CardMinion>().IsBeingDestroyed)
            {
                Board wanted_board = _card.Player_owned ? GameManager.GetInstance().Player_board : GameManager.GetInstance().Enemy_board;
                int index = wanted_board.Cards.FindIndex(x => x == _card);

                // Left
                Card left = (index - 1 >= 0) ? wanted_board.Cards[index - 1] : null;
                Card right = (index + 1 < wanted_board.Cards.Count) ? wanted_board.Cards[index + 1] : null;

                if (left != _adjacent_left)
                {
                    if (_adjacent_left) _adjacent_left.GetComponent<TargetableMinion>().DisableTargetingStatusExit(_card);
                    _adjacent_left = left;
                    _adjacent_left.GetComponent<TargetableMinion>().DisableTargetingStatus(_card);
                }

                if (right != _adjacent_right)
                {
                    if (_adjacent_right) _adjacent_right.GetComponent<TargetableMinion>().DisableTargetingStatusExit(_card);
                    _adjacent_right = right;
                    _adjacent_right.GetComponent<TargetableMinion>().DisableTargetingStatus(_card);
                }

            }
        }
    }
}
