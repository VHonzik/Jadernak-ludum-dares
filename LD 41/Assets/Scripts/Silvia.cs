using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Silvia : MonoBehaviour
{
    private MessageSource MessageSource;
    private GameState GameManager;
    private CharacterMovement CharacterMovement;
    private Health Health;

    private int InteractCount;
    private bool Seen;

    // Use this for initialization
    void Start()
    {
        MessageSource = gameObject.AddComponent<MessageSource>();
        CharacterMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterMovement>();
        GameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameState>();
        Health = FindObjectOfType<Health>();

        CharacterMovement.RegisterInteractable(gameObject, Chat);
    }

    public void Chat()
    {
        List<string> Message = new List<string>();

        bool brakeHeart = false;

        if (GameManager.RequiredSilviaInteractions < 3)
        {
            Message.AddRange(new string[]{
                "You can't just speak to her, that's crazy.",
                "Maybe you could learn something about her so you have things to talk about."
            });
        }
        else
        {
            if (InteractCount == 0)
            {
                Message.AddRange(new string[]{
                    "You ask if is she struck a gold yet.",
                    "\"Not yet,\" she smiles. That smile hits like a lighting... You eventually mumble your name.",
                    "\"Nice to meet you, I am Silvia.\"",
                    "She take a bite of her snack while you search for words. Maybe you could offer to help?",
                    "She measures you with a stare and her expression changes.",
                    "\"I don't need no knight in a shinning armor.\"",
                });
            }
            else if (InteractCount == 1)
            {
                Message.AddRange(new string[]{
                    "You attempt to explain you did not mean it that way.",
                    "\"I don't care.\"",
                    "Is it one of those noes which mean yes? You ask if you can at least hang around.",
                    "\"Absolutely not. Go away.\" A despair overwhelms you.",

                });
                brakeHeart = true;

                if (Health.HP - 1 <= 1 && !GameManager.SpokenWithWizard)
                {
                    Message.Add("There must someone who can give you an advice.");
                }
            }
            else
            {
                Message.AddRange(new string[]{
                    "You are met with a cold stare and a hand reaching for an ax."
                });
            }

            InteractCount += 1;
        }

        MessageSource.Content = Message.ToArray();
        MessageSource.Trigger();

        if (brakeHeart)
        {
            StartCoroutine(BrakeHeart());
        }
    }

    void OnBecameVisible()
    {
        if (!Seen)
        {
            MessageSource.Content = new string[]
            {
            "Sitting against the northern wall is a young lady. She has a cute little nose and a brown ponytail.",
            "Her muscular body is at the same time beautiful and terrifying. She is way out of your league.",
            "All of the possible actions run through your head, most prominently to ignore her presence.",
            };
            MessageSource.Trigger();
            Seen = true;
        }
    }

    private IEnumerator BrakeHeart()
    {
        yield return StartCoroutine(MessageSource.Wait());
        Health.BrakeHeart();
    }
}
