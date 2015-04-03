using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerPawn : MonoBehaviour {

    private const float maxSpeed = 4f;
    private const float acc = 15f;
    private const float deacc = 40f;
    private Rigidbody2D rigidBody;

    private bool movementEnabled;
    private bool iteractionEnabled;
    private bool isJumping;
    private bool isTargetable;

	void Awake () {
        rigidBody = gameObject.GetComponent<Rigidbody2D>();
        movementEnabled = false;
        iteractionEnabled = false;
        isJumping = false;
        isTargetable = true;
	}   
	
	void Update () {
        UpdateMovement(); 
	}

    public void MakeJump(Vector2 direction,float power)
    {        
        isJumping = true;
        rigidBody.AddForce(direction * power);
        StartCoroutine(jumpEvent());
    }

    private IEnumerator jumpEvent()
    {
        while (Mathf.Abs(rigidBody.velocity.y) < 0.2f)
        {
            yield return null;
        }
        
        while (rigidBody.velocity.y > 0.1f )
        {
            yield return null;
        }
        while (Mathf.Abs(rigidBody.velocity.y) < 0.1f)
        {
            yield return null;
        }
        while (rigidBody.velocity.y < -0.1f)
        {
            yield return null;
        }
        isJumping = false;
        enableIteraction();
    }

    public IEnumerator FadeInPlayer()
    {
        gameObject.GetComponent<Animator>().Play("pawn_fadingin");
        yield return new WaitForSeconds(2f);
        isTargetable = true;
    }

    public IEnumerator FadeOutPlayer()
    {
        isTargetable = false;
        gameObject.GetComponent<Animator>().Play("pawn_fadingout");
        yield return new WaitForSeconds(2f);
    }

    public void enableMovement()
    {
        movementEnabled = true;
    }

    public void disableMovement()
    {
        rigidBody.velocity = Vector3.zero;
        movementEnabled = false;
    }

    public void enableIteraction()
    {
        iteractionEnabled = true;
    }

    public void disableIteraction()
    {
        iteractionEnabled = false;
    }

    public bool getIteractionEnabled()
    {
        return iteractionEnabled;
    }

    public bool getTargetable()
    {
        return isTargetable;
    }

    private void UpdateMovementForce()
    {
 
    }

    private void UpdateMovement()
    {
       
        if (Input.GetAxis("Horizontal") > 0.01f && movementEnabled)
        {
            float diff = Mathf.Abs(maxSpeed - rigidBody.velocity.x);
            rigidBody.velocity += new Vector2(Mathf.Min(acc * Time.deltaTime,diff),0);
        }
        else if (Input.GetAxis("Horizontal") < -0.01f && movementEnabled)
        {
            float diff = Mathf.Abs(-maxSpeed - rigidBody.velocity.x);
            rigidBody.velocity -= new Vector2(Mathf.Min(acc * Time.deltaTime, diff), 0);
            
        }

        if (!isJumping && Mathf.Abs(Input.GetAxis("Horizontal"))<=0.01f)
        {
            float diff = Mathf.Abs(rigidBody.velocity.x);
            float newSpeed = -1f * System.Math.Sign(rigidBody.velocity.x) * Mathf.Min(deacc * Time.deltaTime, diff);
            //rigidBody.velocity = new Vector2(newSpeed,rigidBody.velocity.y);
            rigidBody.velocity = new Vector2(0, rigidBody.velocity.y);

        }

    }
}
