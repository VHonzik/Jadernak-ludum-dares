using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CardGame.CardComponents
{
    public class CardSpell : MonoBehaviour
    {
        private Card _card;
        private DetachableCard _detach_card;
        private MoveableCard _move_card;

        public delegate void CastHandler();
        // Triggered when the spell is cast, local to the card, useful for card implementations
        public event CastHandler ECast;

        void Awake()
        {
            _card = GetComponent<Card>();
            _detach_card = GetComponent<DetachableCard>();
            _move_card = GetComponent<MoveableCard>();
        }

        public IEnumerator Cast()
        {
            GameManager.GetInstance().Game_Queue.PlayedCard(_card);

            Hand current_hand = _card.Player_owned ? GameManager.GetInstance().Player_hand : GameManager.GetInstance().Enemy_hand;
            current_hand.Remove(_card);

            if(ECast != null) ECast();
            GameManager.GetInstance().Game_Queue.SpellCast(this);

            StartCoroutine(_card.GetComponent<PhysicalCard>().MakeIntoMinion());
            yield return new WaitForSeconds(0.1f);
            _card.GetComponent<CardWithCost>().Hide();
            _card.GetComponent<CardWithPortrait>().Hide();

            yield return new WaitForSeconds(1.0f);

            Destroy(gameObject);
        }

        public IEnumerator CastPlayer()
        {
            _card.ManipulationEnable(false);
            transform.position = _detach_card.Detached_position;

            yield return StartCoroutine(Cast());
        }

        public IEnumerator CastAI()
        {
            _move_card.Flip(false).Move(GameManager.Enemy_show_card, false).MoveWait(1.5f);

            WaitForCallback<MoveableCard> helper = new WaitForCallback<MoveableCard>(_move_card.MovementArbitraryCoroutine);
            yield return StartCoroutine(helper.Do());

            yield return StartCoroutine(Cast());
        }

    }
}