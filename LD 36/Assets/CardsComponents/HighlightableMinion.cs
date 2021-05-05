using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CardGame.CardComponents
{
    class HighlightableMinion : MonoBehaviour
    {
        private DetachableCard _detach_card;
        private CardWithPortrait _portrait_card;

        private static Color Green_outline_color = new Color(0.3929f, 0.6034f, 0.2710f);
        private static Color Red_outline_color = new Color(0.6627f, 0.2117f, 0.2117f);

        void Awake()
        {
            _detach_card = GetComponent<DetachableCard>();
            _portrait_card = GetComponent<CardWithPortrait>();
        }

        public void Highlight(bool highlight)
        {
            _detach_card.Detached = highlight;

            if (highlight)
            {
                Vector3 _previous_position = transform.position;
                Vector3 _camera_offset = Camera.main.transform.position;
                Vector3 _direction = (_camera_offset - _previous_position).normalized;
                float _t = 1.0f / _direction.y;
                float _wanted_change_y = 1.0f;
                Vector3 _wanted_position = _previous_position + _wanted_change_y * _t * _direction;

                _detach_card.Detached_position = _wanted_position;
                _detach_card.Detach_orientation = Card.Flipped_On;
            }
        }

        public void ShowGreenOutline(bool show)
        {
            if(show)
            {
                _portrait_card.ShowOutline(Green_outline_color);
            }
            else
            {
                _portrait_card.HideOutline();
            }
        }

        public void ShowRedOutline(bool show)
        {
            if (show)
            {
                _portrait_card.ShowOutline(Red_outline_color);
            }
            else
            {
                _portrait_card.HideOutline();
            }
        }
    }
}
