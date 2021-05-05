using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CardGame.CardComponents
{
    class MouseOverableCard : MonoBehaviour
    {
        public bool MouseOver { get; set; }
        public bool MouseOverLastFrame { get; set; }


        void Awake()
        {
            MouseOver = false;
            MouseOverLastFrame = false;
        }

        void Update()
        {
            MouseOverLastFrame = MouseOver;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                MouseOver = hit.transform.gameObject == gameObject;
            }
            else
            {
                MouseOver = false;
            }
        }
    }
}
