using UnityEngine;
using System.Collections;

public class CollectableStar : MonoBehaviour {

    private bool pickable;
    private bool falling;

    private Vector2 initVel;
    private Vector3 vel;

    public delegate void OnCollectDo();
    public AudioClip pickupSound;

    private OnCollectDo onCollectDo;

    private AudioSource soundPlayer;

	// Use this for initialization
	void Awake () {
	    pickable = true;
        falling = false;
        initVel = Random.insideUnitCircle * 2f;
        initVel.y = Mathf.Abs(initVel.y);

        
        soundPlayer = gameObject.AddComponent<AudioSource>();
        soundPlayer.playOnAwake = false;
        soundPlayer.loop = false;
        soundPlayer.volume = 0.8f;
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 starPoint = transform.position + (GameObject.Find("pawn").transform.position - transform.position).normalized * 0.6f;
        bool colliding = GameObject.Find("pawn").GetComponent<BoxCollider2D>().OverlapPoint(starPoint);
        if (pickable && colliding)
        {
            pickable = false;
            falling = true;
            vel = new Vector3(initVel.x,initVel.y,0f);
            soundPlayer.PlayOneShot(pickupSound);
            onCollectDo();
        }

        if (falling)
        {
            vel -= new Vector3(0f, 9.81f, 0f) * Time.deltaTime;
            transform.Translate(vel*Time.deltaTime);
            if (transform.position.y < -7f)
            {
                Destroy(this);
            }
        }
	}

    public void setOnCollect(OnCollectDo col)
    {
        onCollectDo = col;
    }
}
