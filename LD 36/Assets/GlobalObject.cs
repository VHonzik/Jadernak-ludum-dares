using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CardGame
{
    class GlobalObject : MonoBehaviour
    {
        static GlobalObject the_one_and_only;

        public static GlobalObject Instance
        {
            get
            {
                return the_one_and_only;
            }
        }

        public bool Won { get; set; }

        void Awake()
        {
            the_one_and_only = this;
            DontDestroyOnLoad(transform.gameObject);
        }

        void Update()
        {
            if (Input.GetKeyDown("escape"))
            {
                Application.Quit();
            }

        }
    }
}
