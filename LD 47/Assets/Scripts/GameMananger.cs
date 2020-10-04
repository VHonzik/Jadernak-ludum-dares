using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum AudioState
{
  NoClip,
  Playing,
  Paused,
  FadingOut
}

public class GameMananger : MonoBehaviour
{
  public GameObject IntroCard;
  public GameObject[] MainStoryCards;
  public SwipeResults SwipeResults;
  public GameObject MadeBy;
  public GameObject Resources;
  public GameObject LoopReset;

  private GameObject CurrentCard;
  private GameObject CurrentCardGenericCard;
  private Swipe CurrentCardSwipe;
  private Appear CurrentCardAppear;
  private bool CurrentCardIsIntro;
  private TextMeshProUGUI HealthText;
  private TextMeshProUGUI ArmorText;
  private TextMeshProUGUI TurnText;
  private TextMeshProUGUI AimText;
  private Queue<GameObject> CardsToDo = new Queue<GameObject>();
  private List<Appear> PlayedCardsAppear = new List<Appear>();
  private Transform PlayedCardsTransform;

  public delegate void OnCardSwippedDelegate(GameObject card, bool right);
  public event OnCardSwippedDelegate OnCardSwipped;
  public delegate void OnCardDisappearedDelegate(GameObject card, bool swippedRight);
  public event OnCardDisappearedDelegate OnCardDisappeared;

  private int startingHealth = 50;
  private int startingArmor = 100;

  private AudioSource cardAudioSource;
  public AudioState AudioState { get; private set; } = AudioState.NoClip;
  private Queue<AudioClip> audioClips = new Queue<AudioClip>();
  private AudioClip nextCardPreAudioClip;
  private bool autoPlayOn = true;

  public AudioClip SwipeLeftAudioClip;
  public AudioClip SwipeRightAudioClip;
  private AudioSource swipeAudioSource;

  private AudioSource deathAudioSource;
  private AudioSource victoryAudioSource;

  private bool rewinding = false;

  private int health;
  public int Health
  {
    get
    {
      return health;
    }
    set
    {
      if (health != value)
      {
        health = value;

        if (value <= 0)
        {
          health = 0;
          if (!rewinding)
          {
            Death();
          }
        }

        if (!suggestingHealth)
        {
          HealthText.text = health.ToString();
        }
      }
    }
  }

  private int armor;
  public int Armor
  {
    get
    {
      return armor;
    }
    set
    {
      if (armor != value)
      {
        if (value < 0)
        {
          armor = 0;
          Health += value;
        }
        else
        {
          armor = value;
        }

        if (!suggestingArmor)
        {
          ArmorText.text = armor.ToString();
        }
      }
    }
  }

  private int turn;
  public int Turn
  {
    get
    {
      return turn;
    }
    set
    {
      if (turn != value)
      {
        turn = value;
        if (!suggestingTurn)
        {
          TurnText.text = value.ToString();
        }
      }
    }
  }
  private int aim = 1; // tODO
  public int Aim
  {
    get
    {
      return aim;
    }
    set
    {
      if (aim != value)
      {
        aim = value;
        if (!suggestingAim)
        {
          AimText.text = value.ToString();
        }
      }
    }
  }

  public bool AlenNameKnow { get; set; } = false;
  public bool AlenAlive { get; set; } = true;

  private int suggestedHealth = 0;
  private bool suggestingHealth = false;

  private int suggestedArmor = 0;
  private bool suggestingArmor = false;

  private bool suggestingTurn = false;

  private bool suggestingAim = false;

  private static GameMananger instance;
  public static GameMananger Instance
  {
    get
    {
      if (instance == null)
      {
        instance = FindObjectOfType<GameMananger>();
      }

      return instance;
    }
  }

