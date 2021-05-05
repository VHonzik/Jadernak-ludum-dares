using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CardGame.CardComponents
{
    class DiscoverableCard : MonoBehaviour
    {
        private Card _card;
        private MoveableCard _moveable_card;
        private CardWithOutline _outline_card;

        public bool Discovering { get; set; }
        public bool Chosen { get; set; }

        private Vector3 _deck_position;

        void Awake()
        {
            _card = GetComponent<Card>();
            _moveable_card = GetComponent<MoveableCard>();
            _outline_card = GetComponent<CardWithOutline>();
            Chosen = false;
        }

        void Update()
        {
            if(Discovering && GetComponent<MouseOverableCard>().MouseOver && Input.GetMouseButtonDown(0))
            {
                Discovering = false;
                Chosen = true;

                if(_card.Player_owned)
                {
                    GameManager.GetInstance().Player_stock.Draw(_card);
                    GameManager.GetInstance().Player_hand.Add(_card);
                    _card.ManipulationEnable(true);
                }
                else
                {
                    GameManager.GetInstance().Enemy_stock.Draw(_card);
                    GameManager.GetInstance().Enemy_hand.Add(_card);
                }

                _outline_card.RequestOutlineHide(this);
            }
        }

        public IEnumerator Discover(Vector3 wanted_pos)
        {
            _deck_position = transform.position;

            _moveable_card.Flip(false).Move(wanted_pos, false);

            WaitForCallback<MoveableCard> helper = new WaitForCallback<MoveableCard>(_moveable_card.MovementArbitraryCoroutine);
            yield return StartCoroutine(helper.Do());

            Discovering = true;
            Chosen = false;
            _outline_card.RequestOutline(this, CardWithOutline.Green_outline_color);
            
        }

        public IEnumerator ReturnToDeck()
        {
            _moveable_card.Flip(false).Move(_deck_position, false);

            WaitForCallback<MoveableCard> helper = new WaitForCallback<MoveableCard>(_moveable_card.MovementArbitraryCoroutine);
            yield return StartCoroutine(helper.Do());
            _outline_card.RequestOutlineHide(this);
            GetComponent<HighlightableCard>().enabled = false;

        }
    }
}
