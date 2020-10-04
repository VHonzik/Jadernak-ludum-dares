using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterruptedHopeCard : MonoBehaviour
{
  public GameObject ImpendingDoomCard;
  private GenericCard genericCard;
  private static CardKnowledge cardKnowledge = CardKnowledge.NotEncountered;

  // Start is called before the first frame update
  void Start()
  {
    genericCard = GetComponent<GenericCard>();

    genericCard.InitKnowledge(cardKnowledge);

    genericCard.OnSwipe += GenericCard_OnSwipe;
    genericCard.OnDisappear += GenericCard_OnDisappear;
    genericCard.OnSuggestion += GenericCard_OnSuggestion;

    if (GameMananger.Instance.Aim < 3)
    {
      GameMananger.Instance.SuggestAimConditionNotMet();
      genericCard.ChangeCanSwipe(CanSwipe.LeftOnly);
    }
    else
    {
      genericCard.ChangeCanSwipe(CanSwipe.Both);
    }
  }

  private void GenericCard_OnSuggestion(bool right)
  {
    if (right)
    {
      GameMananger.Instance.SuggestNewArmor(GameMananger.Instance.Armor - 20);
    }
    else
    {
      GameMananger.Instance.ResetSuggestions();
    }
  }

  private void GenericCard_OnDisappear(bool right)
  {
    if (!right)
    {
      GameMananger.Instance.Death();
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
      GameMananger.Instance.Armor -= 20;
      GameMananger.Instance.EnqueueCards(new[] { ImpendingDoomCard });
    }
  }
}
