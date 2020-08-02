using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusDisplays : MonoBehaviour, SpaceStationModuleListener
{
  public GameObject AIDisabledMessage;
  public GameObject NewLogMessage;

  private GameState _gameState;

  public void OnModuleSelected(SpaceStationModule module, SpaceStationModuleData moduleData)
  {

  }

  public void OnOverrideChanged(SpaceStationModule module, SpaceStationModuleData moduleData, SpaceStationModuleOverrideState newState)
  {

  }

  public void OnStateChanged(SpaceStationModule module, SpaceStationModuleData moduleData, SpaceStationModuleState newState)
  {
    if (module == SpaceStationModule.AI)
    {
      AIDisabledMessage.SetActive(newState == SpaceStationModuleState.Off || newState == SpaceStationModuleState.EmergencOff);
    }
  }


  public void AddNewLogMessage()
  {
    NewLogMessage.SetActive(true);
  }

  public void ResetNewLogMessage()
  {
    NewLogMessage.SetActive(false);
  }

  void Awake()
  {
    _gameState = FindObjectOfType<GameState>();
  }

  // Start is called before the first frame update
  void Start()
  {
    _gameState.RegisterStationModuleListener(this);
  }

  // Update is called once per frame
  void Update()
  {

  }
}
