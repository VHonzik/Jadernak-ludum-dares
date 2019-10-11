using UnityEngine;
using System.Collections;

public class Troll : MonoBehaviour
{
    private DangerousMonster Monster;
    private GameState GameManager;
    private Inventory Inventory;

    public GameObject Loot;

    // Use this for initialization
    void Start()
    {
        GameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameState>();
        Inventory = FindObjectOfType<Inventory>();

        Monster = gameObject.AddComponent<DangerousMonster>();
        Monster.TriggerSize = 4;
        Monster.TriggeredMessage = new string[]
        {
            "A towering troll occupies this room. Luckily for you it has not seen you yet."
        };
        Monster.RequiredWeapon = "Dagger";
        Monster.NotRequiredWeaponMessage = new string[]
        {
            "Your only chance is to sneak in and kill it before its brain registers the unfortunate situation.",
            "Unfortunately you are lacking a weapon for that."
        };
        Monster.SlainMessage = new string[]
        {
            "Your only chance is to sneak in and kill it before its brain registers the unfortunate situation.",
            "You elegantly jump on its back and with a few quick stabs end its existence.",
            "Under his body you find a sturdy spear and it now belongs to you."
        };
        Monster.SlainCb = () =>
        {
            GameManager.SlainTroll = true;
            var loot = GameObject.Instantiate(Loot);
            loot.name = Loot.name;
            Inventory.AddItem(loot);
        };
    }
}
