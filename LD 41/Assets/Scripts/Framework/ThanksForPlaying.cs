using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ThanksForPlaying : MonoBehaviour
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
        foreach (var text in Texts)
        {
            text.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Visible && Input.anyKeyDown)
        {
            Application.Quit();
        }
    }

    public void Show()
    {
        Background.enabled = true;
        foreach (var text in Texts)
        {
            text.enabled = true;
        }

        Visible = true;
    }
}
