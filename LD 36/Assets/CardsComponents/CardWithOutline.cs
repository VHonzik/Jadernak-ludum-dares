using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CardGame.CardComponents
{
    class CardWithOutline : MonoBehaviour
    {
        private Color _wanted_color;
        private Color _original_color;
        private float _t;

        private List<Tuple<Component, Color>> _requests;

        private GameObject _outline_go;

        private static string GO_Name = "PhysicalCard";
        private static string Outline_GO_Name = "Outline";

        private static Color _hide_color = new Color(1, 1, 1, 0);
        public static Color Green_outline_color = new Color(0.3929f, 0.6034f, 0.2710f);

        private Card _card;

        void Awake()
        {
            _card = GetComponent<Card>();
            GameObject physical_card = _card.GetDetachHandle().transform.FindChild(GO_Name).gameObject;
            _outline_go = physical_card.transform.FindChild(Outline_GO_Name).gameObject;

            _outline_go.GetComponent<Renderer>().material.color = _hide_color;
            _wanted_color = _hide_color;
            _original_color = _hide_color;
            _t = 1.0f;

            _requests = new List<Tuple<Component, Color>>();
        }

        void Update()
        {
            if(_requests.Count > 0)
            {
                if(_requests[0].Second != _wanted_color)
                {
                    if (_wanted_color == _hide_color && _original_color == _hide_color)
                    {
                        _original_color = new Color(_requests[0].Second.r, _requests[0].Second.g, _requests[0].Second.b, 0.0f);
                    }
                    else
                    {
                        _original_color = _outline_go.GetComponent<Renderer>().material.color;
                    }

                    _wanted_color = _requests[0].Second;
                    _t = 0.0f;
                }
            }
            else if (_wanted_color.a > 0.0f)
            {
                _original_color = _outline_go.GetComponent<Renderer>().material.color;
                _wanted_color = new Color(_original_color.r, _original_color.g, _original_color.b, 0.0f);
                _t = 0.0f;
            }

            if (_t < 1.0f)
            {
                _t += Time.deltaTime * 4.0f;
                _outline_go.GetComponent<Renderer>().material.color = Color.Lerp(_original_color, _wanted_color, _t);
            }
        }

        public void RequestOutline(Component who, Color color)
        {
            if(_requests.Find(x => x.First == who) == null)
            {
                _requests.Add(new Tuple<Component, Color>(who, color));
            }            
        }

        public void RequestOutlineHide(Component who)
        {
            int index = _requests.FindIndex(x => x.First == who);
            if(index >=0 && index < _requests.Count) _requests.RemoveAt(index);
        }

        public void MakeIntoMinion()
        {
            _outline_go.GetComponent<Renderer>().enabled = false;
        }
    }
}
