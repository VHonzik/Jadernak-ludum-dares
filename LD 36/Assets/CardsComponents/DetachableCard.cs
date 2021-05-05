using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CardGame.CardComponents
{
    //[RequireComponent(typeof(Card))]
    public class DetachableCard : MonoBehaviour
    {
        private bool _detached;
        public bool Detached { get { return _detached; } set {
                if (value != _detached && value == false)
                {
                    card_component.GetDetachHandle().transform.localPosition = Vector3.zero;
                    card_component.GetDetachHandle().transform.localRotation = Quaternion.identity;
                }
                _detached = value;
            } }
        public Vector3 Detached_position { get; set; }
        public Quaternion Detach_orientation { get; set; }

        private Card card_component;

        void Awake()
        {
            card_component = GetComponent<Card>();
        }

        void Update()
        {
            if (Detached)
            {
                card_component.GetDetachHandle().transform.localPosition = Quaternion.Inverse(transform.rotation) * (Detached_position - transform.position);
                card_component.GetDetachHandle().transform.localRotation = Quaternion.Inverse(transform.rotation) * Detach_orientation;
            }
            else
            {
                card_component.GetDetachHandle().transform.localPosition = Vector3.zero;
                card_component.GetDetachHandle().transform.localRotation = Quaternion.identity;
            }
        }
    }
}
