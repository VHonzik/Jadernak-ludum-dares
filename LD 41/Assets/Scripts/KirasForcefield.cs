using UnityEngine;
using System.Collections;

public class KirasForcefield : MonoBehaviour
{
    private BoxCollider2D BoxCollider2D;
    private CharacterMovement CharacterMovement;
    private MessageSource MessageSource;

    // Use this for initialization
    void Start()
    {
        BoxCollider2D = GetComponent<BoxCollider2D>();
        BoxCollider2D.enabled = false;

        CharacterMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterMovement>();
        CharacterMovement.RegisterInteractable(gameObject, Interact);

        MessageSource = gameObject.AddComponent<MessageSource>();
    }

    // Update is called once per frame
    public void Raise()
    {
        BoxCollider2D.enabled = true;
    }

    public void Interact()
    {
        MessageSource.Content = new string[]
        {
            "A force is pushing you away. It's so strong you can't make any progress further."
        };

        MessageSource.Trigger();
    }
}