  // Start is called before the first frame update
  void Start()
  {
    PlayedCardsTransform = transform.Find("PlayedCards");
    DrawCard(IntroCard);
    CurrentCardIsIntro = true;

    HealthText = Resources.transform.Find("ResourcesRight").Find("Health").Find("Text").GetComponent<TextMeshProUGUI>();
    ArmorText = Resources.transform.Find("ResourcesRight").Find("Armor").Find("Text").GetComponent<TextMeshProUGUI>();
    TurnText = Resources.transform.Find("ResourcesLeft").Find("Turn").Find("Text").GetComponent<TextMeshProUGUI>();
    AimText = Resources.transform.Find("ResourcesLeft").Find("Aim").Find("Text").GetComponent<TextMeshProUGUI>();

    AimText.text = Aim.ToString();

    cardAudioSource = transform.Find("CardAudio").GetComponent<AudioSource>();
    swipeAudioSource = transform.Find("SwipingAudio").GetComponent<AudioSource>();
    deathAudioSource = transform.Find("DeathAudio").GetComponent<AudioSource>();
    victoryAudioSource = transform.Find("VictoryAudio").GetComponent<AudioSource>();
  }

  // Update is called once per frame
  void Update()
  {
    if (AudioState == AudioState.Playing && cardAudioSource.clip != null && cardAudioSource.time >= cardAudioSource.clip.length)
    {
      if (audioClips.Count > 0)
      {
        AudioState = AudioState.Playing;
        cardAudioSource.clip = audioClips.Dequeue();
        cardAudioSource.Play();
      }
      else
      {
        AudioState = AudioState.NoClip;
        cardAudioSource.clip = null;
      }
    }
    else if (AudioState == AudioState.FadingOut)
    {
      cardAudioSource.volume = Mathf.Clamp01(cardAudioSource.volume - Time.deltaTime);
      if (cardAudioSource.volume <= 0.0f)
      {
        if (audioClips.Count > 0)
        {
          cardAudioSource.Stop();
          cardAudioSource.volume = 1.0f;
          AudioState = AudioState.Playing;
          cardAudioSource.clip = audioClips.Dequeue();
          cardAudioSource.Play();
        }
        else
        {
          AudioState = AudioState.NoClip;
          cardAudioSource.Stop();
          cardAudioSource.clip = null;
          cardAudioSource.volume = 1.0f;
        }
      }
    }

  }

  private void DrawCard(GameObject card)
  {
    CurrentCard = Instantiate(card);
    CurrentCard.transform.SetParent(PlayedCardsTransform);
    CurrentCardGenericCard = CurrentCard.transform.Find("GenericCard").gameObject;
    CurrentCardSwipe = CurrentCardGenericCard.GetComponent<Swipe>();
    CurrentCardAppear = CurrentCardGenericCard.GetComponent<Appear>();

    SwipeResults.ActiveCard = CurrentCardSwipe;
    if (CurrentCardAppear.CurrentState == Appear.State.Hidden)
    {
      CurrentCardAppear.Show();
    }

    CurrentCardAppear.OnStateChanged += CurrentCardAppear_OnStateChanged;
    CurrentCardSwipe.OnSwipeStateChanged += CurrentCardSwipe_OnSwipeStateChanged;
  }

  private void CurrentCardSwipe_OnSwipeStateChanged(SwipeState previousState, SwipeState newState)
  {
    if (newState == SwipeState.SwipedLeft || newState == SwipeState.SwipedRight)
    {
      OnCardSwipped?.Invoke(CurrentCardGenericCard, newState == SwipeState.SwipedRight);
      InterruptCardAudio();

      swipeAudioSource.clip = newState == SwipeState.SwipedRight ? SwipeRightAudioClip : SwipeLeftAudioClip;
      swipeAudioSource.Play();
    }
  }  

  private void CurrentCardAppear_OnStateChanged(Appear.State newState)
  {
    if ((CurrentCardSwipe.SwipeState == SwipeState.SwipedLeft || CurrentCardSwipe.SwipeState == SwipeState.SwipedRight) && newState == Appear.State.Hidden)
    {
      CardSwippedAndDisappeared();
    }
  }

