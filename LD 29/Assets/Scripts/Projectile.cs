using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {

    private float acc = 6f;
    private float speed = 0f;
    public delegate void OnHit();
    public OnHit onHit;

    // Use this for initialization
    void Awake()
    {
        onHit = null;
    }


    void Update()
    {
        speed += acc * Time.deltaTime;
        Vector3 dir = new Vector3(Mathf.Sin((180-transform.rotation.eulerAngles.z) * Mathf.Deg2Rad),
            Mathf.Cos((180f - transform.rotation.eulerAngles.z) * Mathf.Deg2Rad), 0f);
        transform.position += dir * speed * Time.deltaTime;

        Vector3 prjPoint = transform.position + (GameObject.Find("pawn").transform.position - transform.position).normalized * 0.5f;
        bool colliding = GameObject.Find("pawn").GetComponent<BoxCollider2D>().OverlapPoint(prjPoint);
        if (colliding && GameObject.Find("pawn").GetComponent<PlayerPawn>().getTargetable())
        {
            GameObject.Find("pawn").GetComponent<PlayerPawn>().disableMovement();
            GameObject.Find("pawn").GetComponent<PlayerPawn>().disableIteraction();
            StartCoroutine(GameObject.Find("pawn").GetComponent<PlayerPawn>().FadeOutPlayer());
            speed = 0f;
            acc = 0f;
            StartCoroutine(FadeOut());
            onHit();
        }

        if (Mathf.Abs(gameObject.transform.position.x) > 10f || Mathf.Abs(gameObject.transform.position.y) > 11f)
        {
            Destroy(this.gameObject);
        }
    }

    public IEnumerator FadeIn()
    {
        gameObject.GetComponent<Animator>().Play("projectil_fadein");
        yield return new WaitForSeconds(0.67f);
    }

    public IEnumerator FadeOut()
    {

        gameObject.GetComponent<Animator>().Play("projectil_fadeout");
        yield return new WaitForSeconds(0.67f);
    }

    public void setAcc(float a)
    {
        acc = a ;
    }

}
