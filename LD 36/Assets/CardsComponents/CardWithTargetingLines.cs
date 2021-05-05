using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CardGame.CardComponents
{
    class CardWithTargetingLines : MonoBehaviour
    {
        private LineRenderer _lines;
        private Vector3[] _lines_position;
        public bool Enabled { get; set; }

        void Awake()
        {
            _lines = GetComponent<LineRenderer>();
            if (!_lines) _lines = gameObject.AddComponent<LineRenderer>();

            _lines.material = Resources.Load("Cards/Materials/TargetLine") as Material;
            _lines.SetColors(Color.red, Color.red);
            _lines.SetWidth(0.2F, 0.1F);
            Enabled = false;
            _lines.SetVertexCount(2);
            _lines_position = new Vector3[2] { transform.position, transform.position };
        }

        void Update()
        {
            _lines.enabled = Enabled;
            if(Enabled)
            {
                _lines.SetPositions(_lines_position);
            }
        }

        public void SetStartPosition(Vector3 start)
        {
            _lines_position[0] = start;
        }

        public void SetEndPosition(Vector3 end)
        {
            _lines_position[1] = end;
        }
    }
}
