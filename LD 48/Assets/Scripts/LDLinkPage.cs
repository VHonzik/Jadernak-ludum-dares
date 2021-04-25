using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LDLinkPage : MonoBehaviour
{
  public int Page;
  public string URL = "";
  public TMPro.TextMeshProUGUI LinkText;

  // Start is called before the first frame update
  void Awake()
  {
    Debug.AssertFormat(URL.Length > 0, "TODO add page link");
    GameManager.Instance.AfterScreenChanged += Instance_AfterScreenChanged;
    LinkText.text = URL;
  }

  private void Instance_AfterScreenChanged(object sender, Listonos.NavigationSystem.NavigationSystem<int>.ScreenChangedEventArgs e)
  {
    if (e.NewScreen == Page && URL.Length > 0)
    {
      Application.OpenURL(URL);
    }
  }
}
