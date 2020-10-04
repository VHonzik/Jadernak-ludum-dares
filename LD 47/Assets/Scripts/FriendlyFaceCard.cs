using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendlyFaceCard : MonoBehaviour
{
  public GameObject DeadlyFaceCard;

  private GenericCard genericCard;
  private static CardKnowledge cardKnowledge = CardKnowledge.NotEncountered;

  // Start is called before the first frame update
  void Start()
  {
    genericCard = GetComponent<GenericCard>();

    genericCard.InitKnowledge(cardKnowledge);

    genericCard.OnSwipe += GenericCard_OnSwipe;

    if (!GameMananger.Instance.AlenNameKnow)
    {
      genericCard.ChangeCanSwipe(CanSwipe.LeftOnly);
    }
    else
    {
      genericCard.ChangeCanSwipe(CanSwipe.Both);
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
      GameMananger.Instance.EnqueueCards(new[] { DeadlyFaceCard });
    }
    else
    {
      GameMananger.Instance.AlenNameKnow = true;
      GameMananger.Instance.AlenAlive = false;
      GameMananger.Instance.EnqueueCards(new[] { DeadlyFaceCard });
    }
  }

  // Update is called once per frame
  void Update()
  {

  }
}
