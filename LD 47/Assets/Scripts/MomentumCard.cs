using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MomentumCard : MonoBehaviour
{
  public GameObject TheyKeepComingCard;
  public GameObject ArtOfMeleeCard;
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
      GameMananger.Instance.ResetSuggestions();
    }
    else
    {
      GameMananger.Instance.SuggestNewArmor(GameMananger.Instance.Armor - 10);
    }
  }

  private void GenericCard_OnSwipe(bool right, bool revealsFoW)
  {
    if (revealsFoW)
    {
      cardKnowledge = GenericCard.RevealFoWHelper(cardKnowledge, right);
    }

    if (!right)
    {
      GameMananger.Instance.Armor -= 10;
      GameMananger.Instance.EnqueueCards(new[] { TheyKeepComingCard });
    }
    else
    {
      GameMananger.Instance.EnqueueCards(new[] { ArtOfMeleeCard });
    }
  }
}
