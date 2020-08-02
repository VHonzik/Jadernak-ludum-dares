using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModuleSprite : MonoBehaviour, SpaceStationModuleListener
{
  public SpaceStationModule Module;

  private Image _image;
  private GameState _gameState;
  private Dictionary<SpaceStationModuleState, Color> _colors;

  public void OnModuleSelected(SpaceStationModule module, SpaceStationModuleData moduleData)
  {
  }

  public void OnOverrideChanged(SpaceStationModule module, SpaceStationModuleData moduleData, SpaceStationModuleOverrideState newState)
  {

  }

  public void OnStateChanged(SpaceStationModule module, SpaceStationModuleData moduleData, SpaceStationModuleState newState)
  {
    if (module == Module)
    {
      _image.color = _colors[newState];
    }
  }

  void Awake()
  {
    _gameState = FindObjectOfType<GameState>();
    _image = GetComponent<Image>();
    _colors = new Dictionary<SpaceStationModuleState, Color>()
    {
        { SpaceStationModuleState.Off, Color.gray },
        { SpaceStationModuleState.EmergencOff, Color.red },
        { SpaceStationModuleState.On, Color.white },
        { SpaceStationModuleState.EmergencyOn, new Color32(0xB7, 0xFF, 0xAD, 0xFF) },
    };
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
