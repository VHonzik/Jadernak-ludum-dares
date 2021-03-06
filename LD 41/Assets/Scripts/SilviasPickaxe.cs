﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SilviasPickaxe : MonoBehaviour
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
    }

    public void Interact()
    {
        List<string> Message = new List<string>();
        Message.AddRange(new string[]
        {
            "Heavy-looking pickax is wet with sweat.",
        });

        if (!Seen)
        {
            Seen = true;
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