  private void CardSwippedAndDisappeared()
  {
    var card = CurrentCardGenericCard;
    var cardSwipeState = CurrentCardSwipe.SwipeState;
    CurrentCardAppear.OnStateChanged -= CurrentCardAppear_OnStateChanged;
    CurrentCardSwipe.OnSwipeStateChanged -= CurrentCardSwipe_OnSwipeStateChanged;
 
    if (CurrentCardIsIntro)
    {
      CurrentCardIsIntro = false;
    }
    else
    {
      PlayedCardsAppear.Add(CurrentCardAppear);
    }
    card.SetActive(false);

    SwipeResults.ActiveCard = null;

    CurrentCard = null;
    CurrentCardAppear = null;
    CurrentCardSwipe = null;
    CurrentCardGenericCard = null;

    OnCardDisappeared?.Invoke(card, cardSwipeState == SwipeState.SwipedRight);

    if (CardsToDo.Count > 0)
    {
      DrawCard(CardsToDo.Dequeue());
    }
  }

  public void StartGame()
  {
    if (MadeBy.activeInHierarchy)
    {
      MadeBy.SetActive(false);
    }

    if (!Resources.activeInHierarchy)
    {
      Resources.SetActive(true);
    }

    Health = startingHealth;
    Armor = startingArmor;
    Turn = 1;
    AlenAlive = true;

    foreach (var card in MainStoryCards)
    {
      CardsToDo.Enqueue(card);
    }
    DrawCard(CardsToDo.Dequeue());
  }

  public void SuggestNewHealth(int newHealth)
  {
    suggestingHealth = true;
    suggestedHealth = Mathf.Clamp(newHealth, 0, int.MaxValue);
    if (suggestedHealth != Health)
    {
      HealthText.text = suggestedHealth.ToString() + "(" + "<color=\"" + (suggestedHealth > Health ? "green" : "red") + "\">" + (suggestedHealth - Health).ToString() + "</color>)";
    }
  }

  public void ResetSuggestions()
  {
    suggestingHealth = false;
    HealthText.text = Health.ToString();

    suggestingArmor = false;
    ArmorText.text = Armor.ToString();

    suggestingTurn = false;
    TurnText.text = Turn.ToString();

    suggestingAim = false;
    AimText.text = Aim.ToString();
  }

  public void SuggestNewArmor(int newArmor)
  {
    suggestingArmor = true;
    suggestedArmor = newArmor;
    if (suggestedArmor != Armor)
    {
      if (suggestedArmor < 0)
      {
        SuggestNewHealth(Health + suggestedArmor);
        suggestedArmor = 0;
      }

      ArmorText.text = suggestedArmor.ToString() + "(" + "<color=\"" + (suggestedHealth > Health ? "green" : "red") + "\">" + (suggestedArmor - Armor).ToString() + "</color>)";
    }
  }

  public void SuggestUnkownStat()
  {
    suggestingArmor = true;
    suggestingHealth = true;
    suggestingAim = true;

    HealthText.text = Health.ToString() + "(???)";
    ArmorText.text = Armor.ToString() + "(???)";
    AimText.text = Aim.ToString() + "(???)";
  }

  public void SuggestTurnIncrease()
  {
    suggestingTurn = true;
    TurnText.text = Turn.ToString() + "(+1)";
  }
  
  public void SuggestAimIncrease()
  {
    suggestingAim = true;
    AimText.text = Aim.ToString() + "(+1)";
  }

  public void SuggestTurnConditionNotMet()
  {
    suggestingTurn = true;
    TurnText.text = Turn.ToString() + "(<color=\"red\">X</color>)";
  }

  public void SuggestAimConditionNotMet()
  {
    suggestingAim = true;
    AimText.text = Aim.ToString() + "(<color=\"red\">X</color>)";
  }

