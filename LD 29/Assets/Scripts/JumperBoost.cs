using UnityEngine;
using System.Collections;

public class JumperBoost : MonoBehaviour {

    private bool isUsed = false;

    public AudioClip jumpSound;

    private AudioSource soundPlayer;

    // Use this for initialization
    void Awake()
    {
        soundPlayer = gameObject.AddComponent<AudioSource>();
        soundPlayer.playOnAwake = false;
        soundPlayer.loop = false;
        soundPlayer.volume = 0.8f;
    }

    void Update()
    {
        if (Input.GetAxis("Jump") != 0 && Mathf.Abs(GameObject.Find("pawn").transform.position.x - gameObject.transform.position.x) < 0.5f)
        {
            if (GameObject.Find("pawn").GetComponent<PlayerPawn>().getIteractionEnabled() && !isUsed)
            {
                isUsed = true;
                GameObject.Find("pawn").GetComponent<PlayerPawn>().disableIteraction();
                startEffect();
            }

        }
    }

    private void startEffect()
    {
        soundPlayer.PlayOneShot(jumpSound);
        Vector2 dir =  new Vector2(0f,1f);
        GameObject.Find("pawn").GetComponent<PlayerPawn>().MakeJump(dir.normalized, 400f);
        isUsed = false;
    }

}
