using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class PickableItem : MonoBehaviour
{
    private Inventory Inventory;
    private CharacterMovement CharacterMovement;
    private MessageSource MessageSource;
    public string[] PickUpMessage;

    void Start()
    {
        CharacterMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterMovement>();
        Inventory = FindObjectOfType<Inventory>();

        MessageSource = gameObject.AddComponent<MessageSource>();

        CharacterMovement.RegisterInteractable(gameObject, Pickup);
    }

    public void Pickup()
    {
        Inventory.AddItem(gameObject);
        if (PickUpMessage.Length > 0)
        {
            MessageSource.Content = PickUpMessage;
            MessageSource.Trigger();
        }
    }
}
