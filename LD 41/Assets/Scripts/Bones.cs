using UnityEngine;
using System.Collections;

public class Bones : MonoBehaviour
{
    private MessageSource MessageSource;
    private GameState GameManager;
    private CharacterMovement CharacterMovement;

    private bool Seen;

    // Use this for initialization
    void Start()
    {
        MessageSource = gameObject.AddComponent<MessageSource>();
        GameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameState>();
        CharacterMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterMovement>();

        CharacterMovement.RegisterInteractable(gameObject, Interact);

        Seen = false;
    }

    void Interact()
    {
        MessageSource.Content = new string[]
        {
            "On the ground are lying bones of a large creature.",
            "The bones are twisted and broken, likely from a powerful kinetic spell.",
        };

        MessageSource.Trigger();

        if (!Seen)
        {
            Seen = true;
            GameManager.ExplorePoints += 1;
        }
    }
}
