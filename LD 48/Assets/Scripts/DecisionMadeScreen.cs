using System;
using UnityEngine;


public class DecisionMadeScreen : MonoBehaviour
{
  public int Screen;
  public Decision Decision;
  public int Choice;

  void Awake()
  {
    GameManager.Instance.NavigationSystem.ScreenChanged += NavigationSystem_ScreenChanged;
  }

  private void NavigationSystem_ScreenChanged(object sender, Listonos.NavigationSystem.NavigationSystem<int>.ScreenChangedEventArgs e)
  {
    if (e.NewScreen == Screen && !GameManager.Instance.Decisions.MadeDecision(Decision))
    {
      GameManager.Instance.Decisions.MakeDecision(Decision, Choice);
    }
  }
}
