using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CardGame.CardComponents
{
    class InspectableMinion : MonoBehaviour
    {
        private Card _tooltip_card;
        private Card _card;
        private MouseOverableCard _mouse_card;
        public bool ForceInspect { get; set; }

        void Awake()
        {
            _card = GetComponent<Card>();
            _mouse_card = GetComponent<MouseOverableCard>();
        }

        void Update()
        {
            if (_tooltip_card)
            {
                if (_mouse_card.MouseOverLastFrame != _mouse_card.MouseOver || ForceInspect)
                {
                    if (_mouse_card.MouseOver || ForceInspect)
                    {
                        _tooltip_card.GetComponent<MoveableCard>().Move(
                            _card.Player_owned ? GameManager.Player_tooltip_card : GameManager.Enemy_tooltip_card, true);
                    }
                    else
                    {
                        _tooltip_card.GetComponent<MoveableCard>().Move(GameManager.Hide_cards_position, true);
                    }
                }
            }
        }

        public void MakeIntoMinion()
        {
            _tooltip_card = CardAtlas.Instance.CreateCard(_card.Name);
            _tooltip_card.transform.position = GameManager.Hide_cards_position;
            _tooltip_card.GetComponent<MoveableCard>().Flip(true);
            _tooltip_card.ManipulationEnable(false);
        }

        void OnDisable()
        {
            if(_tooltip_card) _tooltip_card.GetComponent<MoveableCard>().Move(GameManager.Hide_cards_position, true);
        }

        void OnDestroy()
        {
            if(_tooltip_card) Destroy(_tooltip_card);
        }
    }
}
