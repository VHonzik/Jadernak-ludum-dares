using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using CardGame.CardComponents;

namespace CardGame
{
    public class Hand : MonoBehaviour, IEnumerable
    {
        private List<Card> _cards;
        public List<Card> Cards { get { return _cards; } private set { _cards = value;  } }

        private static float MaxPosAngleDif = 10.0f;
        private static float MinPosAngleDif = 3.0f;

        private static float Radius = 6.0f;

        private static int MaxNumberOfCards = 10;

        public bool Player_hand = true;


        void Awake()
        {
            _cards = new List<Card>();
        }

        public void Fill()
        {
            if(Player_hand)
            {

                Card card = CardAtlas.Instance.CreateCard("crazydave");
                AddWithoutFitting(card);
                card.GetComponent<MoveableCard>().Flip(true);

            }
            else
            {
                Card card = CardAtlas.Instance.CreateCard("ancientgate");
                AddWithoutFitting(card);

                card = CardAtlas.Instance.CreateCard("evilegg");
                AddWithoutFitting(card);

                card = CardAtlas.Instance.CreateCard("evilegg");
                AddWithoutFitting(card);
                
            }
            FitCards(true, null);
        }

        public void FitCards(bool instant, Card additional_card)
        {
            List<Card> cards = new List<Card>(_cards);
            if (additional_card) cards.Add(additional_card);

            float anglePosDif = Mathf.Lerp(MaxPosAngleDif, MinPosAngleDif, (float)cards.Count / (float)MaxNumberOfCards);

            Vector3 middle_card = new Vector3(0, 0, Radius);

            float initial_offset_pos = anglePosDif;

            bool even = cards.Count % 2 == 0;

            if (even)
            {
                initial_offset_pos = 0.5f * anglePosDif;
            }
            else
            {
                int index = (cards.Count / 2);
                cards[index].GetComponent<MoveableCard>().Move(gameObject.transform.position, instant);
                cards[index].GetComponent<MoveableCard>().Rotate((Player_hand ? Card.Flipped_On : Card.Flipped_Off), instant);
            }

            // Left side
            for (int i=0; i < (cards.Count / 2); i++)
            {
                float angle = - initial_offset_pos - i * anglePosDif;
                float reference_angle = 0.5f * Mathf.PI - (Mathf.Deg2Rad * angle);
                Vector3 wanted_pos = new Vector3(Radius * Mathf.Cos(reference_angle), 0, Radius * Mathf.Sin(reference_angle));
                wanted_pos -= middle_card;
                wanted_pos.y = (Player_hand ? -1 : 1) * Card.Height * (i + 1);
                wanted_pos.z *= (Player_hand ? 1 : -1);

                Quaternion wanted_rot = (Player_hand ? Card.Flipped_On : Card.Flipped_Off)
    * Quaternion.Euler(0, 0, angle);

                int index = (cards.Count / 2) - 1 - i;
                cards[index].GetComponent<MoveableCard>().Move(gameObject.transform.position + wanted_pos, instant);
                cards[index].GetComponent<MoveableCard>().Rotate(wanted_rot, instant);
            }

            // Right side
            for (int i = 0; i < (cards.Count / 2); i++)
            {
                float angle = initial_offset_pos + i * anglePosDif;
                float reference_angle = 0.5f * Mathf.PI - (Mathf.Deg2Rad * angle);
                Vector3 wanted_pos = new Vector3(Radius * Mathf.Cos(reference_angle), 0, Radius * Mathf.Sin(reference_angle));
                wanted_pos -= middle_card;
                wanted_pos.z *= (Player_hand ? 1 : -1);

                wanted_pos.y = (Player_hand ? 1 : -1) * Card.Height * (i + 1);

                Quaternion wanted_rot = (Player_hand ? Card.Flipped_On : Card.Flipped_Off)
    * Quaternion.Euler(0, 0, angle);

                int index = (cards.Count / 2) + (even ? 0 : 1) + i;
                cards[index].GetComponent<MoveableCard>().Move(gameObject.transform.position + wanted_pos, instant);
                cards[index].GetComponent<MoveableCard>().Rotate(wanted_rot, instant);
            }

        }

        private void AddWithoutFitting(Card card)
        {
            _cards.Add(card);
            card.transform.parent = gameObject.transform;
            card.Player_owned = Player_hand;
            card.In_hand = true;
        }

        private IEnumerator AddWrapper(Card card)
        {
            AddWithoutFitting(card);
            yield break;
        }

        public void Add(Card card)
        {
            if (card.On_board || card.GetComponent<InspectableMinion>()) Debug.Break();
            FitCards(false, card);
            card.GetComponent<MoveableCard>().MovementArbitraryCoroutine(AddWrapper(card));
        }

        public void Remove(Card card)
        {
            card.In_hand = false;
            _cards.Remove(card);
            card.transform.parent = null;
            FitCards(false, null);

            if (Player_hand)
            {
                card.ManipulationEnable(false);
            }
        }

        public IEnumerator GetEnumerator()
        {
            return _cards.GetEnumerator();
        }

        public void ManipulationEnabled(bool value)
        {
            foreach(Card card in _cards)
            {
                card.ManipulationEnable(value);
            }
        }

    }
}
