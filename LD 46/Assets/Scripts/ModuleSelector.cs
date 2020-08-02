using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModuleSelector : MonoBehaviour
{
  public SpaceStationModule Module;

  private Image _image;

  private void Awake()
  {
    _image = GetComponent<Image>();
  }

  public void SetEnabled(bool enabled)
  {
    _image.enabled = enabled;
  }
}
