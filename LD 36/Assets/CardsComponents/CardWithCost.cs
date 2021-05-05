using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CardGame.CardComponents
{
    //[RequireComponent(typeof(Card))]
    class CardWithCost : MonoBehaviour
    {
        private GameObject _cost_go;
        private GameObject _background;
        private InfoText _cost_text;

        private static Vector3 Relative_Position_Card = new Vector3(0.436f, 0.638f, 0.01f);
        private static string GO_Name = "Cost";
        private static string Background_Name = "Model";

        private Texture2D _texture;
        private static string Texture_name = "InfoCost";

        private Card _card;
        private CardWithOutline _outline_card;

        private int _base_cost = 0;
        private int _current_cost;
        public int Current_cost { get {
                return _current_cost;
            } set {
                _current_cost = value;
                _cost_text.SetText(_current_cost);

                if(_current_cost < _base_cost)
                {
                    _cost_text.Bonus();
                }
                else if(_current_cost == _base_cost)
                {
                    _cost_text.Default();
                }
                else
                {
                    _cost_text.Warning();
                }

            } }

        void Awake()
        {
            _outline_card = GetComponent<CardWithOutline>();
            _texture = Resources.Load("Cards/Textures/" + Texture_name) as Texture2D;
            _card = GetComponent<Card>();
            _cost_go = _card.GetDetachHandle().transform.FindChild(GO_Name).gameObject;
            if (_cost_go)
            {
                _background = _cost_go.transform.FindChild(Background_Name).gameObject;
                _cost_text = _cost_go.GetComponent<InfoText>();
            }
        }

        void Update()
        {
            if (_card.Player_owned && _card.ManipulationEnabled && CanAfford())
            {
                _outline_card.RequestOutline(this, CardWithOutline.Green_outline_color);
            }
            else
            {
                _outline_card.RequestOutlineHide(this);
            }
        }

        public void Create(GameObject detach_handle)
        {
            _texture = Resources.Load("Cards/Textures/" + Texture_name) as Texture2D;

            _cost_go = GameObject.Instantiate(Resources.Load("InfoCircle") as GameObject);
            _cost_go.name = GO_Name;
            _cost_go.transform.parent = detach_handle.transform;
            _cost_go.transform.localPosition = Relative_Position_Card;
            _cost_text = _cost_go.AddComponent<InfoText>();
            _background = _cost_go.transform.FindChild(Background_Name).gameObject;

            _background.GetComponent<Renderer>().material.SetTexture("_MainTex", _texture);
        }

        public void Init()
        {
            _base_cost = _card.Definition.Cost;
            _cost_text.SetText(_base_cost);
            Current_cost = _base_cost;
        }

        public void Hide()
        {
            _cost_go.GetComponent<Renderer>().enabled = false;
            _background.GetComponent<Renderer>().enabled = false;
        }

        public bool CanAfford()
        {
            int supplies = _card.Player_owned ? GameManager.GetInstance().Player_Supplies.SuppliesAvailable() 
                : GameManager.GetInstance().Enemy_Supplies.SuppliesAvailable();
            return Current_cost <= supplies;
        }
    }
}
