using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum SpaceStationModuleState
{
  Off,
  EmergencOff,
  On,
  EmergencyOn,
}

public enum SpaceStationModuleOverrideState
{
  Unsupported,
  Inactive,
  InProgress,
  Active
}

public enum SpaceStationModule
{
  Battery,
  SolarPanels,
  Latches,
  Sensors,
  Atmosphere,
  AI,
  Count
}

public enum SpaceStationStat
{
  AtmosphericPressure,
  AtmosphericQuality,
  CrewVitals,
  Latches,
  Sensors,
  PowerGeneration,
  Batteries,
  AISystems,
}

public enum SpaceStationStatMode
{
  Ok,
  Off,
  Percentage
}

public interface SpaceStationModuleListener
{
  void OnStateChanged(SpaceStationModule module, SpaceStationModuleData moduleData, SpaceStationModuleState newState);
  void OnOverrideChanged(SpaceStationModule module, SpaceStationModuleData moduleData, SpaceStationModuleOverrideState newState);
  void OnModuleSelected(SpaceStationModule module, SpaceStationModuleData moduleData);
}

public interface SpaceStationStatListener
{
  void OnStatChanged(SpaceStationStat stat, SpaceStationStatData statData);
}

public class SpaceStationModuleData
{
  public string Name { get; set; }

  SpaceStationModuleState _state;
  public SpaceStationModuleState State
  {
    get
    {
      return _state;
    }
    set
    {
      _state = value;
      _gameState.TriggerOnStateChanged(_module, this, value);

      if (_module == SpaceStationModule.Sensors)
      {
        if (State == SpaceStationModuleState.On)
        {
          _gameState.GetStationStat(SpaceStationStat.Sensors).Mode = SpaceStationStatMode.Ok;
        }
        else if (State == SpaceStationModuleState.Off || State == SpaceStationModuleState.EmergencOff)
        {
          _gameState.GetStationStat(SpaceStationStat.Sensors).Mode = SpaceStationStatMode.Off;
        }

        for (int i = 0; i <= (int)SpaceStationStat.AISystems; i++)
        {
          var stat = (SpaceStationStat)i;
          if (stat != SpaceStationStat.Sensors)
          {
            _gameState.GetStationStat(stat).SetUnknown(State == SpaceStationModuleState.Off || State == SpaceStationModuleState.EmergencOff);
          }
        }
      }

      if (_module == SpaceStationModule.AI)
      {
        _gameState.GetStationStat(SpaceStationStat.AISystems).Mode = State == SpaceStationModuleState.On ? SpaceStationStatMode.Ok : SpaceStationStatMode.Off;
      }

    }
  }

  internal void RestorePuzzleFlipMatrix(TextMeshProUGUI flipMatrixText)
  {
    string text = "";
    for (int r = 0; r < _overridePuzzleFlipMatrix.GetLength(0); r++)
    {
      for (int c = 0; c < _overridePuzzleFlipMatrix.GetLength(1); c++)
      {
        text += _overridePuzzleFlipMatrix[r, c].ToString();
        if (c < _overridePuzzleFlipMatrix.GetLength(1) - 1)
        {
          text += " ";
        }
      }

      if (r < _overridePuzzleFlipMatrix.GetLength(0) - 1)
      {
        text += "<br>";
      }
    }
    flipMatrixText.text = text;
  }

  SpaceStationModuleOverrideState _overrideState;
  public SpaceStationModuleOverrideState OverrideState
  {
    get
    {
      return _overrideState;
    }
    set
    {
      _overrideState = value;
      _gameState.TriggerOnOverrideChanged(_module, this, _overrideState);
      if (_overrideState == SpaceStationModuleOverrideState.Active)
      {
        if (_module == SpaceStationModule.AI)
        {
          State = SpaceStationModuleState.EmergencyOn;
        }
        else if (_module == SpaceStationModule.Latches)
        {
          State = SpaceStationModuleState.EmergencOff;
          _gameState.GetStationStat(SpaceStationStat.Latches).Mode = State == SpaceStationModuleState.On ? SpaceStationStatMode.Ok : SpaceStationStatMode.Off;
        }
        else if (_module == SpaceStationModule.Atmosphere)
        {
          State = SpaceStationModuleState.EmergencyOn;
        }
        else if (_module == SpaceStationModule.Sensors)
        {
          State = SpaceStationModuleState.EmergencyOn;
        }
      }
    }
  }

  private GameState _gameState;
  private SpaceStationModule _module;

