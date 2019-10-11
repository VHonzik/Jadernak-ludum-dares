using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageSource : MonoBehaviour
{
    public string[] Content;

    private TextBox Textbox;

    // Use this for initialization
    void Start () {
        Textbox = FindObjectOfType<TextBox>();
    }

    public void Trigger()
    {
        if (!Textbox)
        {
            Textbox = FindObjectOfType<TextBox>();
        }

        foreach (var msg in Content)
        {
            Textbox.AddMessage(msg);
        }
    }

    public IEnumerator Wait()
    {
        yield return StartCoroutine(Textbox.Wait());
    }
}
