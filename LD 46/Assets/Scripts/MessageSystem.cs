using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MessageSystem : MonoBehaviour, SpaceStationModuleListener, SpaceStationStatListener
{
  public GameObject AIEnabledFirstTime;
  public GameObject AIOverridenMessage;
  public GameObject LatchesOverridenMessage;
  public GameObject AtmosphereOverridenMessage;
  public GameObject AllOverridenMessage;
  public GameObject LowBatteriesMessage;
  public StatusDisplays StatusDisplays;
  public MainMenu MainMenu;
  public Image BlackoutImage;
  public GameObject BlackoutThanksText;
  public GameObject BlackoutYouLostText;
  public AudioClip FirstMessage;
  public AudioClip AIEnabledMessage;
  public AudioClip AIOverridenMessageClip;
  public AudioClip LatchesOverridenMessageClip;
  public AudioClip AtmoOverridenMessageClip;
  public AudioClip FinalMessageClip;
  public AudioClip EmergencyOverrideClip;
  public AudioClip LowBatteriesMessageClip;

  private Queue<AudioClip> _clips;

  private bool _lowBatteriesPlayed;
  private bool _aiEnabledFirstTime;
  private GameState _gameState;
  private AudioSource _source;

  private IEnumerator FinalSequence()
  {
    AllOverridenMessage.SetActive(true);
    MainMenu.State = MenuState.Off;

    while (_source.isPlaying)
    {
      yield return null;
    }

    _clips.Enqueue(FinalMessageClip);

    yield return new WaitForSeconds(10.0f);

    _gameState.GetStationModule(SpaceStationModule.Atmosphere).State = SpaceStationModuleState.EmergencOff;

    yield return new WaitForSeconds(7.0f);

    _gameState.GetStationModule(SpaceStationModule.Sensors).State = SpaceStationModuleState.EmergencOff;

    yield return new WaitForSeconds(20.0f);

    while (BlackoutImage.color.a < 0.99f)
    {
      BlackoutImage.color = new Color(BlackoutImage.color.r, BlackoutImage.color.g, BlackoutImage.color.b, BlackoutImage.color.a + 0.5f * Time.deltaTime);
      yield return null;
    }
    BlackoutImage.color = new Color(BlackoutImage.color.r, BlackoutImage.color.g, BlackoutImage.color.b, 1.0f);
    BlackoutThanksText.SetActive(true);
  }

  private IEnumerator YouLostSequence()
  {
    MainMenu.State = MenuState.Off;

    while (_source.isPlaying)
    {
      yield return null;
    }

    while (BlackoutImage.color.a < 0.99f)
    {
      BlackoutImage.color = new Color(BlackoutImage.color.r, BlackoutImage.color.g, BlackoutImage.color.b, BlackoutImage.color.a + 0.5f * Time.deltaTime);
      yield return null;
    }
    BlackoutImage.color = new Color(BlackoutImage.color.r, BlackoutImage.color.g, BlackoutImage.color.b, 1.0f);
    BlackoutYouLostText.SetActive(true);
  }

  public void OnModuleSelected(SpaceStationModule module, SpaceStationModuleData moduleData)
  {

  }

  public void IntroFinished()
  {
    _clips.Enqueue(FirstMessage);
  }

  public void OnOverrideChanged(SpaceStationModule module, SpaceStationModuleData moduleData, SpaceStationModuleOverrideState newState)
  {
    if (module == SpaceStationModule.AI && newState == SpaceStationModuleOverrideState.Active)
    {
      _clips.Enqueue(AIOverridenMessageClip);
      AIOverridenMessage.SetActive(true);
    }
    else if (module == SpaceStationModule.Latches && newState == SpaceStationModuleOverrideState.Active)
    {
      _clips.Enqueue(LatchesOverridenMessageClip);
      LatchesOverridenMessage.SetActive(true);
    }
    else if (module == SpaceStationModule.Atmosphere && newState == SpaceStationModuleOverrideState.Active)
    {
      _clips.Enqueue(AtmoOverridenMessageClip);
      AtmosphereOverridenMessage.SetActive(true);
    }

    if (_gameState.GetStationModule(SpaceStationModule.AI).OverrideState == SpaceStationModuleOverrideState.Active &&
      _gameState.GetStationModule(SpaceStationModule.Latches).OverrideState == SpaceStationModuleOverrideState.Active &&
      _gameState.GetStationModule(SpaceStationModule.Atmosphere).OverrideState == SpaceStationModuleOverrideState.Active &&
      _gameState.GetStationModule(SpaceStationModule.Sensors).OverrideState == SpaceStationModuleOverrideState.Active)
    {
      StartCoroutine(FinalSequence());
    }
  }

  public void QueueClip(AudioClip clip)
  {
    _clips.Enqueue(clip);
  }

  public void OnStateChanged(SpaceStationModule module, SpaceStationModuleData moduleData, SpaceStationModuleState newState)
  {
    if (module == SpaceStationModule.AI && !_aiEnabledFirstTime && newState == SpaceStationModuleState.On)
    {
      _aiEnabledFirstTime = true;
      AIEnabledFirstTime.SetActive(true);
      _clips.Enqueue(AIEnabledMessage);
      StatusDisplays.AddNewLogMessage();
    }
  }

  // Start is called before the first frame update
  void Awake()
  {
    _aiEnabledFirstTime = false;
    _gameState = FindObjectOfType<GameState>();
    _source = GetComponent<AudioSource>();
    _clips = new Queue<AudioClip>();
    _lowBatteriesPlayed = false;
  }

  void Start()
  {
    _gameState.RegisterStationModuleListener(this);
    _gameState.RegisterSpaceStationStatListener(this);
  }

  // Update is called once per frame
  void Update()
  {
    if (!_source.isPlaying && _clips.Count > 0)
    {
      _source.PlayOneShot(_clips.Dequeue());
    }
  }

  public void OnStatChanged(SpaceStationStat stat, SpaceStationStatData statData)
  {
    if (!_lowBatteriesPlayed && stat == SpaceStationStat.Batteries && statData.Percentage < 20.0f)
    {
      _lowBatteriesPlayed = true;
      LowBatteriesMessage.SetActive(true);
      _clips.Enqueue(LowBatteriesMessageClip);
    }

    if ((stat == SpaceStationStat.Batteries || stat == SpaceStationStat.CrewVitals) && statData.Percentage < 0.5f)
    {
      StartCoroutine(YouLostSequence());
    }
  }
}