  private int[] _overridePuzzleState;
  private int[] _overridePuzzleSolution;
  private int[,] _overridePuzzleFlipMatrix;

  public SpaceStationModuleData(SpaceStationModule module, string name, SpaceStationModuleState state, GameState gameState)
  {
    _gameState = gameState;
    _module = module;

    Name = name;
    _overrideState = SpaceStationModuleOverrideState.Unsupported;
    _state = state;
  }

  public string FormatState()
  {
    switch (State)
    {
      case SpaceStationModuleState.Off:
        return string.Format("Status: {0}", "Off");
      case SpaceStationModuleState.On:
        return string.Format("Status: {0}", "On");
      case SpaceStationModuleState.EmergencyOn:
        return string.Format("Status: {0}", "On - <color=#C39F62>Locked</color>");
      default:
        return string.Format("Status: {0}", "Off - <color=#C39F62>Locked</color>");
    }
  }

  public string FormatOverride()
  {
    switch (OverrideState)
    {
      case SpaceStationModuleOverrideState.Inactive:
        return string.Format("Override: {0}", "Inactive");
      case SpaceStationModuleOverrideState.InProgress:
        return string.Format("Override: {0}", "In progress");
      case SpaceStationModuleOverrideState.Unsupported:
        return string.Format("Override: {0}", "<color=#C39F62>Not supported</color>");
      default:
        return string.Format("Override: {0}", "<color=#C39F62>Active</color>");
    }
  }

  public void ToggleState()
  {
    if (State == SpaceStationModuleState.On)
    {
      State = SpaceStationModuleState.Off;
    }
    else if (State == SpaceStationModuleState.Off)
    {
      State = SpaceStationModuleState.On;
    }
    else
    {
      return;
    }

    //if (_module == SpaceStationModule.AI)
    //{
    //  _gameState.GetStationStat(SpaceStationStat.AISystems).Mode = State == SpaceStationModuleState.On ? SpaceStationStatMode.Ok : SpaceStationStatMode.Off;
    //}

    //if (_module == SpaceStationModule.Sensors)
    //{
    //  if (State == SpaceStationModuleState.On)
    //  {
    //    _gameState.GetStationStat(SpaceStationStat.Sensors).Mode = SpaceStationStatMode.Ok;
    //  }
    //  else if (State == SpaceStationModuleState.Off)
    //  {
    //    _gameState.GetStationStat(SpaceStationStat.Sensors).Mode = SpaceStationStatMode.Off;
    //  }

    //  for (int i = 0; i <= (int)SpaceStationStat.AISystems; i++)
    //  {
    //    var stat = (SpaceStationStat)i;
    //    if (stat != SpaceStationStat.Sensors)
    //    {
    //      _gameState.GetStationStat(stat).SetUnknown(State == SpaceStationModuleState.On || State == SpaceStationModuleState.EmergencyOn);
    //    }
    //  }
    //}

    //if (_module == SpaceStationModule.Latches)
    //{
    //  _gameState.GetStationStat(SpaceStationStat.Latches).Mode = State == SpaceStationModuleState.On ? SpaceStationStatMode.Ok : SpaceStationStatMode.Off;
    //}
  }

  public void SetOverrideState(SpaceStationModuleOverrideState state)
  {
    OverrideState = state;
  }

  public void InitializePuzzle(int[] initialState, int[,] flipMatrix, int[] solution)
  {
    _overridePuzzleState = (int[])initialState.Clone();
    _overridePuzzleFlipMatrix = (int[,])flipMatrix.Clone();
    _overridePuzzleSolution = (int[])solution.Clone();
    OverrideState = SpaceStationModuleOverrideState.Inactive;
  }

  public void SetPuzzleState(int[] state)
  {
    _overridePuzzleState = (int[])state.Clone();
  }

  public void RestorePuzzle(FlippingPuzzle puzzle)
  {
    puzzle.Initialize(_overridePuzzleState, _overridePuzzleFlipMatrix, _overridePuzzleSolution);
  }
}

public class SpaceStationStatData
{
  private SpaceStationStat _stat;
  private string _name;
  private bool _unknown;

  private SpaceStationStatMode _mode;
  public SpaceStationStatMode Mode
  {
    get { return _mode; }
    set
    {
      _mode = value;
      _gameState.TriggerOnStatChanged(_stat, this);
    }
  }

  private float _percentage;
  public float Percentage
  {
    get { return _percentage; }
    set
    {
      _percentage = value;
      _gameState.TriggerOnStatChanged(_stat, this);
    }
  }

