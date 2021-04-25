using System;
using UnityEngine;

[Serializable]
public class DecisionMade
{
  public Decision Decision;
  public int Choice;
}

public class ChoiceLockedScreen : MonoBehaviour
{
  public int Screen;
  public DecisionMade[] Choices;

  // Start is called before the first frame update
  void Awake()
  {
    GameManager.Instance.AfterScreenChanged += NavigationSystem_ScreenChanged;
  }

  private void NavigationSystem_ScreenChanged(object sender, Listonos.NavigationSystem.NavigationSystem<int>.ScreenChangedEventArgs e)
  {
    if (e.NewScreen == Screen)
    {
      foreach (var choice in Choices)
      {
        if (GameManager.Instance.Decisions.MadeDecisionWithDifferentChoice(choice.Decision, choice.Choice))
        {
          GameManager.Instance.RedirectPage(GameManager.Instance.DecisionAlreadyMadePage);
          break;
        }
      }

    }
  }
}
