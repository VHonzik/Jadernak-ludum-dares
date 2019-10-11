using UnityEngine;
using System.Collections;

public class Celine : MonoBehaviour
{
    private MessageSource MessageSource;
    private GameObject Character;
    private CharacterMovement CharacterMovement;
    private SpriteRenderer SpriteRenderer;
    private Health Health;
    private ThanksForPlaying ThanksForPlaying;

    // Use this for initialization
    void Start()
    {
        MessageSource = gameObject.AddComponent<MessageSource>();
        Character = GameObject.FindGameObjectWithTag("Player");
        CharacterMovement = Character.GetComponent<CharacterMovement>();
        SpriteRenderer = GetComponent<SpriteRenderer>();
        Health = FindObjectOfType<Health>();
        ThanksForPlaying = FindObjectOfType<ThanksForPlaying>();

        CharacterMovement.RegisterInteractable(gameObject, Interact);
    }

    private IEnumerator InteractCo()
    {
        CharacterMovement.BlockInput(this);

        MessageSource.Content = new string[]
        {
            "The room is gleaming with riches of every imaginable kind."
        };

        MessageSource.Trigger();
        yield return StartCoroutine(MessageSource.Wait());

        var originalPosition = Character.transform.position;
        var chosenDestination = Character.transform.position + Vector3.down * 6;
        var MovementTimer = 0.0f;
        while (Vector3.Distance(Character.transform.position, chosenDestination) > 0.0f)
        {
            if (MovementTimer <= 0.0f)
            {
                Character.transform.position += Vector3.down;
                MovementTimer = CharacterMovement.MovementTimePerMove;
            }
            else if (MovementTimer > 0.0f)
            {
                MovementTimer -= Time.deltaTime;
                MovementTimer = Mathf.Max(MovementTimer, 0.0f);
            }
            yield return null;
        }

        MessageSource.Content = new string[]
        {
            "This must the end of this level of the dungeon.",
            "A feminine voice from behind you startles you. \"Wait! Wait for me!\""
        };

        MessageSource.Trigger();
        yield return StartCoroutine(MessageSource.Wait());

        transform.position = originalPosition;
        SpriteRenderer.enabled = true;


        chosenDestination = transform.position + Vector3.down * 5;
        MovementTimer = 0.0f;
        while (Vector3.Distance(transform.position, chosenDestination) > 0.0f)
        {
            if (MovementTimer <= 0.0f)
            {
                transform.position += Vector3.down;
                MovementTimer = CharacterMovement.MovementTimePerMove;
            }
            else if (MovementTimer > 0.0f)
            {
                MovementTimer = Mathf.Max(MovementTimer - Time.deltaTime, 0.0f);
            }
            yield return null;
        }

        MessageSource.Content = new string[]
        {
            "\"I am Celine. That fight with the spider was awesome, you were like...\" she imitates attacks and blocks.",
            "Below the cloak you peak a perfect oblong face. While you are busy studying her beauty she finishes the imaginary spider.",
            "\"Come on, I want to see you in more action.\" She takes your hand and drags you down the stairs.",
            "Her hand is so soft. Your face turns into a giant ruby as you leave all of the riches behind..."
        };

        MessageSource.Trigger();
        yield return StartCoroutine(MessageSource.Wait());

        Vector3[] HisMovements = new Vector3[]
        {
            Vector3.zero,
            Vector3.zero,
            Vector3.right,
            Vector3.down,
        };

        Vector3[] HerMovements = new Vector3[]
        {
            Vector3.right,
            Vector3.down,
            Vector3.down,
        };

        for(int i=0; i < 4; i++)
        {
            MovementTimer = CharacterMovement.MovementTimePerMove;
            if (i < HisMovements.Length)
            {
                Character.transform.position += HisMovements[i];
            }

            if (i < HerMovements.Length)
            {
                transform.position += HerMovements[i];
            }
            else
            {
                SpriteRenderer.enabled = false;
            }

            if (i != 3)
            {
                while (MovementTimer > 0.0f)
                {
                    MovementTimer = Mathf.Max(MovementTimer - Time.deltaTime, 0.0f);
                    yield return null;
                }
            }
        }

        Character.GetComponent<SpriteRenderer>().enabled = false;

        Health.Heal();

        yield return new WaitForSeconds(3.0f);

        ThanksForPlaying.Show();
    }

    void Interact()
    {
        StartCoroutine(InteractCo());
    }
}
