using UnityEngine;
using System.Collections;

public class InteractibleMessageSource : MonoBehaviour
{
    public bool DestroyAfterInteracting = true;
    public string[] Content;

    private MessageSource MessageSource;
    private CharacterMovement CharacterMovement;

    // Use this for initialization
    void Start()
    {
        MessageSource = gameObject.AddComponent<MessageSource>();
        MessageSource.Content = Content;

        CharacterMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterMovement>();

        CharacterMovement.RegisterInteractable(gameObject, Interact);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Interact()
    {
        MessageSource.Trigger();
        if (DestroyAfterInteracting)
        {
            CharacterMovement.UnregisterInteractable(gameObject);
            Destroy(gameObject);
        }
    }
}
