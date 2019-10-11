using UnityEngine;
using System.Collections;

public class Mushroom : MonoBehaviour
{
    private MessageSource MessageSource;
    private CharacterMovement CharacterMovement;
    private Inventory Inventory;
    private Lillith Lillith;

    public GameObject Loot;

    private bool Seen;

    // Use this for initialization
    void Start()
    {
        MessageSource = gameObject.AddComponent<MessageSource>();
        CharacterMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterMovement>();
        Inventory = FindObjectOfType<Inventory>();
        Lillith = FindObjectOfType<Lillith>();

        CharacterMovement.RegisterInteractable(gameObject, Interact);
    }

    // Update is called once per frame
    void Interact()
    {
        if (!Seen && Lillith.InteractCount > 0)
        {
            Seen = true;
            MessageSource.Content = new string[]
            {
                "All the mushrooms look the same to you. So you just pick one."
            };
            MessageSource.Trigger();

            var loot = GameObject.Instantiate(Loot);
            loot.name = Loot.name;
            Inventory.AddItem(loot);

            foreach(var child in GetComponentsInChildren<BoxCollider2D>())
            {
                child.enabled = false;
            }
        }
        else if (Lillith.InteractCount == 0)
        {
            MessageSource.Content = new string[]
            {
                "Brown looking mushroom."
            };
            MessageSource.Trigger();
        }
    }
}
