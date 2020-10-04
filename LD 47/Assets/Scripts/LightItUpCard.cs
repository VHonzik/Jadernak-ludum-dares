using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightItUpCard : MonoBehaviour
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

    if (GameMananger.Instance.Aim < 2)
    {
      GameMananger.Instance.SuggestAimConditionNotMet();
      genericCard.ChangeCanSwipe(CanSwipe.LeftOnly);
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
      GameMananger.Instance.EnqueueCards(new[] { TheyKeepComingCard });
    }
    else
    {
      GameMananger.Instance.EnqueueCards(new[] { ArtOfMeleeCard });
    }
  }

  // Update is called once per frame
  void Update()
  {

  }
}
