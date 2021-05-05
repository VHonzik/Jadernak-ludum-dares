using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CardGame.CardComponents
{
    //[RequireComponent(typeof(Card))]
    class CardWithHP : MonoBehaviour
    {
        private GameObject _hp_go;
        private GameObject _background;
        private InfoText _hp_text;

        private static Vector3 Relative_Position_Card = new Vector3(-0.436f, -0.677f, 0.01f);
        private static Vector3 Relative_Position_Minion = new Vector3(-0.235f, -0.334f, 0.024f);

        private Texture2D _texture;
        private static string Texture_name = "InfoHP";

        private static string GO_Name = "HP";
        private static string Background_Name = "Model";

        private Card _card;

        private int _original_hp;
        private int _base_hp;

        private int _current_hp;
        public int Current_hp { get { return _current_hp; } set {
                _current_hp = value;
                _hp_text.SetText(_current_hp);

                if (_current_hp < _base_hp)
                {
                    _hp_text.Warning();
                }
                else
                {
                    _current_hp = _base_hp;
                    if(_base_hp == _original_hp)
                    {
                        _hp_text.Default();
                    }
                    else
                    {
                        _hp_text.Bonus();
                    }
                    
                }
            } }

        void Awake()
        {
            _card = GetComponent<Card>();
            _hp_go = _card.GetDetachHandle().transform.FindChild(GO_Name).gameObject;
            if (_hp_go)
            {
                _hp_text = _hp_go.GetComponent<InfoText>();
                _background = _hp_go.transform.FindChild(Background_Name).gameObject;
            }
        }

        public void Create(GameObject detach_handle)
        {
            _texture = Resources.Load("Cards/Textures/" + Texture_name) as Texture2D;

            _hp_go = GameObject.Instantiate(Resources.Load("InfoCircle") as GameObject);
            _hp_go.name = GO_Name;
            _hp_go.transform.parent = detach_handle.transform;
            _hp_go.transform.localPosition = Relative_Position_Card;
            _hp_text = _hp_go.AddComponent<InfoText>();
            _background = _hp_go.transform.FindChild(Background_Name).gameObject;

            _background.GetComponent<Renderer>().material.SetTexture("_MainTex", _texture);
        }

        public void Init()
        {
            _base_hp = _card.Definition.Health;
            _original_hp = _base_hp;
            Current_hp = _base_hp;
            _hp_text.SetText(_base_hp);
        }

        public void MakeIntoMinion()
        {
            _hp_go.transform.localPosition = Relative_Position_Minion;
        }

        public void AddPernament(int amount)
        {
            _base_hp += amount;
            Current_hp += amount;
        }
    }
}