  public void Death()
  {
    deathAudioSource.Play();
    rewinding = true;
    Health = 0;
    Armor = 0;
    CardsToDo.Clear();
    StartCoroutine(Rewind());
  }

  public void Restart()
  {
    rewinding = true;
    CardsToDo.Clear();
    StartCoroutine(Rewind());
  }

  IEnumerator Rewind()
  {
    var initialHealth = Health;
    var initialArmor = Armor;
    var initalTurn = Turn;

    LoopReset.SetActive(true);

    var timeT = 0.0f;
    var animationT = 0.0f;
    var duration = 5.0f;
    var perCardT = 0.9 / PlayedCardsAppear.Count;

    PlayedCardsAppear.Reverse();

    while (true)
    {
      timeT += Time.deltaTime / duration;
      animationT = Mathf.Pow(timeT, 4.106f);

      Turn = Mathf.FloorToInt(Mathf.Lerp(initalTurn, 1.0f, animationT));
      Health = Mathf.FloorToInt(Mathf.Lerp(initialHealth, startingHealth, animationT));
      Armor = Mathf.FloorToInt(Mathf.Lerp(initialArmor, startingArmor, animationT));

      for (int i = 0; i < PlayedCardsAppear.Count; i++)
      {
        var cardAppear = PlayedCardsAppear[i];
        if (cardAppear.CurrentState == Appear.State.Hidden && animationT > 0.05f + i * perCardT)
        {
          cardAppear.gameObject.SetActive(true);
          cardAppear.Show();
        }
      }

      if (timeT >= 1.0f)
      {
        break;
      }

      yield return null;
    }

    foreach (var playedCard in PlayedCardsAppear)
    {
      GameObject.Destroy(playedCard.transform.parent.gameObject);
    }
    PlayedCardsAppear.Clear();

    LoopReset.SetActive(false);
    CardsToDo.Clear();

    rewinding = false;

    StartGame();
  }

  public void EnqueueCards(IEnumerable<GameObject> cards)
  {
    if (Health > 0)
    {
      foreach (var card in cards)
      {
        CardsToDo.Enqueue(card);
      }
    }
  }

  public void PlayCardAudio(AudioClip clip)
  {
    if (nextCardPreAudioClip != null)
    {
      cardAudioSource.clip = nextCardPreAudioClip;
      audioClips.Enqueue(clip);
      nextCardPreAudioClip = null;
    }
    else
    {
      cardAudioSource.clip = clip;

    }

    cardAudioSource.Play();
    AudioState = AudioState.Playing;

  }

  public void PauseCardAudio()
  {
    if (AudioState == AudioState.Playing)
    {
      cardAudioSource.Pause();
      AudioState = AudioState.Paused;
    }
  }
  
  public void UnpauseCardAudio()
  {
    if (AudioState == AudioState.Paused)
    {
      cardAudioSource.UnPause();
      AudioState = AudioState.Playing;
    }
  }

  private void InterruptCardAudio()
  {
    if (AudioState == AudioState.Playing)
    {
      audioClips.Clear();
      AudioState = AudioState.FadingOut;
    }
    else if (AudioState == AudioState.Paused || AudioState == AudioState.NoClip)
    {
      audioClips.Clear();
      AudioState = AudioState.NoClip;
      cardAudioSource.clip = null;
    }
  }

  public void ScheduleNextCardAudio(AudioClip clip)
  {
    nextCardPreAudioClip = clip;
  }

  public bool CardWithAudioSpawned(AudioClip clip)
  {
    if (autoPlayOn)
    {
      PlayCardAudio(clip);
      return true;
    }

    return false;
  }

  public void Victory()
  {
    victoryAudioSource.Play();
  }

  public void OnAutoPlayValueChanged(Toggle toggle)
  {
    autoPlayOn = toggle.isOn;
  }
}
