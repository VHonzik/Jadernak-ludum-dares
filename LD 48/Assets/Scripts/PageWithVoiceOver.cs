using Listonos.AudioSystem;
using UnityEngine;

public class PageWithVoiceOver : MonoBehaviour
{
  public AudioClip VoiceOver;
  public int Screen;

  // Start is called before the first frame update
  void Awake()
  {
    GameManager.Instance.AfterScreenChanged += Instance_AfterScreenChanged;
  }

  private void Instance_AfterScreenChanged(object sender, Listonos.NavigationSystem.NavigationSystem<int>.ScreenChangedEventArgs e)
  {
    if (e.NewScreen == Screen && VoiceOver != null)
    {
      AudioManager.Instance.PlayMusicClip(VoiceOver);
    }
  }

}
