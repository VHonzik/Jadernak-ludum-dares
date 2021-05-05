using UnityEngine;
using System.Collections;

public class PlayerCharacter : MonoBehaviour {

    private float bodyFreeze;
    private bool closeToFire = false;
    private FrostEffect frostEffect;

	void Start () {

        bodyFreeze = 0;
        Task Freezing = new Task(AdjustBodyFreeze(1f));
        frostEffect = Camera.main.GetComponent<FrostEffect>();
	}
	
	// Update is called once per frame
	void Update () {

        //is poor guy frozen? :(
        if (bodyFreeze >= GameSettings.freezeLimit)
        {
            Debug.Log("You are dead!!!");
        }
	}

    IEnumerator AdjustBodyFreeze(float period)
    {
        while (true)
        {
            if (closeToFire)
            {
                bodyFreeze = Mathf.Clamp(bodyFreeze - GameSettings.warmGainPerSecond, 0, GameSettings.freezeLimit);
                frostEffect.FrostAmount = bodyFreeze / (2f * GameSettings.freezeLimit);
            }
            else
            {
                bodyFreeze = Mathf.Clamp(bodyFreeze + GameSettings.freezeGainPerSecond, 0, GameSettings.freezeLimit);
                frostEffect.FrostAmount = bodyFreeze / (2f * GameSettings.freezeLimit);
            }

            yield return new WaitForSeconds(period);
        }
    }

    void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "Fire")
        {            
            closeToFire = true;
            
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Fire")
        {
            closeToFire = false;
        }
    }
}