  private GameState _gameState;

  public SpaceStationStatData(SpaceStationStat stat, string shortName, SpaceStationStatMode mode, float percentage, GameState gameState)
  {
    _stat = stat;
    _name = shortName;
    _mode = mode;
    _percentage = percentage;
    _gameState = gameState;
    _unknown = false;
  }

  public string Format(bool leftAlign)
  {
    string format = "";
    string text = "";
    string color = "";

    if (leftAlign)
    {
      format = "{0}{1}<color={2}>{3}</color>";
    }
    else
    {
      format = "<color={2}>{3}</color>{1}{0}";
    }

    switch (_mode)
    {
      case SpaceStationStatMode.Ok:
        {
          text = "OK";
          color = "#56B453";
        }
        break;
      case SpaceStationStatMode.Off:
        {
          text = "OFF";
          color = "#B45553";
        }
        break;
      case SpaceStationStatMode.Percentage:
      default:
        {
          text = string.Format("{0:0.##}%", Math.Floor(_percentage));
          if (_percentage < 20.0f)
          {
            color = "#B45553";
          }
          else
          {
            color = "#C39F62";
          }

        }
        break;
    }

    if (_unknown)
    {
      text = "N/A";
      color = "#A4A4A4";
    }

    return string.Format(format, _name, new string(' ', 14 - _name.Length - text.Length), color, text);
  }

  public void SetUnknown(bool unknown)
  {
    _unknown = unknown;
    _gameState.TriggerOnStatChanged(_stat, this);
  }
}

public class GameState : MonoBehaviour
{
  private Dictionary<SpaceStationModule, SpaceStationModuleData> _modules;
  private Dictionary<SpaceStationStat, SpaceStationStatData> _stats;
  private Dictionary<SpaceStationModule, ModuleSelector> _selectors;
  private List<SpaceStationModuleListener> _listenersModules;
  private List<SpaceStationStatListener> _listenersStats;

  private const float kAtmQualityIncreaseSpeed = 0.1f;
  private const float kAtmQualityDecreaseSpeed = 1.0f;

  private const float kVitalsIncreaseSpeed = 0.01f;
  private const float kVitalsDecreaseSpeed = 4.0f;


  private SpaceStationModule _selectedModule;
  public SpaceStationModule SelectedModule
  {
    get { return _selectedModule; }
    private set
    {
      _selectedModule = value;

      UpdateSelectors();
      TriggerOnModuleSelected(SelectedModule, _modules[SelectedModule]);
    }
  }

  private void UpdateSelectors()
  {
    foreach (var selector in _selectors.Keys)
    {
      _selectors[selector].SetEnabled(ShowSelectors && selector == SelectedModule);
    }
  }

  private bool _showSelectors;
  public bool ShowSelectors
  {
    get { return _showSelectors; }
    set
    {
      _showSelectors = value;
      UpdateSelectors();
    }
  }

