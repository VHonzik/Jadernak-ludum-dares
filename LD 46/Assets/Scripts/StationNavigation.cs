using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StationNavigation : MonoBehaviour, SpaceStationModuleListener
{
  private int _selected;
  private GameState _gameState;
  private SpaceStationModuleData _activeModule;
  public GameObject OverridingControl;
  public GameObject ToggleControl;

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
  void Awake()
  {
    _navigationActive = false;
    _gameState = FindObjectOfType<GameState>();
  }

  void Start()
  {
    _gameState.RegisterStationModuleListener(this);
  }

  // Update is called once per frame
  void Update()
  {
    if (NavigationActive)
    {
      if (Input.GetKeyDown(KeyCode.LeftArrow))
      {
        _gameState.SelectPrevModule();
      }
      else if (Input.GetKeyDown(KeyCode.RightArrow))
      {
        _gameState.SelectNextModule();
      }

      if (Input.GetKeyDown(KeyCode.T))
      {
        _activeModule.ToggleState();
      }
    }
  }

  public bool OverridePressed()
  {
    if (_activeModule.OverrideState == SpaceStationModuleOverrideState.Inactive || _activeModule.OverrideState == SpaceStationModuleOverrideState.InProgress)
    {
      _activeModule.SetOverrideState(SpaceStationModuleOverrideState.InProgress);
      return true;
    }

    return false;
  }

  public void OnStateChanged(SpaceStationModule module, SpaceStationModuleData moduleData, SpaceStationModuleState newState)
  {
    if (moduleData == _activeModule)
    {
      ToggleControl.SetActive(_activeModule.State == SpaceStationModuleState.On || _activeModule.State == SpaceStationModuleState.Off);
    }
  }

  public void OnModuleSelected(SpaceStationModule module, SpaceStationModuleData moduleData)
  {
    _activeModule = moduleData;
    OverridingControl.SetActive(_activeModule.OverrideState == SpaceStationModuleOverrideState.Inactive || _activeModule.OverrideState == SpaceStationModuleOverrideState.InProgress);
    ToggleControl.SetActive(_activeModule.State == SpaceStationModuleState.On || _activeModule.State == SpaceStationModuleState.Off);
  }

  public void OnOverrideChanged(SpaceStationModule module, SpaceStationModuleData moduleData, SpaceStationModuleOverrideState newState)
  {
    if (moduleData == _activeModule)
    {
      OverridingControl.SetActive(_activeModule.OverrideState == SpaceStationModuleOverrideState.Inactive || _activeModule.OverrideState == SpaceStationModuleOverrideState.InProgress);
    }
  }
}
