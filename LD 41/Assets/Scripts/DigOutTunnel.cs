using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DigOutTunnel : MonoBehaviour
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
        List<string> Message = new List<string>();
        Message.AddRange(new string[]
        {
            "Someone is digging a tunnel through the wall. It must be really hard work."
        });

        if (!Seen)
        {
            Seen = true;
            GameManager.ExplorePoints += 1;
            GameManager.RequiredSilviaInteractions += 1;

            if (GameManager.RequiredSilviaInteractions == 3)
            {
                Message.Add("The girl is digging a tunnel somewhere. First question for her, check.");
            }
        }

        MessageSource.Content = Message.ToArray();
        MessageSource.Trigger();
    }
}
