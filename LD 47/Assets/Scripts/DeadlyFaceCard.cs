using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadlyFaceCard : MonoBehaviour
{
  public GameObject InterruptedHopeCard;
  private GenericCard genericCard;
  private static CardKnowledge cardKnowledge = CardKnowledge.NotEncountered;

  // Start is called before the first frame update
  void Start()
  {
    genericCard = GetComponent<GenericCard>();

    genericCard.InitKnowledge(cardKnowledge);

    genericCard.OnSwipe += GenericCard_OnSwipe;
    genericCard.OnSuggestion += GenericCard_OnSuggestion;
  }

  private void GenericCard_OnSuggestion(bool right)
  {
    if (right)
    {
      GameMananger.Instance.SuggestNewArmor(GameMananger.Instance.Armor - 20);
    }
    else
    {
      GameMananger.Instance.SuggestNewArmor(GameMananger.Instance.Armor - 30);
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
      GameMananger.Instance.EnqueueCards(new[] { InterruptedHopeCard });
    }
    else
    {
      GameMananger.Instance.Armor -= 30;
      GameMananger.Instance.EnqueueCards(new[] { InterruptedHopeCard });
    }
  }
}
