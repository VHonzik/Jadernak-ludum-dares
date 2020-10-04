using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppodCard : MonoBehaviour
{
  private GenericCard genericCard;

  private static CardKnowledge cardKnowledge = CardKnowledge.NotEncountered;

  // Start is called before the first frame update
  void Start()
  {
    genericCard = GetComponent<GenericCard>();

    genericCard.InitKnowledge(cardKnowledge);

    genericCard.OnSuggestion += GenericCard_OnSuggestion;
    genericCard.OnSwipe += GenericCard_OnSwipe;


    if (GameMananger.Instance.Turn > 1)
    {
      genericCard.ChangeCanSwipe(CanSwipe.LeftOnly);
      GameMananger.Instance.SuggestTurnConditionNotMet();
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

  // Update is called once per frame
  void Update()
  {

  }
}
