using UnityEngine;
using System.Collections;

public class MagicalUrn : MonoBehaviour
{
    private PickableItem PickableItem;
    private InventoryItem InventoryItem;

    // Use this for initialization
    void Start()
    {
        PickableItem = gameObject.AddComponent<PickableItem>();
        PickableItem.PickUpMessage = new string[]
        {
            "You pick up an ornamental urn.",
            "If whoever is inside was a magic user, the urn itself might be magical. Could be useful."
        };

        InventoryItem = gameObject.AddComponent<InventoryItem>();
        InventoryItem.Uses = -1;
        InventoryItem.UseMessage = new string[]
        {
            "It's an urn full of unknown and no longer existing person."
        };
    }

    // Update is called once per frame
    void Update()
    {

    }
}
