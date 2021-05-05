using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CardGame.CardComponents
{
    //[RequireComponent(typeof(Card), typeof(DetachableCard))]
    class HighlightableCard : MonoBehaviour
    {
        public bool Highlighted { get; set; }

        private DetachableCard _detach_card;
        private DraggeableCard _drag_card;
        private MouseOverableCard _mouse_card;

        void Awake()
        {
            Highlighted = false;

            _detach_card = GetComponent<DetachableCard>();
            _drag_card = GetComponent<DraggeableCard>();
            _mouse_card = GetComponent<MouseOverableCard>();
        }

        void Update()
        {
            if (_mouse_card.MouseOver && !Highlighted)
            {
                Highlight(true);
            }

            if (!_mouse_card.MouseOver && Highlighted 
                && (!_drag_card || !_drag_card.Dragging))
            {
                Highlight(false);
            }
        }

        void OnDisable()
        {
            if(Highlighted) Highlight(false);
        }

        public void Highlight(bool highlight)
        {
            Highlighted = highlight;
            _detach_card.Detached = highlight;

            if (highlight)
            {
                Vector3 _previous_position = transform.position;
                Vector3 _camera_offset = Camera.main.transform.position;
                Vector3 _direction = (_camera_offset - _previous_position).normalized;
                float _t = 1.0f / _direction.y;
                float _wanted_change_y = 0.5f;

                if (GetComponent<Card>().In_hand)
                {
                    _wanted_change_y = 2.5f;
                }

                Vector3 _wanted_position = _previous_position + _wanted_change_y * _t * _direction;
                if(GetComponent<Card>().In_hand)
                {
                    _wanted_position.z = -0.6f;
                    _wanted_position.y = 7f;
                }

                _detach_card.Detached_position = _wanted_position;
                _detach_card.Detach_orientation = Card.Flipped_On;
            }
        }
    }
}
