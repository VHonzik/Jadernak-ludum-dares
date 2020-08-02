using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapeMenu : MonoBehaviour
{
  public GameObject EscapeScene;

  // Start is called before the first frame update
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {
    if (Input.GetKeyDown(KeyCode.Escape))
    {
      EscapeScene.SetActive(!EscapeScene.activeInHierarchy);
    }

    if (EscapeScene.activeInHierarchy && Input.GetKeyDown(KeyCode.Return))
    {
      Application.Quit();
    }
  }
}
