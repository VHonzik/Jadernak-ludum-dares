using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Appear : MonoBehaviour
{
  public enum State
  {
    Visible,
    RevealingLeftFoW,
    RevealingRightFoW,
    Disappearing,
    Hidden,
    Appearing
  }

  public delegate void StateChangedDelegate(State newState);
  public event StateChangedDelegate OnStateChanged;
  public bool StartVisible;

  private State state;
  public State CurrentState
  {
    get
    {
      return state;
    }
    private set
    {
      if (state != value)
      {
        state = value;
        OnStateChanged?.Invoke(state);
      }
    }
  }

  private List<SpriteRenderer> spriteRenderers = new List<SpriteRenderer>();
  private float apppearAnimationT;
  private float revealAnimationT;
  private float appearDuration = 0.25f;
  private float revealDuration = 2.00f;
  private float revealAlphaDuration = 0.50f;
  private Color lightenColor = new Color(0.1f, 0.1f, 0.1f);

  private SpriteRenderer leftFowSprite;
  private SpriteRenderer rightFowSprite;
  private Collider2D cardCollider;

  private bool highlight = false;
  public bool Highlight
  {
    get
    {
      return highlight;
    }

    set
    {
      if (value != highlight)
      {
        highlight = value;
        foreach (var sprite in spriteRenderers)
        {
          HighlightSprite(sprite, Highlight);
        }
      }
    }
  }

  // Start is called before the first frame update
  void Start()
  {
    transform.GetComponentsInChildren(spriteRenderers);

    var leftFow = transform.Find("LeftFoW");
    if (leftFow != null)
    {
      leftFowSprite = leftFow.GetComponent<SpriteRenderer>();
    }

    var rightFow = transform.Find("RightFow");
    if (rightFow != null)
    {
      rightFowSprite = rightFow.GetComponent<SpriteRenderer>();
    }

    revealAnimationT = 1.0f;

    cardCollider = GetComponent<BoxCollider2D>() as Collider2D;


    if (StartVisible)
    {
      CurrentState = State.Visible;
      apppearAnimationT = 1.0f;
      cardCollider.enabled = true;
    }
    else
    {
      CurrentState = State.Hidden;
      apppearAnimationT = 0.0f;
      cardCollider.enabled = false;

      foreach (var sprite in spriteRenderers)
      {
        ChangeSpriteAlpha(sprite, apppearAnimationT);
      }
    }
  }

  // Update is called once per frame
  void Update()
  {
    if (CurrentState == State.Appearing)
    {
      apppearAnimationT = Mathf.Clamp01(apppearAnimationT + Time.deltaTime / appearDuration);

      foreach (var sprite in spriteRenderers)
      {
        ChangeSpriteAlpha(sprite, apppearAnimationT);
      }

      if (apppearAnimationT >= 1.0f)
      {
        CurrentState = State.Visible;
        cardCollider.enabled = true;
      }
    }
    else if (CurrentState == State.Disappearing)
    {
      apppearAnimationT = Mathf.Clamp01(apppearAnimationT - Time.deltaTime / appearDuration);

      foreach (var sprite in spriteRenderers)
      {
        if (sprite == leftFowSprite || sprite == rightFowSprite)
        {
       
        }
        else
        {
          ChangeSpriteAlpha(sprite, apppearAnimationT);
        }
      }

      if (apppearAnimationT <= 0.0f)
      {
        foreach (var sprite in spriteRenderers)
        {
          if (sprite == leftFowSprite || sprite == rightFowSprite)
          {
            ChangeSpriteAlpha(sprite, apppearAnimationT);
          }
        }
        CurrentState = State.Hidden;
        cardCollider.enabled = false;
      }
    }
    else if (CurrentState == State.RevealingLeftFoW || CurrentState == State.RevealingRightFoW)
    {
      var sprite = CurrentState == State.RevealingLeftFoW ? leftFowSprite : rightFowSprite;
      revealAnimationT = Mathf.Clamp01(revealAnimationT - Time.deltaTime / revealDuration);
      var alphaT = Mathf.Clamp01(revealAnimationT - (1.0f - (revealAlphaDuration / revealDuration))) / (revealAlphaDuration / revealDuration);
      ChangeSpriteAlpha(sprite, Mathf.Clamp01(alphaT));

      if (revealAnimationT <= 0.0f)
      {
        if (CurrentState == State.RevealingLeftFoW)
        {
          DisableLeftFogOfWar();
        }
        else
        {
          DisableRightFogOfWar();
        }

        CurrentState = State.Disappearing;
      }
    }
  }

  public void Hide()
  {
    if (CurrentState == State.Appearing || CurrentState == State.Visible)
    {
      CurrentState = State.Disappearing;
    }
  }

  public void Show()
  {
    if (CurrentState == State.Disappearing || CurrentState == State.Hidden)
    {
      CurrentState = State.Appearing;
    }
  }

  public void ShowInstantly()
  {
    CurrentState = State.Visible;
    apppearAnimationT = 1.0f;
    foreach (var sprite in spriteRenderers)
    {
      ChangeSpriteAlpha(sprite, apppearAnimationT);
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
  private void HighlightSprite(SpriteRenderer renderer, bool highlighted)
  {
    renderer.color = renderer.color + (highlighted ? 1 : -1) * lightenColor;
  }

  public void DisableLeftFogOfWar()
  {
    leftFowSprite.gameObject.SetActive(false);
    spriteRenderers.Remove(leftFowSprite);
  }

  public void DisableRightFogOfWar()
  {
    rightFowSprite.gameObject.SetActive(false);
    spriteRenderers.Remove(rightFowSprite);
  }

  public void RevealLeftFogOfWar()
  {
    CurrentState = State.RevealingLeftFoW;
    revealAnimationT = 1.0f;
  }

  public void RevealRightFogOfWar()
  {
    CurrentState = State.RevealingRightFoW;
    revealAnimationT = 1.0f;
  }
}
