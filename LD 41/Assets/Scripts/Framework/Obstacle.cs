using UnityEngine;
using System.Collections;

public class Obstacle : MonoBehaviour
{
    private MessageSource MessageSource;
    private GameState GameManager;
    private CharacterMovement CharacterMovement;
    private GameObject Character;
    private Inventory Inventory;
    private bool Seen;

    public string RequiredItem;
    public string[] NotRequiredItemMessage;
    public string[] RequiredItemMessage;
    public bool UseItem;
    public bool DestroyObstacle;
    public int ExplorePoints;

    // Use this for initialization
    void Start()
    {
        MessageSource = gameObject.AddComponent<MessageSource>();
        GameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameState>();
        Character = GameObject.FindGameObjectWithTag("Player");
        CharacterMovement = Character.GetComponent<CharacterMovement>();
        Inventory = FindObjectOfType<Inventory>();

        CharacterMovement.RegisterInteractable(gameObject, Interact);
    }

    private IEnumerator InteractCo()
    {
        var item = Character.transform.Find(RequiredItem);
        if (item)
        {
            MessageSource.Content = RequiredItemMessage;
            MessageSource.Trigger();

            yield return StartCoroutine(MessageSource.Wait());

            if (UseItem)
            {
                yield return StartCoroutine(Inventory.UseItem(item.gameObject));
            }

            if (DestroyObstacle)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            MessageSource.Content = NotRequiredItemMessage;
            MessageSource.Trigger();

            yield return StartCoroutine(MessageSource.Wait());
        }
    }

    void Interact()
    {
        if (!Seen)
        {
            Seen = true;
            if (ExplorePoints > 0)
            {
                GameManager.ExplorePoints += ExplorePoints;
            }
        }
        StartCoroutine(InteractCo());
    }
}
