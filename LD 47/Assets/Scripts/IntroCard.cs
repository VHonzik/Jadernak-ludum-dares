using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroCard : MonoBehaviour
{
  private Appear appear;
  private Swipe swipe;

  // Start is called before the first frame update
  void Start()
  {
    appear = GetComponent<Appear>();
    swipe = GetComponent<Swipe>();

    appear.OnStateChanged += Appear_OnStateChanged;
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
    if (swippedRight)
    {
      GameMananger.Instance.StartGame();
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

  // Update is called once per frame
  void Update()
  {

  }
}
