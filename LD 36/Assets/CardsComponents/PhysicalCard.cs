using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CardGame.CardComponents
{
    //[RequireComponent(typeof(Card), typeof(CardWithCost))]
    class PhysicalCard : MonoBehaviour
    {
        private GameObject _physical_card;
        private GameObject _description_go;
        private GameObject _title_go;      

        private static string GO_Name = "PhysicalCard";
        private static string Title_GO_Name = "Title";
        private static string Description_GO_Name = "Description";


        private Texture2D Player_Bottom;
        private Texture2D Enemy_Bottom;

        private Card _card;

        void Awake()
        {
            _card = GetComponent<Card>();

            _physical_card = _card.GetDetachHandle().transform.FindChild(GO_Name).gameObject;
            _description_go = _card.GetDetachHandle().transform.FindChild(Description_GO_Name).gameObject;
            _title_go = _card.GetDetachHandle().transform.FindChild(Title_GO_Name).gameObject;

            Player_Bottom = Resources.Load("Cards/Textures/BottomPlayer") as Texture2D;
            Enemy_Bottom = Resources.Load("Cards/Textures/BottomEnemy") as Texture2D;
  
        }

        public void Create(GameObject detach_handle, CardDefinition definition)
        {
            _physical_card = GameObject.Instantiate(Resources.Load(GO_Name) as GameObject);
            _physical_card.name = GO_Name;
            _physical_card.transform.parent = detach_handle.transform;
            _physical_card.transform.localPosition = Vector3.zero;

            foreach (var mat in _physical_card.GetComponent<Renderer>().materials)
            {
                if (mat.name.StartsWith("card_top"))
                {
                    mat.mainTexture = CardsTextureMaker.CreateTexture(definition.Type == CardType.Minion,
                        definition.Type == CardType.Minion);
                }
            }

            _description_go = detach_handle.transform.FindChild(Description_GO_Name).gameObject;
            _title_go = detach_handle.transform.FindChild(Title_GO_Name).gameObject;

            _title_go.GetComponent<TextMesh>().text = definition.Title;
            _description_go.GetComponent<TextMesh>().text = definition.Description;

        }

        public void SetOwner(bool Player_owned)
        {
            foreach (var mat in _physical_card.GetComponent<Renderer>().materials)
            {
                if (mat.name.StartsWith("card_bottom"))
                {
                    mat.mainTexture = Player_owned ? Player_Bottom : Enemy_Bottom;
                }
            }
        }    

        public IEnumerator MakeIntoMinion()
        {
            GetComponent<CardWithOutline>().MakeIntoMinion();
            _title_go.GetComponent<Renderer>().enabled = false;
            _description_go.GetComponent<Renderer>().enabled = false;

            float _t = 0.0f;
            GameObject burning_card = GameObject.Instantiate(Resources.Load("BurnedCard") as GameObject);
            burning_card.GetComponent<Renderer>().material.mainTexture = _physical_card.GetComponent<Renderer>().materials[1].mainTexture;
            Vector3 prev_pos = burning_card.transform.position = _physical_card.transform.position;
            _physical_card.GetComponent<Renderer>().enabled = false;
            burning_card.GetComponent<Renderer>().material.SetFloat("_Control", 0);

            _t = 0.0f;

            while (_t <= 1.0f)
            {
                burning_card.GetComponent<Renderer>().material.SetFloat("_Control", _t);

                if(_t >= 0.1f)
                {
                    _title_go.GetComponent<Renderer>().enabled = false;
                    _description_go.GetComponent<Renderer>().enabled = false;
                }

                burning_card.transform.position = Vector3.Lerp(prev_pos, prev_pos - 0.5f * Vector3.down, _t);
                _t += Time.deltaTime;
                yield return null;
            }

            burning_card.GetComponent<Renderer>().enabled = false;
            Destroy(burning_card);
        }
      

    }
}
