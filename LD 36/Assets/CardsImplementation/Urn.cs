using CardGame;
using CardGame.CardComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Cards
{
    class Urn : MonoBehaviour
    {
        private Card _card;
        private CardTargetedSpell _spell_targeted;

        void Awake()
        {
            _card = GetComponent<Card>();
            _spell_targeted = _card.GetComponent<CardTargetedSpell>();
            _spell_targeted.ECast += new CardTargetedSpell.CastHandler(OnCast);
            _spell_targeted.TargetPredicate = x => x.GetComponent<Card>().Player_owned != _card.Player_owned;
        }

        private void OnCast(CardMinion target)
        {
            if (target)
            {
                GameManager.GetInstance().Game_Queue.DealDamage(target, 3, _card, true);
            }
        }
    }
}
