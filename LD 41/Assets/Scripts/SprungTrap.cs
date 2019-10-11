using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SprungTrap : MonoBehaviour
{
    private MessageSource MessageSource;
    private GameState GameManager;
    private CharacterMovement CharacterMovement;
    private Inventory Inventory;

    public GameObject Loot;

    private bool Seen;

    // Use this for initialization
    void Start()
    {
        MessageSource = gameObject.AddComponent<MessageSource>();
        GameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameState>();
        CharacterMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterMovement>();
        Inventory = FindObjectOfType<Inventory>();

        CharacterMovement.RegisterInteractable(gameObject, Interact);
    }

    private IEnumerator InteractCo()
    {
        List<string> Message = new List<string>();
        Message.AddRange(new string[]
        {
            "Poor fella probably never saw it coming. The trap is covered with a mixture of dirt and blood."
        });

        if (!Seen)
        {
            Message.AddRange(new string[] {
                "You notice a shining object in a skeleton's hand.",
                "As you tear it away you identify it as a small dagger."
            });
        }

        MessageSource.Content = Message.ToArray();
        MessageSource.Trigger();

        yield return StartCoroutine(MessageSource.Wait());

        if (!Seen)
        {
            Seen = true;
            GameManager.ExplorePoints += 1;
            var loot = GameObject.Instantiate(Loot);
            loot.name = Loot.name;
            Inventory.AddItem(loot);
        }

    }

    void Interact()
    {
        StartCoroutine(InteractCo());
    }
}
