using UnityEngine;
using System.Collections;

public class Waterpond : MonoBehaviour
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
            "Water is trickling from a small hole in the wall forming a small pond in the corner of the room.",
            "The water surface is reflecting the surrounding area. You find the whole scene very peaceful."
       };

       MessageSource.Trigger();

        if (!Seen)
        {
            Seen = true;
            GameManager.ExplorePoints += 1;
        }
    }
}