  void Awake()
  {
    _listenersModules = new List<SpaceStationModuleListener>();
    _listenersStats = new List<SpaceStationStatListener>();

    _modules = new Dictionary<SpaceStationModule, SpaceStationModuleData>()
    {
      {SpaceStationModule.Battery, new SpaceStationModuleData(SpaceStationModule.Battery, "Battery module", SpaceStationModuleState.EmergencyOn, this)},
      {SpaceStationModule.SolarPanels, new SpaceStationModuleData(SpaceStationModule.AI, "Solar panels", SpaceStationModuleState.EmergencOff, this)},
      {SpaceStationModule.Latches, new SpaceStationModuleData(SpaceStationModule.Latches, "Latches module", SpaceStationModuleState.On, this)},
      {SpaceStationModule.Sensors, new SpaceStationModuleData(SpaceStationModule.Sensors,"Sensors module", SpaceStationModuleState.On, this)},
      {SpaceStationModule.Atmosphere, new SpaceStationModuleData(SpaceStationModule.Atmosphere,"Atmosphere module", SpaceStationModuleState.On, this)},
      {SpaceStationModule.AI, new SpaceStationModuleData(SpaceStationModule.AI, "AI module", SpaceStationModuleState.Off, this)},
    };

    _stats = new Dictionary<SpaceStationStat, SpaceStationStatData>()
    {
      {SpaceStationStat.AtmosphericPressure, new SpaceStationStatData(SpaceStationStat.AtmosphericPressure, "ATM PR", SpaceStationStatMode.Ok, 100.0f, this) },
      {SpaceStationStat.AtmosphericQuality, new SpaceStationStatData(SpaceStationStat.AtmosphericQuality, "ATM Q", SpaceStationStatMode.Ok, 100.0f, this) },
      {SpaceStationStat.CrewVitals, new SpaceStationStatData(SpaceStationStat.CrewVitals, "CRW VIT", SpaceStationStatMode.Ok, 100.0f, this) },
      {SpaceStationStat.Latches, new SpaceStationStatData(SpaceStationStat.Latches, "LTCHS", SpaceStationStatMode.Ok, 100.0f, this) },
      {SpaceStationStat.Sensors, new SpaceStationStatData(SpaceStationStat.Sensors, "SENSRS", SpaceStationStatMode.Ok, 100.0f, this) },
      {SpaceStationStat.PowerGeneration, new SpaceStationStatData(SpaceStationStat.PowerGeneration, "PWR GEN", SpaceStationStatMode.Percentage, 10.0f, this) },
      {SpaceStationStat.Batteries, new SpaceStationStatData(SpaceStationStat.Batteries, "BATTRS", SpaceStationStatMode.Percentage, 99.0f, this) },
      {SpaceStationStat.AISystems, new SpaceStationStatData(SpaceStationStat.AISystems, "AI SYS", SpaceStationStatMode.Off, 0.0f, this) },
    };

    _modules[SpaceStationModule.AI].InitializePuzzle(
      new int[4] { 1, 0, 0, 1 },
      new int[4, 4] { { 1, 1, 0, 0 }, { 0, 1, 1, 0 }, { 0, 0, 1, 0 }, { 0, 1, 1, 1 } },
      new int[4] { 0, 0, 0, 0 }
    );

    _modules[SpaceStationModule.Latches].InitializePuzzle(
      new int[5] { 1, 0, 1, 1, 1 },
      new int[5, 5] { { 1, 0, 0, 0, 1 }, { 0, 1, 0, 1, 0 }, { 1, 1, 1, 0, 0 }, { 0, 0, 0, 1, 1 }, { 0, 0, 0, 0, 1 } },
      new int[5] { 0, 0, 0, 0, 0 }
    );

    _modules[SpaceStationModule.Atmosphere].InitializePuzzle(
      new int[6] { 1, 1, 1, 1, 1, 0 },
      new int[6, 6] { { 1, 0, 1, 0, 1, 0 }, { 0, 1, 0, 0, 1, 0 }, { 0, 0, 1, 1, 1, 0 }, { 0, 1, 0, 1, 0, 0 }, { 0, 0, 0, 0, 1, 0 }, { 1, 0, 0, 0, 0, 1 }, },
      new int[6] { 0, 0, 0, 0, 0, 0 }
    );

    _modules[SpaceStationModule.Sensors].InitializePuzzle(
      new int[7] { 1, 1, 0, 1, 1, 0, 1 },
      new int[7, 7] { { 1, 0, 1, 0, 1, 0, 0 }, { 0, 0, 0, 1, 1, 0, 0 }, { 1, 0, 1, 1, 0, 0, 1 }, { 0, 0, 0, 1, 0, 1, 0 }, { 0, 0, 0, 0, 1, 0, 0 }, { 1, 1, 0, 0, 0, 1, 0 }, { 1, 0, 0, 0, 0, 1, 1 }},
      new int[7] { 0, 0, 0, 0, 0, 0, 0 }
    );

    _selectors = new Dictionary<SpaceStationModule, ModuleSelector>();

    SelectedModule = SpaceStationModule.Sensors;

    ShowSelectors = false;
  }

  void Start()
  {
    var selectors = FindObjectsOfType<ModuleSelector>();
    foreach (var selector in selectors)
    {
      _selectors[selector.Module] = selector;
    }

    UpdateSelectors();
  }

  public SpaceStationModuleData GetSelectedModule()
  {
    return _modules[SelectedModule];
  }

  public SpaceStationStatData GetStationStat(SpaceStationStat stat)
  {
    return _stats[stat];
  }


  public SpaceStationModuleData GetStationModule(SpaceStationModule module)
  {
    return _modules[module];
  }

  public void RegisterStationModuleListener(SpaceStationModuleListener listener)
  {
    if (!_listenersModules.Contains(listener))
    {
      _listenersModules.Add(listener);

      foreach (var modulePair in _modules)
      {
        listener.OnStateChanged(modulePair.Key, modulePair.Value, modulePair.Value.State);
        listener.OnOverrideChanged(modulePair.Key, modulePair.Value, modulePair.Value.OverrideState);
      }

      listener.OnModuleSelected(SelectedModule, GetSelectedModule());
    }
  }

