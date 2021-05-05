using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CardGame.CardComponents
{
    ////[RequireComponent(typeof(DetachableCard), typeof(Card))]
    class AttackCapableMinion : MonoBehaviour
    {
        private int _exhausted;
        public int Exhausted
        {
            get { return _exhausted; }
            set
            {
                ExhaustChanged(_exhausted, value); _exhausted = value;
            }
        }

        private bool _wants_to_attack;

        private Card _card;

        private Card _target;
        private Card Target
        {
            get { return _target; }
            set
            {
                if(_target != value)
                {
                    if(_target && _target.GetComponent<TargetableMinion>()) _target.GetComponent<TargetableMinion>().Reset();
                    if(value && value.GetComponent<TargetableMinion>()) value.GetComponent<TargetableMinion>().Target();
                }
                _target = value;
            }
        }

        private HighlightableMinion _highlight_minion;
        private DetachableCard _detach_card;
        private CardWithTargetingLines _lines_card;


        void Awake()
        {
            _card = GetComponent<Card>();

            _target = null;

            _highlight_minion = GetComponent<HighlightableMinion>();
            _detach_card = GetComponent<DetachableCard>();
            _lines_card = GetComponent<CardWithTargetingLines>();
            _exhausted = 1;
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0) && !_wants_to_attack && Exhausted <= 0 && _card.Player_owned)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform.gameObject == gameObject)
                    {
                        _wants_to_attack = true;
                        _highlight_minion.Highlight(true);
                        GameManager.GetInstance().ManipulationEnabled(false);
                        GameManager.GetInstance().InspectionEnabled(false);
                        _card.ManipulationEnable(true);
                    }
                }
            }

            if (_wants_to_attack && !Input.GetMouseButton(0))
            {
                if (Target) GameManager.GetInstance().Game_Queue.Attack(GetComponent<CardMinion>(), Target.GetComponent<CardMinion>());
                Target = null;
                _wants_to_attack = false;
                _highlight_minion.Highlight(false);
                _lines_card.Enabled = false;
                GameManager.GetInstance().ManipulationEnabled(true);
                GameManager.GetInstance().InspectionEnabled(true);
            }

            if (_wants_to_attack)
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
                        hitted_card.Player_owned != _card.Player_owned &&
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
        }

        private void ExhaustChanged(int original_value, int new_value)
        {
            if (original_value > 0 && new_value <= 0)
            {
                _highlight_minion.ShowGreenOutline(true);
            }
            else if (original_value <= 0 && new_value > 0)
            {
                _highlight_minion.ShowGreenOutline(false);
            }
        }

        public IEnumerator AttackMovement(CardMinion target)
        {
            _detach_card.Detached = true;
            _detach_card.Detached_position = transform.position;

            Vector3 dir = transform.position - target.transform.position;
            Vector3 wanted_pos = target.transform.position + dir.normalized * 0.7f;
            GetComponent<MoveableCard>().MoveDetached(wanted_pos);

            WaitForCallback<MoveableCard> helper = new WaitForCallback<MoveableCard>(GetComponent<MoveableCard>().MovementArbitraryCoroutine);
            yield return StartCoroutine(helper.Do());


        }

        public IEnumerator AttackReturnMovement()
        {
            GetComponent<MoveableCard>().ReturnDetached();

            WaitForCallback<MoveableCard> helper = new WaitForCallback<MoveableCard>(GetComponent<MoveableCard>().MovementArbitraryCoroutine);
            yield return StartCoroutine(helper.Do());

            _detach_card.Detached = false;
        }

        public bool CanAttack()
        {
            return Exhausted <= 0;
        }
    }
}
