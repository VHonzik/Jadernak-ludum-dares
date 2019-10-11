using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Kira : MonoBehaviour
{
    private MessageSource MessageSource;
    private GameState GameManager;
    private CharacterMovement CharacterMovement;
    private Health Health;
    private KirasForcefield KirasForcefield;

    private int InteractCount;
    private bool Seen;
    private bool AskedAboutBook;

    // Use this for initialization
    void Start()
    {
        MessageSource = gameObject.AddComponent<MessageSource>();
        CharacterMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterMovement>();
        GameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameState>();
        Health = FindObjectOfType<Health>();
        KirasForcefield = FindObjectOfType<KirasForcefield>();

        CharacterMovement.RegisterInteractable(gameObject, Chat);
    }

    public void Chat()
    {
        List<string> Message = new List<string>();

        bool punchOut = false;

        if (InteractCount == 0)
        {
            Message.AddRange(new string[]{
                "You approach the girl.",
                "...",
                "....",
                "\"Hey, sorry, I wanted to finish a paragraph.\"",
                "You have no idea what to say. Hurry up. Think of something. You ask how is she doing.",
            });

            if (GameManager.InteractedWithKirasBook)
            {
                Message.AddRange(new string[]{
                    "You could ask about the book!",
                    "\"Oh it's called The Plague and it's from Albert Camus.\"",
                    "\"It's pretty dark but I am enjoying the characters so far and Camus just knows how to write.\"",
                    "You have not read that so there is not much else to say."
                });
                AskedAboutBook = true;
            }
            else
            {
                Message.AddRange(new string[]{
                    "\"I am doing good, thanks for asking.\"",
                    "Nothing else comes out of your mouth. She goes back to reading a book."
                }); ;
            }
        }
        else if (InteractCount == 1)
        {
            if (GameManager.InteractedWithKirasBook && !AskedAboutBook)
            {
                Message.AddRange(new string[]{
                    "Ask about the book you stupid, girls like good listeners.",
                    "\"It's called The Plague and it's from Albert Camus.\"",
                    "\"It's pretty dark but I am enjoying the characters so far\"",
                    "\"and Camus just knows how to write.\"",
                    "You have not read that book.",
                    "..."
                });
                AskedAboutBook = true;
            }

            Message.AddRange(new string[]{
                "Someone told you that you have to bold with girls, so you decide to invite her to come exploring with you.",
                "\"Not really interested right now. I can't wait for the next version of Joseph's sentence.\"",
                "She goes back to reading the book."
            });
        }
        else if (InteractCount == 2)
        {
            Message.AddRange(new string[]{
                "Before you can ask something else...",
                "\"You are still here?\"",
                "The tone of her voice lacks any emotions apart from surprise. It is also very crushing."
            });
        }
        else if (InteractCount == 3)
        {
            Message.AddRange(new string[]{
                "\"Enough.\"",
                "She clenches one of her hands into a fist.",
            });
            punchOut = true;
        }

        MessageSource.Content = Message.ToArray();
        MessageSource.Trigger();
        InteractCount += 1;

        if (punchOut)
        {
            StartCoroutine(ForceAway());
        }
    }

    void OnBecameVisible()
    {
        if (!Seen)
        {
            MessageSource.Content = new string[]
            {
            "In the corner of the room you spot a girl lying on the ground.",
            "Upon getting closer you notice she is rather attractive and you immediately become nervous.",
            "She has a beautiful long dark hair and a nice little mouth. There is overwhelming urge to flee on your mind.",
            "You repeat to yourself, just stay cool.",
            };
            MessageSource.Trigger();
            Seen = true;
        }
    }

    private IEnumerator ForceAway()
    {
        yield return MessageSource.Wait();

        yield return StartCoroutine(CharacterMovement.PushAway(transform.position, 5, 0.25f));
        KirasForcefield.Raise();

        List<string> Message = new List<string>();

        Message.AddRange(new string[]{
            "You feel like a giant rock just hit you and thrown you good distance away.",
            "Apart from bruised body you take away bruised feelings."
        });

        if (Health.HP-1 <= 1 && !GameManager.SpokenWithWizard)
        {
            Message.Add("There must someone who give you an advice.");
        }

        MessageSource.Content = Message.ToArray();
        MessageSource.Trigger();

        yield return StartCoroutine(MessageSource.Wait());
        Health.BrakeHeart();
    }
}
