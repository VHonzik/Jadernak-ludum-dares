using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeResults : MonoBehaviour
{
  private Swipe activeCard;
  public Swipe ActiveCard
  {
    get
    {
      return activeCard;
    }

    set
    {
      activeCard = value;
      if (value != null)
      {
        activeCard.OnSwipeStateChanged += ActiveCardSwipeStateChanged;
      }
    }
  }

  private SpriteRenderer LeftArrow;
  private SpriteRenderer RightArrow;

  // Start is called before the first frame update
  void Start()
  {
    LeftArrow = transform.Find("Left").GetComponent<SpriteRenderer>();
    RightArrow = transform.Find("Right").GetComponent<SpriteRenderer>();
    ChangeSpriteAlpha(LeftArrow, 0.0f);
    ChangeSpriteAlpha(RightArrow, 0.0f);
  }

  // Update is called once per frame
  void Update()
  {
  }

  private void ActiveCardSwipeStateChanged(SwipeState oldSteate, SwipeState newState)
  {
    ChangeSpriteAlpha(LeftArrow, newState == SwipeState.SwippingCommitLeft ? 1.0f : 0.0f);
    ChangeSpriteAlpha(RightArrow, newState == SwipeState.SwippingCommitRight ? 1.0f : 0.0f);

    if (newState == SwipeState.SwipedLeft || newState == SwipeState.SwipedRight)
    {
      ActiveCard.OnSwipeStateChanged -= ActiveCardSwipeStateChanged;
    }
  }

  private void ChangeSpriteAlpha(SpriteRenderer renderer, float alpha)
  {
    var a = Mathf.Clamp01(alpha);
    if (a != renderer.color.a)
    {
      var wantedColor = renderer.color;
      wantedColor.a = a;
      renderer.color = wantedColor;
    }
  }

}
