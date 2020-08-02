using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class FlippingPuzzle : MonoBehaviour
{
  public delegate void OnSolvedDelegate();
  public delegate void OnFlipDelegate(int flipCount);

  public event OnSolvedDelegate OnSolved;
  public event OnFlipDelegate OnFlip;

  private TextMeshProUGUI _text;
  private TextEffect _textEffect;

  private int[] _initialState;
  private int[,] _flipMatrix;
  private int[] _solution;

  private int[] _state;
  private int _selected;
  private int _length;
  private int _flipCount;

  public bool InputActive { get; set; }

  private void UpdateText()
  {
    string result = "";
    for (int i = 0; i < _state.Length; i++)
    {
      string colorFormat = _state[i] == _solution[i] ? "<color=#56B453>{0}</color>" : "<color=#FFFFFF>{0}</color>";
      string selectionFormat = i == _selected && InputActive ? "[{0}]" : " {0} ";

      result += string.Format(selectionFormat, string.Format(colorFormat, _state[i]));
    }
    _text.text = result;
  }

  public void Initialize(int[] initialState, int[,] flipMatrix, int[] solution)
  {
    _initialState = (int[])initialState.Clone();
    _flipMatrix = (int[,])flipMatrix.Clone();
    _solution = (int[])solution.Clone();
    _state = (int[])initialState.Clone();
    _length = _state.Length;
    _selected = 0;
    InputActive = true;
    UpdateText();
  }
  public int[] GetState()
  {
    return _state;
  }

  // Start is called before the first frame update
  void Awake()
  {
    _text = GetComponent<TextMeshProUGUI>();
    _textEffect = GetComponent<TextEffect>();
    InputActive = false;
    _text.text = "";
    _flipCount = 0;
  }

  // Update is called once per frame
  void Update()
  {
    if (InputActive)
    {
      if (Input.GetKeyDown(KeyCode.LeftArrow))
      {
        _selected = _selected - 1;
        if (_selected < 0)
        {
          _selected = _length - 1;
        }
        UpdateText();
      }
      else if (Input.GetKeyDown(KeyCode.RightArrow))
      {
        _selected = _selected + 1;
        if (_selected > _length - 1)
        {
          _selected = 0;
        }
        UpdateText();
      }

      if (Input.GetKeyDown(KeyCode.F))
      {
        _flipCount++;
        bool solved = true;
        for (int i = 0; i < _length; i++)
        {
          if (_flipMatrix[_selected, i] != 0)
          {
            _state[i] = _state[i] == 0 ? 1 : 0;
          }

          if (_state[i] != _solution[i])
          {
            solved = false;
          }
        }

        OnFlip?.Invoke(_flipCount);

        UpdateText();

        if (solved)
        {
          InputActive = false;
          UpdateText();

          _textEffect.Blinking.DurationOnOff = 0.5f;
          _textEffect.AnimateBlinking();
          OnSolved?.Invoke();
        }
      }
    }
  }
}
