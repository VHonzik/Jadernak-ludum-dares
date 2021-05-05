using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CardGame
{
    class Supply : MonoBehaviour
    {
        public bool IsAvailable { get; set; }

        private Texture2D _texture;
        private static string Texture_name = "InfoSupply";

        private static Quaternion Top = Quaternion.Euler(new Vector3(0, 0, 0));
        private static Quaternion Bottom = Quaternion.Euler(new Vector3(0, 0, 180));

        private float _t;

        private GameObject _top;

        private Vector3 _start_flip_position;
        private Vector3 _goal_flip_position;

        void Awake()
        {
            _texture = Resources.Load("Cards/Textures/" + Texture_name) as Texture2D;
            _top = transform.FindChild("Top").gameObject;
            _top.GetComponent<Renderer>().material.SetTexture("_MainTex", _texture);
            IsAvailable = false;

            transform.rotation = Top;
            _t = 1.0f;

            _start_flip_position = Vector3.zero;
            _goal_flip_position = Vector3.zero;
        }

        void Update()
        {
            if(_t < 1.0f)
            {
                _t += 2.0f * Time.deltaTime;
                if(_t >= 1.0f)
                {
                    _t = 1.0f;
                }

                Quaternion wanted_rot;
                if (IsAvailable) { wanted_rot = Quaternion.Lerp(Bottom, Top, _t); }
                else { wanted_rot = Quaternion.Lerp(Top, Bottom, _t); }
                transform.rotation = wanted_rot;

                Vector3 wanted_pos = Vector3.Lerp(_start_flip_position, _goal_flip_position, (0.5f - Mathf.Abs(_t - 0.5f)) * 2.0f);
                transform.position = wanted_pos;
            }
        }

        public static Supply Create()
        {
            GameObject go = Instantiate(Resources.Load("Supply") as GameObject);
            return go.AddComponent<Supply>();
        }

        private void Flip()
        {
            _t = 0.0f;

            _start_flip_position = transform.position;

            Vector3 _previous_position = transform.position;
            Vector3 _camera_offset = Camera.main.transform.position;
            Vector3 _direction = (_camera_offset - _previous_position).normalized;
            float t = 1.0f / _direction.y;
            float _wanted_change_y = 0.222f;

            _goal_flip_position = _previous_position + _wanted_change_y * t * _direction;
        }

        public void Pay()
        {
            IsAvailable = false;
            Flip(); 
            
        }

        public void StartTurn()
        {
            if(!IsAvailable)
            {
                IsAvailable = true;
                Flip();
            }
        }

    }
}
