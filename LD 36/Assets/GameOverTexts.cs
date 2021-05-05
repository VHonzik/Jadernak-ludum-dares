using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;


namespace CardGame
{
    class GameOverTexts : MonoBehaviour
    {
        void Awake()
        {
            transform.FindChild("Result").gameObject.GetComponent<TextMesh>().text =
                GlobalObject.Instance.Won ? "You did it!" : "The ancient evil has prevailed...";

            if(GlobalObject.Instance.Won)
            {
                transform.FindChild("Tip").gameObject.GetComponent<Renderer>().enabled = false;
            }
        }
    }
}
