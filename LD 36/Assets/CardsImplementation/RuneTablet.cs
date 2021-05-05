using CardGame;
using CardGame.CardComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Cards
{
    class RuneTablet : MonoBehaviour
    {
        private Card _card;

        void Awake()
        {
            _card = GetComponent<Card>();
            _card.GetComponent<CardSpell>().ECast += new CardSpell.CastHandler(OnCast);
        }

        private void OnCast()
        {
            Hand hand = _card.Player_owned ? GameManager.GetInstance().Player_hand : GameManager.GetInstance().Enemy_hand;
            Stock stock = _card.Player_owned ? GameManager.GetInstance().Player_stock : GameManager.GetInstance().Enemy_stock;
            Card artifact = hand.Cards.Find(x => x.Name == "ancientartifact");
            if (artifact) artifact.GetComponent<CardWithCost>().Current_cost -= 10;

            artifact = stock.Cards.Find(x => x.Name == "ancientartifact");
            if (artifact) artifact.GetComponent<CardWithCost>().Current_cost -= 10;
        }
    }
}
