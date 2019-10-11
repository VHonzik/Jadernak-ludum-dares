using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Lillith : MonoBehaviour
{
    private MessageSource MessageSource;
    private GameObject Character;
    private CharacterMovement CharacterMovement;
    private Health Health;
    private GameState GameManager;

    private int Speed = 4;

    public int InteractCount;
    private bool Seen;

    // Use this for initialization
    void Start()
    {
        MessageSource = gameObject.AddComponent<MessageSource>();
        Character = GameObject.FindGameObjectWithTag("Player");
        CharacterMovement = Character.GetComponent<CharacterMovement>();
        Health = FindObjectOfType<Health>();

        GameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameState>();

        CharacterMovement.RegisterInteractable(gameObject, Chat);
    }

    void Chat()
    {
        List<string> Message = new List<string>();

        if (InteractCount == 0)
        {
            Message.AddRange(new string[]{
                "\"My name is Lilith and right now I am a professional mushroom picker.\"",
                "\"My mom send me to find wolf boletes for my sick little brother and I already have almost all we need.\"",
                "\"I might consider applying to mushroom university.\" She laughs wholeheartedly.",
                "\"What's your name?\" ... \"Oh look another one,\" she picks up a mushroom.",
                "\"Pardon, sometimes I talk too much, go ahead.\"",
                "So many words and yet you feel more comfortable knowing she will do most of the talking. You say your name.",
                "\"Pleasure to meet you. You wouldn't believe what happened to me today.\"",
                "\"Don't just stand there, go get some boletes. You know how they look right?\"",
                "Not wanting to look uneducated in her eyes you quickly nod. She launches into a monologue."
            });
        }
        else if (InteractCount >= 1)
        {
            var item = Character.transform.Find("Mushroom");
            if (!item)
            {
                Message.Add("She asked you bring a dog mushroom or something, you should look around for it.");
            }
            else
            {
                StartCoroutine(Poison());
            }
        }

        MessageSource.Content = Message.ToArray();
        MessageSource.Trigger();
        InteractCount += 1;
    }

    void OnBecameVisible()
    {
        if (!Seen)
        {
            MessageSource.Content = new string[]
            {
            "Among the underground flora is kneeling a tall and slender figure.",
            "As you enter the room its head snaps to look at you. It is a girl with short blond hair.",
            "Her eyes have a color of cold silver and they melt your heart like a hot iron.",
            "\"A fellow adventurer! What brings you to my favorite spot in this dungeon?\"",
            "She beckons you to come speak with her."
            };
            MessageSource.Trigger();
            Seen = true;
        }
    }

    private IEnumerator Poison()
    {
        MessageSource.Content = new string[]
        {
            "You hand her the mushroom you found. She inspects it and then looks at you.",
            "A few agonizing seconds later: \"Good find boy.\" You sigh with relief.",
            "\"Here,\" she takes a chopped mushroom from her bag, \"as a reward, a snack on me. It's really good.\"",
            "You take it from her and start chewing on it.",
            "\"Did you know there is a mushroom called the running dapperling?\"",
            "\"It's really funny cause, once eaten, you will get a very acute diarrhea, or should I say the runs?\"",
            "\"Or there is this less funny one, called royal agaric, which will stop your breathing in less than 10 minutes.\"",
            "You are starting to feel little dizzy. You need to sit down.",
            "\"Which by the way happens to be the one you brought me. Guess who is failing his truth-telling class?\"",
            "Opening your mouth in shock would be a natural reaction but somehow your body does not respond.",
            "\"And last but not least, gray milkcap known for its effect of quick paralysis. Don't worry it passes in a few minutes.\"",
            "\"Anyway gotta go now. It's was nice chatting with you.\" She extends her hand for a hand shake.",
            "\"No? And I was begging to think we could be friends.\""
        };
        MessageSource.Trigger();
        CharacterMovement.BlockInput(this);

        yield return StartCoroutine(MessageSource.Wait());

        var chosenDestination = transform.position + Vector3.down * 3;

        while (Vector3.Distance(transform.position, chosenDestination) > 0.0f)
        {
            transform.position = Vector3.MoveTowards(transform.position, chosenDestination, Speed * Time.deltaTime);
            yield return null;
        }

        chosenDestination = transform.position + Vector3.right * 18;
        while (Vector3.Distance(transform.position, chosenDestination) > 0.0f)
        {
            transform.position = Vector3.MoveTowards(transform.position, chosenDestination, Speed * Time.deltaTime);
            yield return null;
        }

        List<string> Message = new List<string>();

        Message.AddRange(new string[]
        {
            "Why must you be so bad at talking to girls? This one literally poisoned you to get rid of you."
        });

        if (Health.HP - 1 <= 1 && !GameManager.SpokenWithWizard)
        {
            Message.Add("There must someone who can give you an advice.");
        }

        MessageSource.Content = Message.ToArray();
        MessageSource.Trigger();

        yield return StartCoroutine(MessageSource.Wait());

        Health.BrakeHeart();

        CharacterMovement.UnblockInput(this);

        Destroy(gameObject);
    }
}
