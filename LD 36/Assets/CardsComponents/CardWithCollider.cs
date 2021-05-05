using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CardGame.CardComponents
{
    //[RequireComponent(typeof(Card))]
    class CardWithCollider : MonoBehaviour
    {
        //private Mesh _card_mesh;
        private Mesh _minion_mesh;
        private Card _card;

        void Awake()
        {
            _card = GetComponent<Card>();
            //_card_mesh = (Resources.Load("Cards/card") as GameObject).GetComponent<MeshFilter>().sharedMesh;
            _minion_mesh = (Resources.Load("Cards/portrait") as GameObject).GetComponent<MeshFilter>().sharedMesh;
        }

        public void MakeIntoMinion()
        {
            _card.GetComponent<MeshCollider>().sharedMesh = _minion_mesh;
        }
    }
}
