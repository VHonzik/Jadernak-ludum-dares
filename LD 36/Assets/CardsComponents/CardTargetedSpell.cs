using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CardGame.CardComponents
{
    public class CardTargetedSpell : MonoBehaviour
    {
        private Card _card;
        private DetachableCard _detach_card;
        private MoveableCard _move_card;
        private CardWithTargetingLines _lines_card;

        public delegate void CastHandler(CardMinion target);
        // Triggered when the spell is cast, local to the card, useful for card implementations
        public event CastHandler ECast;

        public Predicate<CardMinion> TargetPredicate
        {
            get; set;
        }

        public bool IsCasting { get; set; }

        private Card _target;
        private Card Target
        {
            get { return _target; }
            set
            {
                if (_target != value)
                {
                    if (_target && _target.GetComponent<TargetableMinion>()) _target.GetComponent<TargetableMinion>().Reset();
                    if (value && value.GetComponent<TargetableMinion>()) value.GetComponent<TargetableMinion>().Target();
                }
                _target = value;
            }
        }

        void Awake()
        {
            _card = GetComponent<Card>();
            _detach_card = GetComponent<DetachableCard>();
            _move_card = GetComponent<MoveableCard>();
            _lines_card = GetComponent<CardWithTargetingLines>();
            TargetPredicate = x => true;

            IsCasting = false;
        }

        void Update()
        {
            if (IsCasting)
            {
                if (!_lines_card.Enabled) _lines_card.Enabled = true;
                Vector3 point = Input.mousePosition;
                point.z = Camera.main.transform.position.y - transform.position.y;
                _lines_card.SetStartPosition(_detach_card.Detached_position);
                _lines_card.SetEndPosition(!Target ? Camera.main.ScreenToWorldPoint(point) : Target.transform.position);

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    Card hitted_card = hit.transform.gameObject.GetComponent<Card>();
                    if (hitted_card && hitted_card.On_board &&
                        TargetPredicate.Invoke(hitted_card.GetComponent<CardMinion>()) &&
                        hitted_card.GetComponent<TargetableMinion>() && hitted_card.GetComponent<TargetableMinion>().CanBeTargeted)
                    {
                        Target = hitted_card;
                    }
                    else
                    {
                        Target = null;
                    }
                }
                else
                {
                    Target = null;
                }
            }

            if (IsCasting && !Input.GetMouseButton(0))
            {
                if (Target) StartCoroutine(Cast(Target.GetComponent<CardMinion>()));
                Target = null;
                _lines_card.Enabled = false;
                IsCasting = false;
                GameManager.GetInstance().ManipulationEnabled(true);
                GameManager.GetInstance().InspectionEnabled(true);
            }
        }

        public IEnumerator Cast(CardMinion target)
        {
            GameManager.GetInstance().Game_Queue.PlayedCard(_card);

            Hand current_hand = _card.Player_owned ? GameManager.GetInstance().Player_hand : GameManager.GetInstance().Enemy_hand;
            current_hand.Remove(_card);

            if (ECast != null) ECast(target);
            GameManager.GetInstance().Game_Queue.TargetedSpellCast(this);

            StartCoroutine(_card.GetComponent<PhysicalCard>().MakeIntoMinion());
            yield return new WaitForSeconds(0.1f);
            _card.GetComponent<CardWithCost>().Hide();
            _card.GetComponent<CardWithPortrait>().Hide();

            yield return new WaitForSeconds(1.0f);

            Destroy(gameObject);
        }

        public IEnumerator CastPlayer(CardMinion target)
        {
            _card.ManipulationEnable(false);
            transform.position = _detach_card.Detached_position;

            yield return StartCoroutine(Cast(target));
        }

        public IEnumerator CastAI(CardMinion target)
        {
            _move_card.Flip(false).Move(GameManager.Enemy_show_card, false).MoveWait(1.5f);

            WaitForCallback<MoveableCard> helper = new WaitForCallback<MoveableCard>(_move_card.MovementArbitraryCoroutine);
            yield return StartCoroutine(helper.Do());

            yield return StartCoroutine(Cast(target));
        }

    }
}