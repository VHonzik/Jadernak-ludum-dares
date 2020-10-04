using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wiggle : MonoBehaviour
{
  public bool WiggleOn
  {
    get { return wiggleOn; }
    set
    {
      if (wiggleOn != value)
      {
        Reset();
        wiggleOn = value;
      }
    }
  }

  public bool DisableWiggle { get; set; }

  private bool wiggleOn = true;
  private float amount = 0.4f;
  private float count = 3.0f;
  private float wiggleDuration = 0.9f;
  private float pauseDuration = 2.0f;
  private float wiggleTimer;
  private float pauseTimer;
  private Vector3 initialPos;

  // Start is called before the first frame update
  void Start()
  {
    initialPos = transform.position;
    DisableWiggle = false;
    wiggleTimer = 0.0f;
    pauseTimer = pauseDuration;
  }

  // Update is called once per frame
  void Update()
  {
    if (WiggleOn && !DisableWiggle)
    {
      if (wiggleTimer <= wiggleDuration)
      {
        float t = wiggleTimer / wiggleDuration;

        transform.position = initialPos + new Vector3(amount * Mathf.Sin(t * count * Mathf.PI), 0, 0);

        wiggleTimer += Time.deltaTime;
      }
      else
      {
        pauseTimer -= Time.deltaTime;

        if (pauseTimer < 0.0f)
        {
          wiggleTimer = 0.0f;
          pauseTimer = pauseDuration;
        }
      }
    }
  }

  void OnMouseEnter()
  {
    WiggleOn = false;
  }
  void OnMouseExit()
  {
    WiggleOn = true;
  }
  

  private void Reset()
  {
    wiggleTimer = wiggleDuration + 1.0f;
    pauseTimer = pauseDuration;
  }
}
