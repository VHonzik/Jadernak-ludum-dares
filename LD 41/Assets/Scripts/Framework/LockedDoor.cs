using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D))]
public class LockedDoor : MonoBehaviour
{
    public Sprite ClosedSprite;
    public Sprite OpenedSprite;

    public bool StartsClosed = true;

    public string ItemRequired;
    public string[] ItemRequiredNotFoundMessage;

    private SpriteRenderer SpriteRenderer;
    private BoxCollider2D BoxCollider2D;
    private MessageSource MessageSource;
    private Inventory Invetory;

    public bool Opened;

    // Use this for initialization
    void Start()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();
        BoxCollider2D = GetComponent<BoxCollider2D>();

        MessageSource = gameObject.AddComponent<MessageSource>();
        MessageSource.Content = ItemRequiredNotFoundMessage;

        Invetory = FindObjectOfType<Inventory>(); ;

        SpriteRenderer.sprite = StartsClosed ? ClosedSprite : OpenedSprite;
        Opened = !StartsClosed;
        BoxCollider2D.enabled = StartsClosed;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Open()
    {
        if (!Opened)
        {
            if (ItemRequired.Length > 0 && !Invetory.HasItemWithName(ItemRequired))
            {
                MessageSource.Trigger();
            }
            else
            {
                Opened = true;
                BoxCollider2D.enabled = false;
                SpriteRenderer.sprite = OpenedSprite;
                Invetory.RemoveItemWithName(ItemRequired);
            }
        }
    }

    public void Close()
    {
        if (Opened)
        {
            Opened = false;
            BoxCollider2D.enabled = true;
            SpriteRenderer.sprite = ClosedSprite;
        }
    }
}
