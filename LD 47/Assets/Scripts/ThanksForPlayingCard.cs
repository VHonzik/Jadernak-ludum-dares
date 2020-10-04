using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThanksForPlayingCard : MonoBehaviour
{
  private Appear appear;
  private Swipe swipe;

  // Start is called before the first frame update
  void Start()
  {
    appear = GetComponent<Appear>();
    swipe = GetComponent<Swipe>();

    appear.OnStateChanged += Appear_OnStateChanged;

    GameMananger.Instance.Victory();
  }

  private void Appear_OnStateChanged(Appear.State newState)
  {
    if ((swipe.SwipeState == SwipeState.SwipedLeft || swipe.SwipeState == SwipeState.SwipedRight) && newState == Appear.State.Hidden)
    {
      OnCardDisappeared(swipe.SwipeState == SwipeState.SwipedRight);
    }
  }

  private void OnCardDisappeared(bool swippedRight)
  {
    if (!swippedRight)
    {
      GameMananger.Instance.Restart();
    }
    else
    {
#if UNITY_EDITOR
      UnityEditor.EditorApplication.isPlaying = false;
#else
      Application.Quit();
#endif
    }
  }
}
