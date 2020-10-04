using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;

public enum SwipeState
{
  Default,
  SwippingRight,
  SwippingCommitRight,
  SwipedRight,
  SwippingLeft,
  SwippingCommitLeft,
  SwipedLeft,
}

public enum CanSwipe
{
  Both,
  LeftOnly,
  RightOnly
}

public class Swipe : MonoBehaviour
{
  public float SwipeT { get; private set; }
  public float SwipeCommitT { get; private set; }

  private SwipeState swipeState;
  public SwipeState SwipeState
  {
    get { return swipeState; }
    set
    {
      if (value != swipeState)
      {
        var oldState = swipeState;
        swipeState = value;
        OnSwipeStateChanged?.Invoke(oldState, swipeState);

      }
    }
  }

  public CanSwipe CanSwipe { get; set; } = CanSwipe.Both;

  public delegate void SwipeStateChangedDelegate(SwipeState previousState, SwipeState newState);
  public event SwipeStateChangedDelegate OnSwipeStateChanged;

  private bool hovered;
  private bool dragging;
  private bool swipped;
  private Vector3 mouseDragStart;
  private Vector3 dragStartPosition;

  private float maxPixelMovement = 250.0f;
  private float minCommitPixelMovement = 100.0f;
  private float maxWorldMovementX = 2.0f;
  private float maxWorldMovementY = -0.7f;
  private float maxWorldRotationDegrees = 14.0f;

  private Wiggle wiggle;
  private Appear appear;

  // Start is called before the first frame update
  void Start()
  {
    swipped = false;
    hovered = false;
    dragging = false;
    wiggle = GetComponent<Wiggle>();
    appear = GetComponent<Appear>();
    SwipeT = 0.0f;
    SwipeCommitT = 0.0f;
    SwipeState = SwipeState.Default;
  }

  // Update is called once per frame
  void Update()
  {
    if (swipped) return;

    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
    var hover = hit.collider ? hit.collider.gameObject == gameObject : false;

    if (hover != hovered)
    {
      hovered = hover;
      if (!swipped && hovered)
      {
        appear.Highlight = true;
      }
      else if (!dragging && !swipped && !hovered)
      {
        appear.Highlight = false;
      }
    }

    if (Input.GetMouseButtonDown(0) && hovered)
    {
      dragging = true;
      mouseDragStart = Input.mousePosition;
      dragStartPosition = transform.position;
      if (wiggle) wiggle.DisableWiggle = true;
    }
    else if (dragging && Input.GetMouseButtonUp(0))
    {
      dragging = false;

      if (SwipeState == SwipeState.SwippingCommitRight)
      {
        SwipeState = SwipeState.SwipedRight;
        Swipped();
      }
      else if (SwipeState == SwipeState.SwippingCommitLeft)
      {
        SwipeState = SwipeState.SwipedLeft;
        Swipped();
      }
      else
      {
        ResetDragging();
      }
    }

    if (dragging)
    {
      float mouseX = (Input.mousePosition - mouseDragStart).x;
      SwipeT = Mathf.Clamp(mouseX, -maxPixelMovement, maxPixelMovement) / maxPixelMovement;

      if ((SwipeT > 0.0f && CanSwipe == CanSwipe.LeftOnly) || (SwipeT < 0.0f && CanSwipe == CanSwipe.RightOnly))
      {
        return;
      }

      var commitMaxPixelMovement = maxPixelMovement - minCommitPixelMovement;
      SwipeCommitT = Mathf.Clamp((Mathf.Max(Mathf.Abs(mouseX), minCommitPixelMovement) - minCommitPixelMovement), 0.0f, commitMaxPixelMovement) / commitMaxPixelMovement;
      transform.position = dragStartPosition + new Vector3(SwipeT * maxWorldMovementX, SwipeCommitT * maxWorldMovementY, 0);
      transform.rotation = Quaternion.Euler(0, 0, -Mathf.Sign(mouseX) * maxWorldRotationDegrees * SwipeCommitT);


      if (Mathf.Abs(SwipeT) <= Mathf.Epsilon && SwipeCommitT <= 0.0f)
      {
        SwipeState = SwipeState.Default;
      }
      else if (SwipeT > 0.0 && SwipeCommitT <= 0.0f)
      {
        SwipeState = SwipeState.SwippingRight;
      }
      else if (SwipeT < 0.0 && SwipeCommitT <= 0.0f)
      {
        SwipeState = SwipeState.SwippingLeft;
      }
      else if (SwipeT > 0.0 && SwipeCommitT > 0.0f)
      {
        SwipeState = SwipeState.SwippingCommitRight;
      }
      else if (SwipeT < 0.0 && SwipeCommitT > 0.0f)
      {

        SwipeState = SwipeState.SwippingCommitLeft;
      }
    }
  }

  private void ResetDragging()
  {
    if (wiggle) wiggle.DisableWiggle = false;
    SwipeState = SwipeState.Default;
    SwipeT = 0.0f;
    SwipeCommitT = 0.0f;
    transform.position = dragStartPosition;
    transform.rotation = Quaternion.identity;

    if (!hovered)
    {
      appear.Highlight = false;
    }
  }

  void Swipped()
  {
    swipped = true;
    appear.Highlight = false;
    appear.Hide();
  }
}
