using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CardGame
{
    public class Supplies : MonoBehaviour
    {
        private List<Supply> Supplies_List { get; set; }
        private static float Space = 0.6f;
        public bool Player_Owned { get; set; }

        private static int MaxSupplies = 10;

        void Awake()
        {
            Supplies_List = new List<Supply>();
            Player_Owned = true;            
        }

        public void Create()
        {
            CreateBackgrounds();
        }

        private void CreateBackgrounds()
        {
            for(int i = 0; i < MaxSupplies; i++)
            {
                GameObject go = Instantiate(Resources.Load("SupplyBackground") as GameObject);
                go.transform.parent = transform;
                go.transform.localPosition =  new Vector3((Player_Owned ? -1.0f : 1.0f) * i * Space, 0, 0);
            }
        }

        public int SuppliesAvailable()
        {
            return Supplies_List.FindAll(x => x.IsAvailable == true).Count;
        }

        private void CreateSupplyIcon()
        {
            Supply supply = Supply.Create();
            Supplies_List.Add(supply);
            supply.transform.parent = transform;
            supply.transform.localPosition =  new Vector3((Player_Owned ? -1.0f : 1.0f) * (Supplies_List.Count - 1) * Space, 0, 0);
        }

        public IEnumerator AddPernament(int count)
        {
            for(int i=0; i < count; i++)
            {
                CreateSupplyIcon();
            }
            yield break;
        }

        public IEnumerator StartTurn()
        {
            foreach (var supply in Supplies_List)
            {
                supply.StartTurn();
            }

            yield break;
        }

        public void Pay(int count)
        {
            foreach(var supply in Supplies_List.FindAll(x => x.IsAvailable == true).Take(count))
            {
                supply.Pay();
            }            
        }
    }
}
