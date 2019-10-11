using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Exit : MonoBehaviour
{
    private Image Background;
    private List<Text> Texts;
    private bool Visible;

    // Use this for initialization
    void Start()
    {
        Background = GetComponent<Image>();
        Background.enabled = false;
        Texts = new List<Text>(GetComponentsInChildren<Text>());
        foreach(var text in Texts)
        {
            text.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Visible && Input.anyKeyDown)
        {
            if (Input.GetButtonDown("InspectItem"))
            {
                Application.Quit();
            }
            else
            {
                Background.enabled = false;
                foreach (var text in Texts)
                {
                    text.enabled = false;
                }
                Visible = false;
            }
        }

        if (Input.GetButtonDown("Exit"))
        {
            Background.enabled = true;
            foreach (var text in Texts)
            {
                text.enabled = true;
            }

            Visible = true;
        }
    }
}
