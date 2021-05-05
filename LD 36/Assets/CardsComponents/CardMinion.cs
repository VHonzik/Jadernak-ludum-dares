using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CardGame.CardComponents
{

    //[RequireComponent(typeof(Card), typeof(DetachableCard), typeof(PhysicalCard))]
    //[RequireComponent(typeof(CardWithCost), typeof(CardWithCollider), typeof(CardWithPortrait))]
    public class CardMinion : MonoBehaviour
    {
        private Card _card;
        private DetachableCard _detach_card;
        private PhysicalCard _physical_card;
        private CardWithCost _cost;
        private CardWithHP _hp;
        private CardWithAttack _attack;
        private MoveableCard _move_card;
        private CardWithCollider _collider_card;
        private CardWithPortrait _portrait_card;
        private AttackCapableMinion _minion_can_attack;

        public delegate void DroppedOnBoardHandler(Board board);
        // Triggered when the minion enters the board, local to the card, useful for card implementations
        public event DroppedOnBoardHandler EDroppedOnBoard;


        public Card KilledBy { get; set; }

        public bool IsBeingDestroyed = false;

        void Awake()
        {
            _card = GetComponent<Card>();
            _detach_card = GetComponent<DetachableCard>();
            _physical_card = GetComponent<PhysicalCard>();
            _cost = GetComponent<CardWithCost>();
            _hp = GetComponent<CardWithHP>();
            _attack = GetComponent<CardWithAttack>();
            _move_card = GetComponent<MoveableCard>();
            _collider_card = GetComponent<CardWithCollider>();
            _portrait_card = GetComponent<CardWithPortrait>();
        }

        public void Init()
        {

        }

        public IEnumerator DropImmidiately()
        {
            Board wanted_board = _card.Player_owned ? GameManager.GetInstance().Player_board : GameManager.GetInstance().Enemy_board;
            Vector3 wanted_position; int wanted_index;
            wanted_board.RegisterCandidate(_card, 0);
            wanted_board.PrepareForDrop(out wanted_position, out wanted_index);

            transform.rotation = Card.Flipped_On;
            transform.position = wanted_position;

            yield return StartCoroutine(MakeIntoMinion());

            wanted_board.DropCandidate(_card, wanted_index);
            if (EDroppedOnBoard != null) EDroppedOnBoard(wanted_board);

            _card.In_hand = false;
            _card.On_board = true;
            _card.ManipulationEnable(true);
        }

        private IEnumerator Drop()
        {
            GameManager.GetInstance().Game_Queue.PlayedCard(_card);

            Hand current_hand = _card.Player_owned ? GameManager.GetInstance().Player_hand : GameManager.GetInstance().Enemy_hand;
            current_hand.Remove(_card);

            Board wanted_board = _card.Player_owned ? GameManager.GetInstance().Player_board : GameManager.GetInstance().Enemy_board;
            Vector3 wanted_position; int wanted_index;
            wanted_board.PrepareForDrop(out wanted_position, out wanted_index);            

            transform.rotation = Card.Flipped_On;

            _move_card.Move(wanted_position, false);
            WaitForCallback<MoveableCard> helper = new WaitForCallback<MoveableCard>(_move_card.MovementArbitraryCoroutine);
            yield return StartCoroutine(helper.Do());

            yield return StartCoroutine(MakeIntoMinion());

            wanted_board.DropCandidate(_card, wanted_index);
            if (EDroppedOnBoard != null) EDroppedOnBoard(wanted_board);

            _card.On_board = true;
            _card.ManipulationEnable(true);
        }

        public IEnumerator DropAI()
        {
            _move_card.Flip(false).Move(GameManager.Enemy_show_card, false).MoveWait(1.5f);

            WaitForCallback<MoveableCard> helper = new WaitForCallback<MoveableCard>(_move_card.MovementArbitraryCoroutine);
            yield return StartCoroutine(helper.Do());

            GameManager.GetInstance().Enemy_board.RegisterCandidate(_card, 0);

            yield return StartCoroutine(Drop());
        }

        public IEnumerator DropPlayer()
        {
            _card.ManipulationEnable(false);
            transform.position = _detach_card.Detached_position;

            yield return StartCoroutine(Drop());
        }

        private IEnumerator MakeIntoMinion()
        {
            StartCoroutine(_physical_card.MakeIntoMinion());
            yield return new WaitForSeconds(0.1f);
            _cost.Hide();
            _hp.MakeIntoMinion();
            _attack.MakeIntoMinion();
            _collider_card.MakeIntoMinion();
            _portrait_card.MakeIntoMinion();

            gameObject.AddComponent<HighlightableMinion>();
            _minion_can_attack = gameObject.AddComponent<AttackCapableMinion>();
            gameObject.AddComponent<TargetableMinion>();
            gameObject.AddComponent<InspectableMinion>().MakeIntoMinion();

            _minion_can_attack.Exhausted = 1;
            yield return null;

        }

        public void ChangeHP(int change)
        {
            _hp.Current_hp += change;

       }

    }
}
