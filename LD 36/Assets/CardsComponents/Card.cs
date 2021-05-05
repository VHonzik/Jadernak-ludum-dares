using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CardGame.CardComponents
{
    public class Card : MonoBehaviour
    {
        
        public static float Height = (0.075f + 0.118f) * 0.125f; // Card + Portrait
        public static float StockHeight = 0.075f * 0.125f;

        public static Quaternion Flipped_On = Quaternion.Euler(-90, 90, 90);
        public static Quaternion Flipped_Off = Quaternion.Euler(90, 90, 90);

        public bool In_hand { get; set; }
        public bool On_board { get; set; }
        public bool ManipulationEnabled { get; set; }

        private bool _player_owned;
        public bool Player_owned { get { return _player_owned; } set {
                _player_owned = value;
                if (GetComponent<PhysicalCard>()) GetComponent<PhysicalCard>().SetOwner(value);
            } }

        private GameObject _detach_handle;

        public CardDefinition Definition { get; set; }

        public string Name { get; set; }

        void Awake()
        {
            In_hand = false;
            On_board = false;
            gameObject.transform.rotation = Flipped_Off;
            _detach_handle = transform.GetChild(0).gameObject;
            ManipulationEnabled = false;
        }

        public GameObject GetDetachHandle()
        {
            return _detach_handle;
        }

        public IEnumerator SetIsInHandWrapper(bool value)
        {
            In_hand = value;
            yield break;
        }

        public void ManipulationEnable(bool value)
        {
            ManipulationEnabled = value;

            // MinionCanAttack has it's own highlighting
            if (GetComponent<AttackCapableMinion>())
            {
                GetComponent<AttackCapableMinion>().enabled = value;
                if (GetComponent<HighlightableCard>()) GetComponent<HighlightableCard>().enabled = false;
                if (GetComponent<DraggeableCard>()) GetComponent<DraggeableCard>().enabled = false;
            }
            else
            {
                if (GetComponent<HighlightableCard>()) GetComponent<HighlightableCard>().enabled = value;
                if (GetComponent<DraggeableCard>()) GetComponent<DraggeableCard>().enabled = value;
            }

        }

        public void InspectionEnable(bool value)
        {
            if (GetComponent<InspectableMinion>()) GetComponent<InspectableMinion>().enabled = value;
        }

        public IEnumerator Destroy()
        {
            Destroy(gameObject);
            yield break;
        }


    }
}
