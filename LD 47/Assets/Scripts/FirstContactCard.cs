using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstContactCard : MonoBehaviour
{
  private GenericCard genericCard;
  public GameObject LightItUpCard;
  public GameObject MomentumCard;

  private static CardKnowledge cardKnowledge = CardKnowledge.NotEncountered;

  // Start is called before the first frame update
  void Start()
  {
    genericCard = GetComponent<GenericCard>();

    genericCard.InitKnowledge(cardKnowledge);

    genericCard.OnSwipe += GenericCard_OnSwipe;
  }

  private void GenericCard_OnSwipe(bool right, bool revealsFoW)
  {
    if (revealsFoW)
    {
      cardKnowledge = GenericCard.RevealFoWHelper(cardKnowledge, right);
    }

    if (right)
    {
      GameMananger.Instance.EnqueueCards(new[] { LightItUpCard });
    }
    else
    {
      GameMananger.Instance.EnqueueCards(new[] { MomentumCard });
    }
  }

  // Update is called once per frame
  void Update()
  {

  }
}
