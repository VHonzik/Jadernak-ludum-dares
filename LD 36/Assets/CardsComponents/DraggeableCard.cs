using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CardGame.CardComponents
{
    //[RequireComponent(typeof(Card), typeof(HighlightableCard), typeof(DetachableCard))]
    //[RequireComponent(typeof(CardWithCost))]
    public class DraggeableCard : MonoBehaviour
    {
        public bool Dragging { get; set; }

        private CardWithCost _cost_card;
        private HighlightableCard _highlight_card;
        private DetachableCard _detach_card;
        private Card _card;

        void Awake()
        {
            _highlight_card = GetComponent<HighlightableCard>();
            _detach_card = GetComponent<DetachableCard>();
            _card = GetComponent<Card>();
            _cost_card = GetComponent<CardWithCost>();

            Dragging = false;
        }

        void Update()
        {
            if (_highlight_card.Highlighted && Input.GetMouseButton(0) && !Dragging && _cost_card.CanAfford())
            {
                _highlight_card.Highlight(false);
                Dragging = true;
                _detach_card.Detached = true;
                _detach_card.Detach_orientation = Card.Flipped_On;

                GameManager.GetInstance().ManipulationEnabled(false);
                _card.ManipulationEnable(true);
            }

            if (Dragging && !Input.GetMouseButton(0))
            {
                Dragging = false;

                if (!GetComponent<CardTargetedSpell>())
                {
                    _detach_card.Detached = false;
                    GameManager.GetInstance().ManipulationEnabled(true);
                }

                Board wanted_board = _card.Player_owned ? GameManager.GetInstance().Player_board : GameManager.GetInstance().Enemy_board;

                if (GetComponent<CardMinion>() 
                    && _detach_card.Detached_position.z > GameManager.Player_hand_threshold
                    && wanted_board.Cards.Count < Board.MaxNumberOfMinions)
                {                    
                    StartCoroutine(GetComponent<CardMinion>().DropPlayer());
                    
                }

                if (GetComponent<CardSpell>()
                    && _detach_card.Detached_position.z > GameManager.Player_hand_threshold)
                {
                    StartCoroutine(GetComponent<CardSpell>().CastPlayer());
                }
            }

            if (Dragging)
            {

                Vector3 point = Input.mousePosition;
                point.z = Camera.main.transform.position.y - transform.position.y;
                Vector3 wanted_pos = Camera.main.ScreenToWorldPoint(point);

                if (_detach_card.Detached_position.z >= GameManager.Player_hand_threshold && GetComponent<CardTargetedSpell>())
                {
                    GetComponent<CardTargetedSpell>().IsCasting = true;
                    if(_detach_card.Detached_position.z >= GameManager.Player_hand_threshold + 0.1f)
                    {
                        wanted_pos.z = GameManager.Player_hand_threshold + 0.05f;
                        _detach_card.Detached_position = wanted_pos;
                    }                    
                }
                else
                {
                    _detach_card.Detached_position = wanted_pos ;
                    if(GetComponent<CardTargetedSpell>()) GetComponent<CardTargetedSpell>().IsCasting = false;
                }

                Board wanted_board = _card.Player_owned ? GameManager.GetInstance().Player_board : GameManager.GetInstance().Enemy_board;

                if (GetComponent<CardMinion>() && _detach_card.Detached_position.z > GameManager.Player_hand_threshold)
                {                    
                    wanted_board.RegisterCandidate(_card);
                }
                else
                {
                    wanted_board.RegisterCandidate(null);
                }

            }
        }

    }
}