  public void RegisterSpaceStationStatListener(SpaceStationStatListener listener)
  {
    if (!_listenersStats.Contains(listener))
    {
      _listenersStats.Add(listener);

      foreach (var statPair in _stats)
      {
        listener.OnStatChanged(statPair.Key, statPair.Value);
      }
    }
  }

  public void TriggerOnStateChanged(SpaceStationModule module, SpaceStationModuleData moduleData, SpaceStationModuleState newState)
  {
    foreach (var listener in _listenersModules)
    {
      listener.OnStateChanged(module, moduleData, newState);
    }
  }

  public void TriggerOnOverrideChanged(SpaceStationModule module, SpaceStationModuleData moduleData, SpaceStationModuleOverrideState newState)
  {
    foreach (var listener in _listenersModules)
    {
      listener.OnOverrideChanged(module, moduleData, newState);
    }
  }

  public void TriggerOnModuleSelected(SpaceStationModule module, SpaceStationModuleData moduleData)
  {
    foreach (var listener in _listenersModules)
    {
      listener.OnModuleSelected(module, moduleData);
    }
  }

  public void TriggerOnStatChanged(SpaceStationStat stat, SpaceStationStatData statData)
  {
    foreach (var listener in _listenersStats)
    {
      listener.OnStatChanged(stat, statData);
    }
  }

  public void SelectNextModule()
  {
    int selectedModule = (int)SelectedModule;
    selectedModule += 1;
    if (selectedModule == (int)SpaceStationModule.Count)
    {
      selectedModule = 0;
    }
    SelectedModule = (SpaceStationModule)selectedModule;

  }

  public void SelectPrevModule()
  {
    int selectedModule = (int)SelectedModule;
    selectedModule -= 1;
    if (selectedModule < 0)
    {
      selectedModule = (int)SpaceStationModule.Count - 1;
    }
    SelectedModule = (SpaceStationModule)selectedModule;

  }

  // Update is called once per frame
  void Update()
  {
    if (_modules[SpaceStationModule.Atmosphere].State == SpaceStationModuleState.Off || _modules[SpaceStationModule.Atmosphere].State == SpaceStationModuleState.EmergencOff)
    {
      _stats[SpaceStationStat.AtmosphericQuality].Percentage = Math.Max(_stats[SpaceStationStat.AtmosphericQuality].Percentage - Time.deltaTime * kAtmQualityDecreaseSpeed, 0.0f);
      if (_stats[SpaceStationStat.AtmosphericQuality].Percentage < 100.0f && _stats[SpaceStationStat.AtmosphericQuality].Mode != SpaceStationStatMode.Percentage)
      {
        _stats[SpaceStationStat.AtmosphericQuality].Mode = SpaceStationStatMode.Percentage;
      }

      if (_stats[SpaceStationStat.AtmosphericQuality].Percentage < 50.0f)
      {
        _stats[SpaceStationStat.CrewVitals].Percentage = Math.Max(_stats[SpaceStationStat.CrewVitals].Percentage - Time.deltaTime * kVitalsDecreaseSpeed, 0.0f);
        if (_stats[SpaceStationStat.CrewVitals].Percentage < 100.0f && _stats[SpaceStationStat.CrewVitals].Mode != SpaceStationStatMode.Percentage)
        {
          _stats[SpaceStationStat.CrewVitals].Mode = SpaceStationStatMode.Percentage;
        }
      }
    }
    else if (_modules[SpaceStationModule.Atmosphere].State == SpaceStationModuleState.On || _modules[SpaceStationModule.Atmosphere].State == SpaceStationModuleState.EmergencyOn)
    {
      _stats[SpaceStationStat.AtmosphericQuality].Percentage = Math.Min(_stats[SpaceStationStat.AtmosphericQuality].Percentage + Time.deltaTime * kAtmQualityIncreaseSpeed, 100.0f);
      if (_stats[SpaceStationStat.AtmosphericQuality].Percentage >= 100.0f && _stats[SpaceStationStat.AtmosphericQuality].Mode != SpaceStationStatMode.Ok)
      {
        _stats[SpaceStationStat.AtmosphericQuality].Mode = SpaceStationStatMode.Ok;
      }
    }
  }
}
