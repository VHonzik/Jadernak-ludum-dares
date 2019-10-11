using UnityEngine;
using System.Collections;

public class KirasBook : MonoBehaviour
{
    private MessageSource MessageSource;
    private GameState GameManager;
    private CharacterMovement CharacterMovement;

    // Use this for initialization
    void Start()
    {
        MessageSource = gameObject.AddComponent<MessageSource>();
        GameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameState>();
        CharacterMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterMovement>();

        CharacterMovement.RegisterInteractable(gameObject, Interact);
    }

    public void Interact()
    {
        GameManager.InteractedWithKirasBook = true;

        MessageSource.Content = new string[]
        {
            "The girl is holding a black book. She is fully engrossed in reading.",
            "You wonder what the book is."
        };

        MessageSource.Trigger();

    }
}
