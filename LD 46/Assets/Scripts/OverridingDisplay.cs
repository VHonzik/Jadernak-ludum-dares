using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OverridingDisplay : MonoBehaviour
{
  public GameObject Container;
  public FlippingPuzzle Puzzle;
  public GameObject FlipControl;
  public GameObject FlipMatrix;

  private GameState _gameState;
  private TextMeshProUGUI _flipMatrixText;

  private bool _displayActive;
  public bool DisplayActive
  {
    get
    {
      return _displayActive;
    }
    set
    {
      _displayActive = value;
      RestorePuzzleState();
      Container.SetActive(value);
      FlipMatrix.SetActive(value);
    }
  }

  private void Awake()
  {
    _gameState = FindObjectOfType<GameState>();
    _flipMatrixText = FlipMatrix.GetComponent<TextMeshProUGUI>();
  }

  void Start()
  {
    Puzzle.OnSolved += OnPuzzleSolved;
    Puzzle.OnFlip += OnPuzzleFlip;
    DisplayActive = false;
  }

  private void OnPuzzleSolved()
  {
    FlipControl.SetActive(false);
    Puzzle.InputActive = false;
    var selectedModule = _gameState.GetSelectedModule();
    selectedModule.SetOverrideState(SpaceStationModuleOverrideState.Active);
  }

  private void OnPuzzleFlip(int flipCount)
  {
    _gameState.GetSelectedModule().SetPuzzleState(Puzzle.GetState());
    _gameState.GetStationStat(SpaceStationStat.Batteries).Percentage -= 0.9f;
  }

  private void RestorePuzzleState()
  {
    var selectedModule = _gameState.GetSelectedModule();
    if (DisplayActive && selectedModule.OverrideState != SpaceStationModuleOverrideState.Unsupported && selectedModule.OverrideState != SpaceStationModuleOverrideState.Active)
    {
      selectedModule.RestorePuzzle(Puzzle);
      selectedModule.RestorePuzzleFlipMatrix(_flipMatrixText);
      FlipControl.SetActive(true);
    }
    else
    {
      Puzzle.InputActive = false;
      FlipControl.SetActive(false);
    }
  }

  // Update is called once per frame
  void Update()
  {

  }
}
