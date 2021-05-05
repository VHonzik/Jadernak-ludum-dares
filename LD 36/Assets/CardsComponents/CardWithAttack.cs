using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CardGame.CardComponents
{
    //[RequireComponent(typeof(Card))]
    class CardWithAttack : MonoBehaviour
    {
        private GameObject _attack_go;
        private GameObject _background;
        private InfoText _attack_text;

        private static Vector3 Relative_Position_Card = new Vector3(0.439f, -0.677f, 0.01f);
        private static Vector3 Relative_Position_Minion = new Vector3(0.235f, -0.334f, 0.024f);

        private Texture2D _texture;
        private static string Texture_name = "InfoAttack";

        private static string GO_Name = "Attack";
        private static string Background_Name = "Model";

        private Card _card;

        private int _original_attack;

        private int _current_attack;
        public int Current_attack { get { return _current_attack; } set {
                _current_attack = value;
                _attack_text.SetText(_current_attack);
                if (_current_attack <= _original_attack)
                {
                    _attack_text.Default();
                }
                else
                {
                    _attack_text.Bonus();
                }
            } }

        void Awake()
        {
            
            _card = GetComponent<Card>();
            _attack_go = _card.GetDetachHandle().transform.FindChild(GO_Name).gameObject;
            if (_attack_go)
            {
                _attack_text = _attack_go.GetComponent<InfoText>();
                _background = _attack_go.transform.FindChild(Background_Name).gameObject;
            }
        }

        public void Create(GameObject detach_handle)
        {
            _texture = Resources.Load("Cards/Textures/" + Texture_name) as Texture2D;

            _attack_go = GameObject.Instantiate(Resources.Load("InfoCircle") as GameObject);
            _attack_go.name = GO_Name;
            _attack_go.transform.parent = detach_handle.transform;
            _attack_go.transform.localPosition = Relative_Position_Card;
            _attack_text = _attack_go.AddComponent<InfoText>();
            _background = _attack_go.transform.FindChild(Background_Name).gameObject;

            _background.GetComponent<Renderer>().material.SetTexture("_MainTex", _texture);
        }

        public void Init()
        {
            _original_attack = _card.Definition.Attack;
            _attack_text.SetText(_original_attack);
            Current_attack = _original_attack;
        }

        public void MakeIntoMinion()
        {
            _attack_go.transform.localPosition = Relative_Position_Minion;
        }

        public void AddPernament(int amount)
        {
            Current_attack += amount;
        }
    }
}
