using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EngagingMonster : MonoBehaviour
{
    private GameObject Trigger;
    private BoxCollider2D TriggerCollider;

    private MessageSource MessageSource;
    private CharacterMovement CharacterMovement;
    private GameObject Character;

    public delegate void SlainDelegate();

    public string[] TriggeredMessage;
    public int TriggerSize;

    public int Speed;
    public string RequiredWeapon;
    public string[] NotRequiredWeaponMessage;
    public string[] DisengageMessage;
    public string[] SlainMessage;
    public SlainDelegate SlainCb;


    // Use this for initialization
    void Start()
    {
        MessageSource = gameObject.AddComponent<MessageSource>();
        Character = GameObject.FindGameObjectWithTag("Player");
        CharacterMovement = Character.GetComponent<CharacterMovement>();

        Trigger = new GameObject("Trigger");
        Trigger.transform.SetParent(transform);
        Trigger.transform.localPosition = new Vector3(0, 0, 0);
        TriggerCollider = Trigger.AddComponent<BoxCollider2D>();
        TriggerCollider.size = new Vector2(TriggerSize * 2 + 1, TriggerSize * 2 + 1);

        CharacterMovement.RegisterInteractable(Trigger, GotCloseEnough);
    }

    private IEnumerator Engage()
    {
        TriggerCollider.enabled = false;

        MessageSource.Content = TriggeredMessage;
        MessageSource.Trigger();

        yield return StartCoroutine(MessageSource.Wait());

        // Find possible direction
        List<Vector3> possibleDirections = new List<Vector3>();

        Vector3 direction = Vector3.down;
        for (var dir = 0; dir < 4; dir++)
        {
            direction = Quaternion.Euler(0, 0, dir * 90.0f) * direction;
            direction.Normalize();

            var destination = CharacterMovement.transform.position + direction;

            var walkable = Physics2D.OverlapCircleNonAlloc(destination, 0.0f, new Collider2D[1], 1 << 8) > 0;
            var obstacle = Physics2D.OverlapCircleNonAlloc(destination, 0.0f, new Collider2D[1], ~(1 << 8)) > 0;

            if (walkable && !obstacle)
            {
                possibleDirections.Add(direction);
            }
        }

        if (possibleDirections.Count > 0)
        {
            // Choose the closest one to the direction between monster and player
            Vector3 mostWanted = (transform.position - CharacterMovement.transform.position).normalized;
            float closestDot = possibleDirections.Max(x => Vector3.Dot(mostWanted, x));
            var closest = possibleDirections.First(x => Vector3.Dot(mostWanted, x) == closestDot);
            var chosenDestination = CharacterMovement.transform.position + closest;

            var initialPosition = transform.position;

            while (Vector3.Distance(transform.position, chosenDestination) > 0.0f)
            {
                transform.position = Vector3.MoveTowards(transform.position, chosenDestination, Speed * Time.deltaTime);
                yield return null;
            }

            // Resolve combat
            if (Character.transform.Find(RequiredWeapon))
            {
                MessageSource.Content = SlainMessage;
                MessageSource.Trigger();

                yield return StartCoroutine(MessageSource.Wait());

                SlainCb();
                Destroy(gameObject);
            }
            else
            {
                MessageSource.Content = NotRequiredWeaponMessage;
                MessageSource.Trigger();

                yield return StartCoroutine(MessageSource.Wait());

                StartCoroutine(CharacterMovement.PushAway(transform.position, 2, 0.25f));

                while (Vector3.Distance(transform.position, initialPosition) > 0.0f)
                {
                    transform.position = Vector3.MoveTowards(transform.position, initialPosition, Speed * Time.deltaTime);
                    yield return null;
                }

                MessageSource.Content = DisengageMessage;
                MessageSource.Trigger();

                yield return StartCoroutine(MessageSource.Wait());

            }
        }

        TriggerCollider.enabled = true;
    }

    private void GotCloseEnough()
    {
        StartCoroutine(Engage());
    }
}
