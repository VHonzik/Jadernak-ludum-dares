using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroScene : MonoBehaviour
{
  public GameObject Scene;
  public GameObject NextScene;
  public MessageSystem MessageSystem;
  public AudioSource AudioSource;

  public FlippingPuzzle IntroPuzzle;
  public TextEffect ContinueText;
  public GameObject Controls;

  // Start is called before the first frame update
  void Start()
  {
    IntroPuzzle.Initialize(
      new int[2] { 1, 1 },
      new int[2, 2] { { 1, 0 }, { 1, 1 } },
      new int[2] { 0, 0 });
    IntroPuzzle.OnSolved += PuzzleSolved;

    MessageSystem.QueueClip(MessageSystem.EmergencyOverrideClip);
  }

  void Update()
  {
    if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
    {
      if (!AudioSource.isPlaying)
      {
        MessageSystem.QueueClip(MessageSystem.EmergencyOverrideClip);
      }

      AudioSource.volume = Mathf.Clamp01(AudioSource.volume + (Input.GetKeyDown(KeyCode.UpArrow) ? 0.1f : -0.1f));
    }
  }

  void PuzzleSolved()
  {
    StartCoroutine(TriggerNextScene());
  }

  IEnumerator TriggerNextScene()
  {
    Controls.SetActive(false);

    ContinueText.SetText("System override successful...");
    ContinueText.SerialScramble.FixedDuration = true;
    ContinueText.SerialScramble.Duration = 2.5f;
    ContinueText.AnimateSerialScramble();

    yield return new WaitForSeconds(5.0f);
    Scene.SetActive(false);
    if (NextScene)
    {
      NextScene.SetActive(true);
      MessageSystem.IntroFinished();
    }
  }
}
