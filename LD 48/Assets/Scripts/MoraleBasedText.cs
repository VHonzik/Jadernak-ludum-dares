using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoraleBasedText : MonoBehaviour
{
  public int Screen;
  public int MoralThreshold;
  public TMPro.TextMeshProUGUI Text;
  public string EnoughText;
  public string NotEnoughText;

  void Awake()
  {
    GameManager.Instance.NavigationSystem.ScreenChanged += NavigationSystem_ScreenChanged;
  }

  private void NavigationSystem_ScreenChanged(object sender, Listonos.NavigationSystem.NavigationSystem<int>.ScreenChangedEventArgs e)
  {
    if (e.NewScreen == Screen)
    {
      Text.text = GameManager.Instance.Decisions.Morale > MoralThreshold ? EnoughText : NotEnoughText;
    }
  }
}
