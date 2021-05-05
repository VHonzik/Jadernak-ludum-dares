using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CardGame.CardComponents
{
    //[RequireComponent(typeof(Card))]
    class CardWithPortrait : MonoBehaviour
    {
        private GameObject _portrait_go;
        private GameObject _outline_go;

        private static Vector3 Relative_Position_Card = new Vector3(0, 0.264f, 0);
        private static Vector3 Relative_Position_Minion = Vector3.zero;

        private static string GO_Name = "Portrait";
        private static string Outline_GO_Name = "Outline";

        private static Color _hide_color = new Color(1, 1, 1, 0);

        private Color _wanted_color;
        private Color _original_color;
        private float _t;

        private Card _card;

        void Awake()
        {
            _card = GetComponent<Card>();
            _portrait_go = _card.GetDetachHandle().transform.FindChild(GO_Name).gameObject;
            _outline_go = _portrait_go.transform.FindChild(Outline_GO_Name).gameObject;

            _outline_go.GetComponent<Renderer>().material.color = _hide_color;
            _wanted_color = _hide_color;
            _original_color = _hide_color;
            _t = 1.0f;
        }

        void Update()
        {
            if(_t < 1.0f)
            {
                _t += Time.deltaTime * 4.0f;
                _outline_go.GetComponent<Renderer>().material.color = Color.Lerp(_original_color, _wanted_color, _t);
            }
        }

        public void Create(GameObject detach_handle, CardDefinition definition)
        {
            _portrait_go = GameObject.Instantiate(Resources.Load("Portrait") as GameObject);
            _portrait_go.name = GO_Name;
            _portrait_go.transform.parent = detach_handle.transform;
            _portrait_go.transform.localPosition = Relative_Position_Card;
            _portrait_go.GetComponent<Renderer>().material.SetTexture("_MainTex", CardsTextureMaker.CreatePortrait(definition));
        }

        public void MakeIntoMinion()
        {
            _portrait_go.transform.localPosition = Relative_Position_Minion;
        }

        public void Hide()
        {
            _portrait_go.GetComponent<Renderer>().enabled = false;
        }

        public void ShowOutline(Color wanted_color)
        {
            if(_wanted_color == _hide_color && _original_color == _hide_color)
            {
                _original_color = new Color(wanted_color.r, wanted_color.g, wanted_color.b, 0.0f);
            }
            else
            {
                _original_color = _outline_go.GetComponent<Renderer>().material.color;
            }
            
            _wanted_color = wanted_color;
            _t = 0.0f;
        }

        public void HideOutline()
        {
            _original_color = _outline_go.GetComponent<Renderer>().material.color;
            _wanted_color = new Color(_original_color.r, _original_color.g, _original_color.b, 0.0f);
            _t = 0.0f;
        }
    }
}
