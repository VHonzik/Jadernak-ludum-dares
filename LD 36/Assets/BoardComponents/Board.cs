using CardGame.CardComponents;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CardGame
{
    public class Board : MonoBehaviour
    {
        public List<Card> Cards { get; private set; }

        public static int MaxNumberOfMinions = 7;

        private Card _candidate;
        private int _candidate_index;
        private bool _forced_index;

        private static Vector3 _offset = new Vector3(1.0f, 0, 0);

        void Awake()
        {
            Cards = new List<Card>();
            _candidate = null;
            _candidate_index = -1;
            _forced_index = false;
        }

        public void Add(Card card)
        {
            card.In_hand = false;
            card.transform.parent = transform;
            Cards.Insert(BoardPositonFromWorldPosition(card), card);
        }

        public void RegisterCandidate(Card candidate, int forced_position = -1)
        {
            // New existing
            if(candidate && _candidate != candidate && Cards.Count < Board.MaxNumberOfMinions)
            {
                _candidate = candidate;
                if(forced_position >= 0)
                {
                    _candidate_index = forced_position;
                    _forced_index = true;
                }
                else
                {
                    _candidate_index = BoardPositonFromWorldPosition(candidate);
                }
                
                FitCards(candidate, false);
            }
            // Reset
            else if(!candidate && _candidate != candidate && Cards.Count < Board.MaxNumberOfMinions)
            {
                _candidate = candidate;
                _candidate_index = -1;
                _forced_index = false;
                FitCards(null, false);
            }
        }

        void Update()
        {
            if(_candidate && Cards.Count < Board.MaxNumberOfMinions && !_forced_index)
            {
                int index = BoardPositonFromWorldPosition(_candidate);
                if(index != _candidate_index)
                {
                    FitCards(_candidate, false);
                    _candidate_index = index;
                }
            }
        }

        private Vector3 WantedPositionFromIndexAndCount(int index, int count)
        {
            Vector3 offset = Vector3.zero;

            if (count % 2 == 0)
            {
                offset = (index - ((count / 2) - 0.5f)) * _offset;                
            }
            else
            {
                offset = (index - (count / 2)) * _offset;
            }

            return transform.TransformPoint(offset);
        }

        public Vector3 SpawnPositionFromWorldPosition(Card card)
        {
            int index = BoardPositonFromWorldPosition(card);
            return WantedPositionFromIndexAndCount(index, Cards.Count + 1);
        }

        private int BoardPositonFromWorldPosition(Card card)
        {

            if (Cards.Count == 0)
            {
                return 0;
            }

            int i = 0;
            Card current = Cards[i];

            while(card.GetComponent<DetachableCard>().Detached_position.x > current.transform.position.x)
            {
                i++;

                if(i >= Cards.Count)
                {
                    i = Cards.Count;
                    break;
                }

                current = Cards[i];
            }

            return i;
        }

        private void FitCards(Card additional_card, bool instant)
        {
            int add_index = Cards.Count;
            if (additional_card && additional_card == _candidate)
            {
                add_index = _candidate_index;
            }
            else if (additional_card)
            {
                add_index = BoardPositonFromWorldPosition(additional_card);
            }
            
            int add_count = additional_card ? Cards.Count + 1  : Cards.Count;
            for (int i=0; i< Cards.Count(); i++)
            {
                int real_index = (i >= add_index) ? i + 1 : i;
               Cards[i].GetComponent<MoveableCard>().Move(WantedPositionFromIndexAndCount(real_index, add_count), instant);
            }
        }

        public void PrepareForDrop(out Vector3 position, out int index)
        {
            index = _candidate_index;
            position = WantedPositionFromIndexAndCount(_candidate_index, Cards.Count+1);
            _candidate = null;
            _candidate_index = 0;
        }

        public void DropCandidate(Card card, int index)
        {
            card.transform.parent = transform;
            Cards.Insert(index, card);
        }

        public void OwnerTurnStart()
        {
            foreach(Card card in Cards)
            {
                int current = card.GetComponent<AttackCapableMinion>().Exhausted;
                card.GetComponent<AttackCapableMinion>().Exhausted = current - 1;
            }
        }

        public IEnumerator GetEnumerator()
        {
            return Cards.GetEnumerator();
        }

        public void OwnerTurnEnd()
        {
            foreach (Card card in Cards)
            {
                int current = card.GetComponent<AttackCapableMinion>().Exhausted;
                if(current <= 0) card.GetComponent<AttackCapableMinion>().Exhausted = 1;
            }
        }

        public void Remove(Card card)
        {
            Cards.Remove(card);
            card.transform.parent = null;
            FitCards(null, false);
        }

        public bool IsFull()
        {
            return Cards.Count >= MaxNumberOfMinions;
        }

        internal void InspectionEnabled(bool value)
        {
            foreach(Card card in Cards)
            {
                card.InspectionEnable(value);
            }
        }
    }
}
