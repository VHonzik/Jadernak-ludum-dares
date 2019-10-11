using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DangerousMonster : MonoBehaviour
{
    private GameObject Trigger;
    private BoxCollider2D TriggerCollider;

    private MessageSource MessageSource;
    private CharacterMovement CharacterMovement;
    private GameObject Character;

    public delegate void SlainDelegate();

    public string[] TriggeredMessage;
    public int TriggerSize;

    public string RequiredWeapon;
    public string[] NotRequiredWeaponMessage;
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
        CharacterMovement.RegisterInteractable(gameObject, Interact);
    }

    private IEnumerator CloseEnoughCo()
    {
        List<string> Message = new List<string>();

        Message.AddRange(TriggeredMessage);

        bool CanEngage = true;

        if (!Character.transform.Find(RequiredWeapon))
        {
            Message.AddRange(NotRequiredWeaponMessage);
            CanEngage = false;
        }

        MessageSource.Content = Message.ToArray();
        MessageSource.Trigger();

        yield return StartCoroutine(MessageSource.Wait());

        if (CanEngage)
        {
            TriggerCollider.enabled = false;
        }
    }

    private void GotCloseEnough()
    {
        StartCoroutine(CloseEnoughCo());
    }

    private IEnumerator InteractCo()
    {
        MessageSource.Content = SlainMessage;
        MessageSource.Trigger();

        yield return StartCoroutine(MessageSource.Wait());

        SlainCb();
        Destroy(gameObject);
    }

    private void Interact()
    {
        StartCoroutine(InteractCo());
    }
}
