using UnityEngine;
using System.Collections;

public class Spider : MonoBehaviour
{
    private EngagingMonster Monster;
    private GameState GameManager;

    // Use this for initialization
    void Start()
    {
        GameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameState>();

        Monster = gameObject.AddComponent<EngagingMonster>();
        Monster.TriggerSize = 3;
        Monster.TriggeredMessage = new string[]
        {
            "As you get closer, a giant spider registers you. It hisses at you and charges you."
        };
        Monster.Speed = 7;
        Monster.RequiredWeapon = "Spear";
        Monster.NotRequiredWeaponMessage = new string[]
        {
            "The spider is blazing fast and has a superior reach with his giant legs. You need to run."
        };
        Monster.DisengageMessage = new string[]
        {
            "You will need a better weapon. Something for up close but not that up close."
        };
        Monster.SlainMessage = new string[]
        {
            "The spear proves effective against both spider's offense and defense.",
            "With a few skilled moves it draws its last breath."
        };
        Monster.SlainCb = () => GameManager.SlainSpider = true;
    }

}
