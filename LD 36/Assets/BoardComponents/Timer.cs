using CardGame.CardComponents;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CardGame
{
    public class Timer : MonoBehaviour
    {
        private static float MaxTurnTime = 75;
        private static float MaxDisplayChanged = 150;

        private static Vector3 Default_Position = Vector3.zero;
        private static Vector3 Highlight_Position = new Vector3(0,2,0);

        private GameObject _outline_go;

        private Color _wanted_color;
        private Color _original_color;
        private float _t;

        private static Color _hide_color = new Color(1, 1, 1, 0);
        private static Color Green_outline_color = new Color(0.3929f, 0.6034f, 0.2710f);

        private static string Outline_GO_Name = "Outline";

        private bool Running { get; set; }
        private bool PlayerTurn { get; set; }
        private bool ManipulationEnabled { get; set; }
        private bool Mouse_over { get; set; }

        private float _displayed_time;
        public float RealTime { get; set; } 

        void Awake()
        {
            _displayed_time = 0;
            Mouse_over = false;
            RealTime = 0;
            Running = false;
            PlayerTurn = true;
            OrientationFromTime();

            _outline_go = transform.FindChild(Outline_GO_Name).gameObject;

            _outline_go.GetComponent<Renderer>().material.color = _hide_color;
            _wanted_color = _hide_color;
            _original_color = _hide_color;
            _t = 1.0f;
        }

        private void MouseProcess()
        {
            if (PlayerTurn && ManipulationEnabled)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit) && hit.transform.gameObject == gameObject)
                {
                    Mouse_over = true;
                }
            }
        }

        private void MaxTimeReached()
        {
            RealTime = MaxTurnTime;
            _displayed_time = MaxTurnTime;
            if (PlayerTurn)
            {
                GameManager.GetInstance().Game_Queue.EndPlayerTurn();
            }
            else
            {
                GameManager.GetInstance().Game_Queue.EndEnemyTurn();
            }
            Running = false;
        }

        private void HighlightProcess()
        {
            if(PlayerTurn && ManipulationEnabled)
            {
                bool no_playeble_card_in_hand = GameManager.GetInstance().Player_hand.Cards.Count(
                    x => x.GetComponent<CardWithCost>().CanAfford() == true
                    ) == 0;

                bool no_minion_with_attack = GameManager.GetInstance().Player_board.Cards.Count(
                    x => x.GetComponent<AttackCapableMinion>().CanAttack() == true
                    ) == 0;

                if(no_playeble_card_in_hand && no_minion_with_attack)
                {
                    ShowOutline(Green_outline_color);
                }
                else
                {
                    HideOutline();
                }
            }
            else
            {
                HideOutline();
            }

        }


        void Update()
        {
            Mouse_over = false;

            if (Running)
            {
                RealTime += Time.deltaTime;
                _displayed_time = Mathf.MoveTowards(_displayed_time, RealTime, MaxDisplayChanged * Time.deltaTime);

                if (_displayed_time >= MaxTurnTime)
                {
                    MaxTimeReached();                    
                }

                OrientationFromTime();

                MouseProcess();
                HighlightProcess();
            }

            transform.position = Mouse_over ? Highlight_Position : Default_Position;

            if (_t < 1.0f)
            {
                _t += Time.deltaTime * 4.0f;
                _outline_go.GetComponent<Renderer>().material.color = Color.Lerp(_original_color, _wanted_color, _t);
            }

            if (ManipulationEnabled && Mouse_over && Input.GetMouseButtonDown(0))
            {
                HideOutline();
                ManipulationEnable(false);
                EndTurn();
            }
        }

        private void OrientationFromTime()
        {
            float z = (PlayerTurn ? 180.0f : 0.0f) + (_displayed_time / MaxTurnTime) * 180.0f;
            gameObject.transform.rotation = Quaternion.Euler(-90, 180, z);
        }

        private void EndTurn()
        {
            RealTime = MaxTurnTime;
        }

        public IEnumerator EndTurnCo()
        {
            EndTurn();

            while (_displayed_time < RealTime)
            {
                yield return null;
            }
        }

        public IEnumerator StartPlayerTurnCo()
        {
            PlayerTurn = true;
            RealTime = 0;
            _displayed_time = 0;
            Running = true;
            yield break;
        }

        public IEnumerator StartEnemyTurnCo()
        {
            PlayerTurn = false;
            RealTime = 0;
            _displayed_time = 0;
            Running = true;
            yield break;
        }

        public void ManipulationEnable(bool value)
        {
            ManipulationEnabled = value;
        }

        public void ShowOutline(Color wanted_color)
        {
            if (wanted_color != _wanted_color)
            {
                if (_wanted_color == _hide_color && _original_color == _hide_color)
                {
                    _original_color = new Color(wanted_color.r, wanted_color.g, wanted_color.b, 0.0f);
                }
                else
                {
                    _original_color = _outline_go.GetComponent<Renderer>().material.color;
                }

                _wanted_color = wanted_color;
                _t = 0.0f;
            }
        }

        public void HideOutline()
        {
            if (_wanted_color.a > 0.0f)
            {
                _original_color = _outline_go.GetComponent<Renderer>().material.color;
                _wanted_color = new Color(_original_color.r, _original_color.g, _original_color.b, 0.0f);
                _t = 0.0f;
            }
        }
    }
}
