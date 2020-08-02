using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ModuleDataDisplay : MonoBehaviour, SpaceStationModuleListener
{
  public GameObject ModuleData;
  public TextMeshProUGUI ModuleName;
  public TextMeshProUGUI ModuleStatus;
  public TextMeshProUGUI ModuleOverrideStatus;

  private SpaceStationModuleData _activeModule;
  private GameState _gameState;

  private bool _displayActive;
  public bool DisplayActive
  {
    get { return _displayActive; }
    set
    {
      _displayActive = value;
      ModuleData.SetActive(value);
    }
  }

  private void UpdateModuleData(SpaceStationModuleData moduleData)
  {
    ModuleName.text = moduleData.Name;
    ModuleStatus.text = moduleData.FormatState();
    ModuleOverrideStatus.text = moduleData.FormatOverride();
  }

  public void OnModuleSelected(SpaceStationModule module, SpaceStationModuleData moduleData)
  {
    _activeModule = moduleData;
    UpdateModuleData(moduleData);
  }

  public void OnOverrideChanged(SpaceStationModule module, SpaceStationModuleData moduleData, SpaceStationModuleOverrideState newState)
  {
    if (_activeModule == moduleData)
    {
      UpdateModuleData(moduleData);
    }
  }

  public void OnStateChanged(SpaceStationModule module, SpaceStationModuleData moduleData, SpaceStationModuleState newState)
  {
    if (_activeModule == moduleData)
    {
      UpdateModuleData(moduleData);
    }
  }

  void Awake()
  {
    _gameState = FindObjectOfType<GameState>();
  }

  void Start()
  {
    _gameState.RegisterStationModuleListener(this);
  }

  // Update is called once per frame
  void Update()
  {

  }
}
