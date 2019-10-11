using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Wizard : MonoBehaviour
{
    private MessageSource MessageSource;
    private CharacterMovement CharacterMovement;
    private Health Health;
    private GameState GameManager;

    private int InteractCount;
    private bool Seen;

    // Use this for initialization
    void Start()
    {
        MessageSource = gameObject.AddComponent<MessageSource>();
        CharacterMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterMovement>();
        Health = FindObjectOfType<Health>();
        GameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameState>();

        CharacterMovement.RegisterInteractable(gameObject, Chat);
    }

    // Update is called once per frame
    void Chat()
    {
        List<string> Message = new List<string>();

        if (Health.HP > 1)
        {
            Message.AddRange(new string[]{
                "Below a bare tree is a wizard snoozing away. Best not to disrupt him unless in dire need.",
            });
        }
        else if (Health.HP <= 1 && InteractCount == 0)
        {
            Message.AddRange(new string[]{
                "Below a bare tree is a wizard snoozing away.",
                "As gently as humanly possible you attempt to wake up the wizard.",
                "\"DIE YOU FOUL BEAST!\" screams the wizard. He notices it's just you covering in fear.",
                "\"Ehm ehm, how can I help you son?\" How can someone go from utter fury to gentle tenderness so quickly?",
                "You nervously stare at your feet and produce non-verbal sounds.",
                "\"Out with it son, I have important matters to attend.\"",
                "You quietly ask him why are you so bad with girls. His expression softens even more.",
                "\"I can't speak for your particular actions but here is something I have learned over my long life.\"",
                "\"Don't try too hard. Learn how to be alone and be content with it. Do what you enjoy, go out there and live the life.\"",
                "You ask how is that gonna help with talking to girls.",
                "The wizards laughs. \"You would be surprised,\" he winks at you.",
                "\"Now as I have said I have other things to do. Good luck son.\" He goes back to sleep.",
                "That did not seem really helpful. You sigh and go on clearing this dungeons."
            });
            InteractCount += 1;
            GameManager.SpokenWithWizard = true;
        }
        else if (InteractCount > 0)
        {
            Message.AddRange(new string[]{
                "The wizard has other important matters to attend.",
            });
        }

        MessageSource.Content = Message.ToArray();
        MessageSource.Trigger();
    }
}
