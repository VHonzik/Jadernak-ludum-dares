using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CardGame.CardComponents
{
    class InfoText : MonoBehaviour
    {
        private TextMesh _text;

        private static int FontSizeOneDigit = 33;
        private static int FontSizeMoreDigits = 24;

        private static Color Default_Color = Color.black;
        private static Color Warning_Color = new Color(0.6627f, 0.2117f, 0.2117f);
        private static Color Bonus_color = new Color(0.3929f, 0.6034f, 0.2710f);

        void Awake()
        {
            _text = GetComponent<TextMesh>();
        }

        public void SetText(int value)
        {
            _text.text = value.ToString();
            if(value < 0)
            {
                _text.fontSize = FontSizeMoreDigits;
            }
            else if(value / 10 > 0)
            {
                _text.fontSize = FontSizeMoreDigits;
            }
            else {
                _text.fontSize = FontSizeOneDigit;
            }
        }

        public void Warning()
        {
            _text.GetComponent<Renderer>().material.color = Warning_Color;
        }

        public void Default()
        {
            _text.GetComponent<Renderer>().material.color = Default_Color;
        }

        public void Bonus()
        {
            _text.GetComponent<Renderer>().material.color = Bonus_color;
        }

        
    }
}
