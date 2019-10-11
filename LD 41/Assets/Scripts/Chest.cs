using UnityEngine;
using System.Collections;

public class Chest : MonoBehaviour
{
    private MessageSource MessageSource;
    private CharacterMovement CharacterMovement;
    private Inventory Inventory;

    public GameObject Loot;

    private bool Seen;

    // Use this for initialization
    void Start()
    {
        MessageSource = gameObject.AddComponent<MessageSource>();
        CharacterMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterMovement>();
        Inventory = FindObjectOfType<Inventory>();

        CharacterMovement.RegisterInteractable(gameObject, Interact);
    }

    void Interact()
    {
        if (!Seen)
        {
            Seen = true;
            MessageSource.Content = new string[]
            {
                "Inside the chest is handful of worthless pennies and a well-preserved sword. You take the latter."
            };
            MessageSource.Trigger();

            var loot = GameObject.Instantiate(Loot);
            loot.name = Loot.name;
            Inventory.AddItem(loot);
        }

    }
}
