using UnityEngine;
using System.Collections;

public class InventoryItem : MonoBehaviour
{
    public int Uses;

    public string[] UseMessage;

    private MessageSource MessageSource;

    void Start()
    {
        MessageSource = gameObject.AddComponent<MessageSource>();
        MessageSource.Content = UseMessage;
    }

    public bool Use()
    {
        if (UseMessage.Length > 0)
        {
            MessageSource.Trigger();
        }

        if (Uses == -1)
        {
            return false;
        }
        else
        {
            Uses -= 1;
            return Uses == 0;
        }
    }

}
