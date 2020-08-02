using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MenuState
{
  MainMenu,
  StationNavigation,
  Overriding,
  Log,
  Off
}

public class MainMenu : MonoBehaviour
{
  public GameObject MainMenuControls;
  public GameObject StationNavigationControls;
  public GameObject OverridingControls;
  public GameObject LogScene;
  public GameObject StatusScene;
  public StationNavigation StationNavigationScript;
  public ModuleDataDisplay ModuleDisplayScript;
  public OverridingDisplay GameFlipPuzzle;
  public LogNavigation LogNavigationScript;
  public StatusDisplays StatusDisplaysScript;

  private GameState _gameState;

  private bool _stateUpdating;

  private MenuState _state;
  public MenuState State
  {
    get { return _state; }
    set
    {
      _state = value;
      UpdateMenus();
    }
  }

  // Start is called before the first frame update
  void Awake()
  {
    _stateUpdating = false;
    _state = MenuState.MainMenu;
    _gameState = GetComponent<GameState>();
  }

  private IEnumerator UpdateMenuCoor()
  {
    if (_state == MenuState.Off)
    {
      GameFlipPuzzle.DisplayActive = false;
    }

    MainMenuControls.SetActive(false);
    StationNavigationScript.NavigationActive = false;
    StationNavigationControls.SetActive(false);
    LogNavigationScript.NavigationActive = false;
    OverridingControls.SetActive(false);
    _gameState.ShowSelectors = false;
    ModuleDisplayScript.DisplayActive = false;
    LogScene.SetActive(false);
    StatusScene.SetActive(false);

    yield return new WaitForSeconds(0.5f);

    MainMenuControls.SetActive(_state == MenuState.MainMenu);
    StationNavigationScript.NavigationActive = _state == MenuState.StationNavigation;
    LogNavigationScript.NavigationActive = _state == MenuState.Log;
    StationNavigationControls.SetActive(_state == MenuState.StationNavigation);
    OverridingControls.SetActive(_state == MenuState.Overriding);
    _gameState.ShowSelectors = _state == MenuState.Overriding || _state == MenuState.StationNavigation;
    ModuleDisplayScript.DisplayActive = _state == MenuState.Overriding || _state == MenuState.StationNavigation;
    LogScene.SetActive(_state == MenuState.Log);
    StatusScene.SetActive(_state != MenuState.Log);



    _stateUpdating = false;
  }

  private void UpdateMenus()
  {
    _stateUpdating = true;
    StartCoroutine(UpdateMenuCoor());
  }

  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {
    if (!_stateUpdating && State == MenuState.MainMenu && Input.GetKeyDown(KeyCode.M))
    {
      State = MenuState.StationNavigation;
    }

    if (!_stateUpdating && State == MenuState.StationNavigation && Input.GetKeyDown(KeyCode.B))
    {
      State = MenuState.MainMenu;
    }

    if (!_stateUpdating && State == MenuState.StationNavigation && Input.GetKeyDown(KeyCode.O) && StationNavigationScript.OverridePressed())
    {
      State = MenuState.Overriding;
      GameFlipPuzzle.DisplayActive = true;
    }

    if (!_stateUpdating && State == MenuState.Overriding && Input.GetKeyDown(KeyCode.B))
    {
      State = MenuState.StationNavigation;
      GameFlipPuzzle.DisplayActive = false;
    }

    if (!_stateUpdating && State == MenuState.MainMenu && Input.GetKeyDown(KeyCode.L))
    {
      State = MenuState.Log;
      StatusDisplaysScript.ResetNewLogMessage();
    }

    if (!_stateUpdating && State == MenuState.Log && Input.GetKeyDown(KeyCode.B))
    {
      State = MenuState.MainMenu;
    }
  }
}
