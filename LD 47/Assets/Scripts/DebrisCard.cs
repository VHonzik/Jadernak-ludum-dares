using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebrisCard : MonoBehaviour
{
  public GameObject FriendlyFace;
  private GenericCard genericCard;

  private static CardKnowledge cardKnowledge = CardKnowledge.NotEncountered;

  // Start is called before the first frame update
  void Start()
  {
    genericCard = GetComponent<GenericCard>();

    genericCard.InitKnowledge(cardKnowledge);

    genericCard.OnDisappear += GenericCard_OnDisappear;
    genericCard.OnSuggestion += GenericCard_OnSuggestion;
    genericCard.OnSwipe += GenericCard_OnSwipe;


    if (GameMananger.Instance.Turn > 5)
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
      GameMananger.Instance.EnqueueCards(new[] { FriendlyFace });      
    }
  }

  private void GenericCard_OnSuggestion(bool right)
  {
    if (right)
    {
      GameMananger.Instance.ResetSuggestions();
    }
    else
    {
      GameMananger.Instance.SuggestNewHealth(0);
    }
  }

  private void GenericCard_OnDisappear(bool right)
  {
    if (!right)
    {
      GameMananger.Instance.Death();
    }
  }

  // Update is called once per frame
  void Update()
  {

  }
}
