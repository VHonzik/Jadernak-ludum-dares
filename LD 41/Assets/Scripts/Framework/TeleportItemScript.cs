using UnityEngine;
using System.Collections;

[RequireComponent(typeof(DirectionalItemScript))]
public class TeleportItemScript : MonoBehaviour
{
    private CharacterMovement CharacterMovement;

    public float Duration;
    public int Distance;
    public string[] FailedMessage;

    private MessageSource FailedMessageSource;


    public IEnumerator DirectionSelected(Vector3 direction)
    {
        var destination = CharacterMovement.transform.position + direction * Distance;
        if (Physics2D.OverlapCircleNonAlloc(destination, 0.0f, new Collider2D[1], 1 << 8) > 0)
        {
            yield return StartCoroutine(CharacterMovement.Teleport(destination, Duration));
        }
        else
        {
            FailedMessageSource.Trigger();
        }
    }

    // Use this for initialization
    void Start()
    {
        FailedMessageSource = gameObject.AddComponent<MessageSource>();
        FailedMessageSource.Content = FailedMessage;
        CharacterMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterMovement>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
