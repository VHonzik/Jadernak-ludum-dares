using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastBreathCard : MonoBehaviour
{
  public GameObject ThanksForPlayingCard;

  private GenericCard genericCard;
  private static CardKnowledge cardKnowledge = CardKnowledge.NotEncountered;

  // Start is called before the first frame update
  void Start()
  {
    genericCard = GetComponent<GenericCard>();

    genericCard.InitKnowledge(cardKnowledge);

    genericCard.OnSwipe += GenericCard_OnSwipe;
    genericCard.OnSuggestion += GenericCard_OnSuggestion;
    genericCard.OnDisappear += GenericCard_OnDisappear;
  }

  private void GenericCard_OnDisappear(bool right)
  {
    if (!right)
    {
      GameMananger.Instance.Death();
    }
  }

  private void GenericCard_OnSuggestion(bool right)
  {
    if (right)
    {
      GameMananger.Instance.SuggestNewArmor(GameMananger.Instance.Armor - 50);
    }
    else
    {
      GameMananger.Instance.ResetSuggestions();
    }
  }

  private void GenericCard_OnSwipe(bool right, bool revealsFoW)
  {
    if (revealsFoW)
    {
      cardKnowledge = GenericCard.RevealFoWHelper(cardKnowledge, right);
    }

    if (right)
    {
      GameMananger.Instance.Armor -= 50;
      GameMananger.Instance.EnqueueCards(new[] { ThanksForPlayingCard });
    }
  }
}
