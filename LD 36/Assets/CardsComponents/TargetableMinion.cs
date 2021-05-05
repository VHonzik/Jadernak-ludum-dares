using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CardGame.CardComponents
{
    class TargetableMinion : MonoBehaviour
    {
        static bool SomeoneIsTargeting { get; set; }

        private List<Card> _disable_targeting_statuses;

        private bool _can_be_targeted;
        public bool CanBeTargeted { get { return _can_be_targeted; } private set {
                if (!value) Reset();
                _can_be_targeted = value;
            } }

        private HighlightableMinion _highlight_minion;

        void Awake()
        {
            CanBeTargeted = true;
            _disable_targeting_statuses = new List<Card>();
            _highlight_minion = GetComponent<HighlightableMinion>();
        }

        void Update()
        {  
            CanBeTargeted = _disable_targeting_statuses.Count <= 0;
        }

        public void Target()
        {
            if(CanBeTargeted)
            {
                _highlight_minion.Highlight(true);
                _highlight_minion.ShowRedOutline(true);
            }
        }

        public void Reset()
        {
            _highlight_minion.Highlight(false);
            _highlight_minion.ShowRedOutline(false);
        }

        public void DisableTargetingStatus(Card source)
        {
            _disable_targeting_statuses.Add(source);
        }

        public void DisableTargetingStatusExit(Card source)
        {
            if(source)
            {
                _disable_targeting_statuses.Remove(source);
            }
        }
    }
}
