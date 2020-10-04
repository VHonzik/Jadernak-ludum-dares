using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardKnowledge
{
  NotEncountered,
  LeftOutcomeKnown,
  RightOutcomeKnown,
  FullyKnown
}

public class GenericCard : MonoBehaviour
{
  private Swipe swipe;
  private Appear appear;
  private CardKnowledge cardKnowledge = CardKnowledge.NotEncountered;

  public delegate void SuggestionDelegate(bool right);
  public event SuggestionDelegate OnSuggestion;

  public delegate void SwipeDelegate(bool right, bool revealsFoW);
  public event SwipeDelegate OnSwipe;

  public delegate void DisappearDelegate(bool right);
  public event DisappearDelegate OnDisappear;

  // Start is called before the first frame update
  void Start()
  {
    swipe = GetComponent<Swipe>();
    appear = GetComponent<Appear>();
    swipe.OnSwipeStateChanged += Swipe_OnSwipeStateChanged;
    appear.OnStateChanged += Appear_OnStateChanged;
  }

  public void InitKnowledge(CardKnowledge knowledge)
  {
    cardKnowledge = knowledge;

    if (knowledge == CardKnowledge.LeftOutcomeKnown)
    {
      appear.DisableLeftFogOfWar();
    }
    else if (knowledge == CardKnowledge.RightOutcomeKnown)
    {
      appear.DisableRightFogOfWar();
    }
    else if (knowledge == CardKnowledge.FullyKnown)
    {
      appear.DisableLeftFogOfWar();
      appear.DisableRightFogOfWar();
    }
  }

  private void Appear_OnStateChanged(Appear.State newState)
  {
    if ((swipe.SwipeState == SwipeState.SwipedLeft || swipe.SwipeState == SwipeState.SwipedRight) && newState == Appear.State.Hidden)
    {
      OnDisappear?.Invoke(swipe.SwipeState == SwipeState.SwipedRight);
    }
  }

  private void Swipe_OnSwipeStateChanged(SwipeState previousState, SwipeState newState)
  {
    if (newState == SwipeState.SwippingCommitRight)
    {
      if (cardKnowledge == CardKnowledge.RightOutcomeKnown || cardKnowledge == CardKnowledge.FullyKnown)
      {
        OnSuggestion?.Invoke(true);
      }
      else
      {
        GameMananger.Instance.SuggestUnkownStat();
      }
      GameMananger.Instance.SuggestTurnIncrease();
    }
    else if (newState == SwipeState.SwippingCommitLeft)
    {
      if (cardKnowledge == CardKnowledge.LeftOutcomeKnown || cardKnowledge == CardKnowledge.FullyKnown)
      {
        OnSuggestion?.Invoke(false);
      }
      else
      {
        GameMananger.Instance.SuggestUnkownStat();
      }
      GameMananger.Instance.SuggestTurnIncrease();
    }
    else if (newState == SwipeState.Default || newState == SwipeState.SwippingLeft || newState == SwipeState.SwippingRight)
    {
      GameMananger.Instance.ResetSuggestions();
    }
    else if (newState == SwipeState.SwipedRight)
    {
      var firstTime = cardKnowledge == CardKnowledge.LeftOutcomeKnown || cardKnowledge == CardKnowledge.NotEncountered;
      if (firstTime)
      {
        appear.RevealRightFogOfWar();
      }

      GameMananger.Instance.ResetSuggestions();
      GameMananger.Instance.Turn += 1;
      OnSwipe?.Invoke(true, firstTime);
    }
    else if (newState == SwipeState.SwipedLeft)
    {
      var firstTime = cardKnowledge == CardKnowledge.RightOutcomeKnown || cardKnowledge == CardKnowledge.NotEncountered;
      if (firstTime)
      {
        appear.RevealLeftFogOfWar();
      }
      GameMananger.Instance.ResetSuggestions();
      GameMananger.Instance.Turn += 1;
      OnSwipe?.Invoke(false, firstTime);
    }
  }

  // Update is called once per frame
  void Update()
  {

  }

  public void ChangeCanSwipe(CanSwipe newCan)
  {
    swipe.CanSwipe = newCan;
  }

  public static CardKnowledge RevealFoWHelper(CardKnowledge current, bool right)
  {
    switch (current)
    {
      case CardKnowledge.NotEncountered:
        return right ? CardKnowledge.RightOutcomeKnown : CardKnowledge.LeftOutcomeKnown;
      case CardKnowledge.LeftOutcomeKnown:
        return right ? CardKnowledge.FullyKnown : CardKnowledge.LeftOutcomeKnown;
      case CardKnowledge.RightOutcomeKnown:
        return right ? CardKnowledge.RightOutcomeKnown : CardKnowledge.FullyKnown;
      case CardKnowledge.FullyKnown:
        return CardKnowledge.FullyKnown;
      default:
        return current;
    }

  }
}
