using Listonos.AudioSystem;
using Listonos.NavigationSystem;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
  public GameObject PageInputArea;
  public GameObject ExitArea;
  public GameObject VolumeArea;
  public GameObject VolumeBarsGroup;
  public GameObject VolumeMuteOn;
  public GameObject VolumeMuteOff;
  public TMPro.TextMeshProUGUI PageInputText;
  public float NoInputConfirmationDuration;
  public float ExitWindowDuration;
  public float VolumeWindowDuration;
  public int PageToRouteWhenInvalidPageEntered;
  public int StartingPage;
  public int HomePage;
  public int DecisionAlreadyMadePage;
  public int LDRatePage;
  public AudioClip IntroPageMusic;
  public NavigationSystemInt NavigationSystem;

  public Decisions Decisions { get; private set; } = new Decisions();

  public event EventHandler<NavigationSystemInt.ScreenChangedEventArgs> AfterScreenChanged;

  private bool entertingPageNumber = false;
  private string inputPageNumberString = "";
  private float inputTimer;
  private float exitTimer;
  private float volumeTimer;

  private HashSet<int> validPages = new HashSet<int>();

  private List<int> pageHistory = new List<int>();
  private int pageHistoryIndex;

  private List<GameObject> volumeBars = new List<GameObject>();
  private int volumeBarsShown;
  private bool volumeMuted;

  void Awake()
  {
    Debug.Log("Game manager started!");
    NavigationSystem.InitialScreen = StartingPage;
    pageHistory.Add(StartingPage);
    pageHistoryIndex = 0;
    exitTimer = ExitWindowDuration;
    volumeTimer = VolumeWindowDuration;
    volumeMuted = false;

    for (int i = VolumeBarsGroup.transform.childCount - 1; i >= 0; i--)
    {
      volumeBars.Add(VolumeBarsGroup.transform.GetChild(i).gameObject);
    }

    volumeBarsShown = volumeBars.Count / 2;
    UpdateVolumeBars();
  }

  // Start is called before the first frame update
  void Start()
  {
    if (IntroPageMusic != null)
    {
      AudioManager.Instance.PlayMusicClip(IntroPageMusic);
    }
  }

  void Update()
  {
    if (entertingPageNumber)
    {
      inputTimer -= Time.deltaTime;
      if (inputTimer <= 0.0f)
      {
        FinishEnteringPage();
      }
    }

    for (int i = 0; i <= KeyCode.Alpha9 - KeyCode.Alpha0; i++)
    {
      if (Input.GetKeyDown(KeyCode.Alpha0 + i))
      {
        NumberButtonPressed(i);
      }
    }

    for (int i = 0; i <= KeyCode.Keypad9 - KeyCode.Keypad0; i++)
    {
      if (Input.GetKeyDown(KeyCode.Keypad0 + i))
      {
        NumberButtonPressed(i);
      }
    }

    if (Input.GetKeyDown(KeyCode.Backspace))
    {
      BackspaceButtonPressed();
    }

    if (entertingPageNumber && (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return)))
    {
      FinishEnteringPage();
    }

    if (Input.GetKeyDown(KeyCode.Home))
    {
      HomeButtonPressed();
    }

    if (Input.GetKeyDown(KeyCode.PageUp) || Input.GetMouseButtonDown(3))
    {
      BackButtonPressed();
    }

    if (Input.GetKeyDown(KeyCode.PageDown) || Input.GetMouseButtonDown(4))
    {
      ForwardButtonPressed();
    }

    if (ExitArea.activeInHierarchy)
    {
      exitTimer -= Time.deltaTime;
      if (exitTimer <= 0.0f)
      {
        exitTimer = ExitWindowDuration;
        ExitArea.SetActive(false);
      }
    }

    if (VolumeArea.activeInHierarchy)
    {
      volumeTimer -= Time.deltaTime;
      if (volumeTimer <= 0.0f)
      {
        volumeTimer = VolumeWindowDuration;
        VolumeMuteOn.SetActive(false);
        VolumeMuteOff.SetActive(false);
        VolumeArea.SetActive(false);
      }
    }
  }

  public void RedButtonPressed()
  {
    EnterPage(100);
  }

  public void GreenButtonPressed()
  {
    EnterPage(200);
  }

  public void YellowButtonPressed()
  {
    EnterPage(300);
  }

  public void BlueButtonPressed()
  {
    EnterPage(400);
  }

  public void HomeButtonPressed()
  {
    EnterPage(HomePage);
  }

  public void NumberButtonPressed(int number)
  {
    Debug.Assert(number >= 0 && number <= 9);
    inputTimer = NoInputConfirmationDuration;
    if (!entertingPageNumber)
    {
      entertingPageNumber = true;
      inputPageNumberString = number.ToString();
      PageInputArea.SetActive(true);
      PageInputText.text = FormatPageNumber();
    }
    else
    {
      if (inputPageNumberString.Length < 3)
      {
        inputPageNumberString += number.ToString();
        PageInputText.text = FormatPageNumber();
      }
    }
  }

  public void PowerButtonPressed()
  {
    if (ExitArea.activeInHierarchy)
    {
#if UNITY_EDITOR
      UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit(0);
#endif
    }
    else
    {
      ExitArea.SetActive(true);
    }
  }

  public void BackButtonPressed()
  {
    if (pageHistoryIndex > 0)
    {
      pageHistoryIndex = Mathf.Clamp(pageHistoryIndex - 1, 0, pageHistory.Count);
      ChangePage(pageHistory[pageHistoryIndex]);
    }
  }

  public void ForwardButtonPressed()
  {
    if (pageHistoryIndex < pageHistory.Count - 1)
    {
      pageHistoryIndex = Mathf.Clamp(pageHistoryIndex + 1, 0, pageHistory.Count);
      ChangePage(pageHistory[pageHistoryIndex]);
    }
  }

  public void BackspaceButtonPressed()
  {
    inputTimer = NoInputConfirmationDuration;
    if (entertingPageNumber && inputPageNumberString.Length > 0)
    {
      inputPageNumberString = inputPageNumberString.Remove(inputPageNumberString.Length - 1);
      PageInputText.text = FormatPageNumber();
    }
  }

  public void OkButtonPressed()
  {
    if (entertingPageNumber)
    {
      FinishEnteringPage();
    }
  }

  public void VolumeDownButtonPressed()
  {
    volumeMuted = false;
    volumeTimer = VolumeWindowDuration;
    VolumeArea.SetActive(true);
    VolumeMuteOn.SetActive(false);
    VolumeMuteOff.SetActive(false);

    volumeBarsShown = Mathf.Clamp(volumeBarsShown - 1, 0, volumeBars.Count);
    AudioManager.Instance.SetMusicVolume((float)volumeBarsShown / volumeBars.Count);
    UpdateVolumeBars();
  }

  public void VolumeUpButtonPressed()
  {
    volumeMuted = false;
    volumeTimer = VolumeWindowDuration;
    VolumeArea.SetActive(true);

    volumeBarsShown = Mathf.Clamp(volumeBarsShown + 1, 0, volumeBars.Count);
    AudioManager.Instance.SetMusicVolume((float)volumeBarsShown / volumeBars.Count);
    UpdateVolumeBars();
  }

  public void VolumeMuteButtonPressed()
  {
    volumeTimer = VolumeWindowDuration;
    VolumeArea.SetActive(true);
    AudioManager.Instance.SetMusicVolume(0.0f);

    volumeMuted = !volumeMuted;
    if (volumeMuted)
    {
      AudioManager.Instance.SetMusicVolume(0.0f);
      VolumeMuteOn.SetActive(true);
    }
    else
    {
      VolumeMuteOff.SetActive(true);
      AudioManager.Instance.SetMusicVolume((float)volumeBarsShown / volumeBars.Count);
    }
  }

  public void AddValidPage(int pageNumber)
  {
    validPages.Add(pageNumber);
  }

  private string FormatPageNumber()
  {
    if (inputPageNumberString.Length == 0)
    {
      return "___";
    }

    Debug.Assert(inputPageNumberString.Length < 4);

    return inputPageNumberString.PadLeft(3, '_');
  }

  public void EnterPage(int pageNumber)
  {
    var finalPage = pageNumber;
    if (!validPages.Contains(pageNumber))
    {
      finalPage = PageToRouteWhenInvalidPageEntered;
    }

    var startingRemovalIndex = pageHistoryIndex + 1;
    var historyElementsToRemove = pageHistory.Count - startingRemovalIndex;
    if (historyElementsToRemove > 0)
    {
      pageHistory.RemoveRange(startingRemovalIndex, historyElementsToRemove);
    }

    pageHistory.Add(finalPage);
    pageHistoryIndex = pageHistory.Count - 1;
    ChangePage(finalPage);
  }

  public void RedirectPage(int pageNumber)
  {
    var finalPage = pageNumber;
    if (!validPages.Contains(pageNumber))
    {
      finalPage = PageToRouteWhenInvalidPageEntered;
    }

    var startingRemovalIndex = pageHistoryIndex;
    var historyElementsToRemove = pageHistory.Count - startingRemovalIndex;
    if (historyElementsToRemove > 0)
    {
      pageHistory.RemoveRange(startingRemovalIndex, historyElementsToRemove);
    }

    pageHistory.Add(finalPage);
    pageHistoryIndex = pageHistory.Count - 1;
    ChangePage(finalPage);
  }

  private void ChangePage(int pageNumber)
  {
    NavigationSystem.CurrentScreen = pageNumber;
    if (pageNumber == StartingPage)
    {
      Decisions.Reset();
    }
    AfterScreenChanged?.Invoke(this, new NavigationSystem<int>.ScreenChangedEventArgs() { NewScreen = pageNumber });
  }

  private void FinishEnteringPage()
  {
    inputTimer = -1.0f;
    entertingPageNumber = false;
    PageInputArea.SetActive(false);

    if (inputPageNumberString.Length > 0)
    {
      var pageNumber = int.Parse(inputPageNumberString);
      Debug.Assert(pageNumber >= 0 && pageNumber <= 999);
      inputPageNumberString = "";
      EnterPage(pageNumber);
    }
  }

  private void UpdateVolumeBars()
  {
    for (int i = 0; i < volumeBars.Count; i++)
    {
      volumeBars[i].SetActive(i < volumeBarsShown);
    }
  }
}
