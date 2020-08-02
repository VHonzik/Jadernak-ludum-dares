using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogNavigation : MonoBehaviour
{
  public GameObject LogContent;

  private RectTransform _rectTransform;
  private Vector2 _initialOffsetMin;
  private Vector2 _initialOffsetMax;

  private float _scrollingOffset;
  private const float _scrollingOffsetMin = 0.0f;
  private const float _scrollingOffsetMax = 110.0f;
  private const float _scrollingSpeed = 50.0f;

  private bool _navigationActive;
  public bool NavigationActive
  {
    get { return _navigationActive; }
    set
    {
      _navigationActive = value;
    }
  }

  // Start is called before the first frame update
  void Start()
  {
    _rectTransform = LogContent.GetComponent<RectTransform>();
    _initialOffsetMin = _rectTransform.offsetMin;
    _initialOffsetMax = _rectTransform.offsetMax;
    _scrollingOffset = 0.0f;
  }

  // Update is called once per frame
  void Update()
  {
    if (Input.GetKeyDown(KeyCode.DownArrow))
    {
      _scrollingOffset = Mathf.Min(_scrollingOffset + _scrollingSpeed, _scrollingOffsetMax);
      _rectTransform.offsetMin = _initialOffsetMin + new Vector2(0, -_scrollingOffset);
      _rectTransform.offsetMax = _initialOffsetMax + new Vector2(0, _scrollingOffset);
    }
    else if (Input.GetKeyDown(KeyCode.UpArrow))
    {
      _scrollingOffset = Mathf.Max(_scrollingOffset - _scrollingSpeed, _scrollingOffsetMin);
      _rectTransform.offsetMin = _initialOffsetMin + new Vector2(0, -_scrollingOffset);
      _rectTransform.offsetMax = _initialOffsetMax + new Vector2(0, _scrollingOffset);
    }
  }
}
