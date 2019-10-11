using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour {

    private bool MovementEnabled = true;

    public float MovementSpeed;

    private float MovementTimer;
    public float MovementTimePerMove;

    private float[] LastFrameAxes;

    bool FirstFrame;
    bool JustUnblocked;

    private List<MonoBehaviour> RequestingMovementStop;

    public delegate void InteractableDelegate();

    private Dictionary<GameObject, InteractableDelegate> InteractableMap;

    void Awake()
    {
        RequestingMovementStop = new List<MonoBehaviour>();
        InteractableMap = new Dictionary<GameObject, InteractableDelegate>();
        MovementTimePerMove = 1.0f / MovementSpeed;
        MovementTimer = 0.0f;
        LastFrameAxes = new float[2] { 0.0f, 0.0f };
        FirstFrame = false;
        JustUnblocked = false;
    }

    // Use this for initialization
    void Start () {

    }

    // Update is called once per frame
    void Update() {
        bool nonZeroVertical = false;
        bool nonZeroHorizontal = false;

        if (MovementTimer > 0.0f)
        {
            MovementTimer -= Time.deltaTime;
            MovementTimer = Mathf.Max(MovementTimer, 0.0f);
        }

        var vertical = Input.GetAxis("Vertical");
        var horizontal = Input.GetAxis("Horizontal");

        FirstFrame = false;

        if (!Mathf.Approximately(vertical, 0.0f))
        {
            nonZeroVertical = true;
            if (Mathf.Approximately(LastFrameAxes[0], 0.0f) && !JustUnblocked)
            {
                FirstFrame = true;
            }
        }

        if (!Mathf.Approximately(horizontal, 0.0f))
        {
            nonZeroHorizontal = true;
            if (Mathf.Approximately(LastFrameAxes[1], 0.0f) && !JustUnblocked)
            {
                FirstFrame = true;
            }
        }

        JustUnblocked = false;

        if (MovementEnabled == true && (nonZeroVertical || nonZeroHorizontal) && MovementTimer <= 0.0f)
        {
            // Try the dominant first, then weaker
            List<Vector3> changes = new List<Vector3>();
            if (Mathf.Abs(vertical) > Mathf.Abs(horizontal))
            {
                if (nonZeroVertical) changes.Add(Mathf.Sign(vertical) * Vector3.up);
                if (nonZeroHorizontal) changes.Add(Mathf.Sign(horizontal) * Vector3.right);
            }
            else
            {
                if (nonZeroHorizontal) changes.Add(Mathf.Sign(horizontal) * Vector3.right);
                if (nonZeroVertical) changes.Add(Mathf.Sign(vertical) * Vector3.up);
            }

            foreach(var change in changes)
            {
                var hit = Physics2D.Linecast(transform.position, transform.position + change, ~(1 << 8));
                var walkable = Physics2D.OverlapCircleNonAlloc(transform.position + change, 0.0f, new Collider2D[1], 1 << 8) > 0;

                if (!hit && walkable)
                {
                    MovementTimer = MovementTimePerMove;
                    transform.position += change;
                    break;
                }
                else if (hit && FirstFrame)
                {
                    var go = hit.transform.gameObject;
                    var parent = hit.transform.parent ? hit.transform.parent.gameObject : null;

                    if (InteractableMap.ContainsKey(go))
                    {
                        InteractableMap[go]();
                    }
                    else if (parent && InteractableMap.ContainsKey(parent))
                    {
                        InteractableMap[parent]();
                    }
                    break;
                }
            }
        }

        LastFrameAxes[0] = vertical;
        LastFrameAxes[1] = horizontal;
    }

    public void BlockInput(MonoBehaviour who)
    {
        if (!RequestingMovementStop.Contains(who))
        {
            RequestingMovementStop.Add(who);
            if (MovementEnabled)
            {
                MovementEnabled = false;
            }
        }
    }

    public void UnblockInput(MonoBehaviour who)
    {
        if (RequestingMovementStop.Contains(who))
        {
            RequestingMovementStop.Remove(who);
            if (!MovementEnabled && RequestingMovementStop.Count == 0)
            {
                MovementEnabled = true;
                MovementTimer = MovementTimePerMove;
                JustUnblocked = true;
            }
        }
    }

    public IEnumerator Teleport(Vector3 destination, float duration)
    {
        BlockInput(this);
        var startPosition = transform.position;
        var t = 0.0f;

        while(t < duration)
        {
            t += Time.deltaTime;
            t = Mathf.Min(t, duration);
            transform.position = Vector3.Lerp(startPosition, destination, t / duration);
            yield return null;
        }

        UnblockInput(this);
    }

    public IEnumerator PushAway(Vector3 from, int minimalDistance, float duration)
    {
        BlockInput(this);

        // See which way to push
        // First try directly away

        Vector3 direction = (transform.position - from).normalized;
        bool possible = true;

        for(var dir=0; dir < 4; dir++)
        {
            possible = true;
            direction = Quaternion.Euler(0, 0, dir * 90.0f) * direction;
            direction.Normalize();
            for (var i = 1; i <= minimalDistance; i++)
            {
                var walkable = Physics2D.OverlapCircleNonAlloc(transform.position + direction * i, 0.0f, new Collider2D[1], 1 << 8) > 0;
                var obstacle = Physics2D.OverlapCircleNonAlloc(transform.position + direction * i, 0.0f, new Collider2D[1], ~(1 << 8)) > 0;
                if (!walkable || obstacle)
                {
                    possible = false;
                    break;
                }
            }

            if (possible)
            {
                break;
            }
        }

        if (possible)
        {
            var startPosition = transform.position;
            var t = 0.0f;

            while (t < duration)
            {
                t += Time.deltaTime;
                t = Mathf.Min(t, duration);
                transform.position = Vector3.Lerp(startPosition, startPosition + direction * minimalDistance, t / duration);
                yield return null;
            }
        }

        UnblockInput(this);
    }

    public void RegisterInteractable(GameObject go, InteractableDelegate interact)
    {
        if (!InteractableMap.ContainsKey(go))
        {
            InteractableMap.Add(go, interact);
        }
    }

    public void UnregisterInteractable(GameObject go)
    {
        InteractableMap.Remove(go);
    }

}
