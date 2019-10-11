using UnityEngine;
using System.Collections;

public class InitialMessage : MonoBehaviour
{
    private MessageSource MessageSource;
    private bool Triggered;

    // Use this for initialization
    void Start()
    {
        MessageSource = gameObject.AddComponent<MessageSource>();
        MessageSource.Content = new string[] {
            "As you descend into into a small rectangular room, you are hit with familiar musty smell of a dungeon.",
            "You can't wait to see what adventures lay ahead of you.",
            "...",
            "Maybe you will even meet a pretty, smart and funny girl..."
        };
        Triggered = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!Triggered)
        {
            Triggered = true;
            MessageSource.Trigger();
        }
    }
}
