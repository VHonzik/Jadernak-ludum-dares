using CardGame;
using CardGame.CardComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Cards
{
    //[RequireComponent(typeof(Card))]
    class AncientArtifact : MonoBehaviour
    {
        private Card _card;

        void Awake()
        {
            _card = GetComponent<Card>();
            _card.GetComponent<CardSpell>().ECast += new CardSpell.CastHandler(OnCast);
        }

        private void OnCast()
        {
            Card gate = GameManager.GetInstance().Enemy_board.Cards.Find(x => x.Name == "ancientgate");
            if (gate) GameManager.GetInstance().Game_Queue.Destroy(gate.GetComponent<CardMinion>(), _card);

            gate = GameManager.GetInstance().Player_board.Cards.Find(x => x.Name == "ancientgate");
            if (gate) GameManager.GetInstance().Game_Queue.Destroy(gate.GetComponent<CardMinion>(), _card);
        }
    }
}
