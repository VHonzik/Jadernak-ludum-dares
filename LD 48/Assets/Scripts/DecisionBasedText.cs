using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecisionBasedText : MonoBehaviour
{
  public int Screen;
  public Decision Decision;
  public TMPro.TextMeshProUGUI[] ChoicesTexts;

  void Awake()
  {
    GameManager.Instance.NavigationSystem.ScreenChanged += NavigationSystem_ScreenChanged;
  }

  private void NavigationSystem_ScreenChanged(object sender, Listonos.NavigationSystem.NavigationSystem<int>.ScreenChangedEventArgs e)
  {
    if (e.NewScreen == Screen && GameManager.Instance.Decisions.MadeDecision(Decision))
    {
      Debug.Assert(ChoicesTexts.Length <= GameManager.Instance.Decisions.GetDecisionData(Decision).MoralePenality.Length);
      for (int i = 0; i < ChoicesTexts.Length; i++)
      {
        ChoicesTexts[i].gameObject.SetActive(GameManager.Instance.Decisions.MadeDecisionWithChoice(Decision, i));
      }
    }
  }
}
