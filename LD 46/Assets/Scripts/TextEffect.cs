using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Linq;

[System.Serializable]
public class TEffect
{
  public bool AnimateOnStart = false;
  public float AnimateOnStartDelay = 0.0f;
  protected string _content;

  public void DoStart(MonoBehaviour obj, TextMeshProUGUI text)
  {
    if (AnimateOnStart)
    {
      var coroutine = Animate(obj, text);
      obj.StartCoroutine(coroutine);
    }
  }

  protected virtual IEnumerator OnAnimationStart(TextMeshProUGUI text) { yield break; }
  protected virtual IEnumerator OnAnimationBody(TextMeshProUGUI text) { yield break; }

  public IEnumerator Animate(MonoBehaviour obj, TextMeshProUGUI text)
  {
    _content = text.text;
    var startCoroutine = OnAnimationStart(text);
    yield return obj.StartCoroutine(startCoroutine);

    if (AnimateOnStartDelay > 0.0f)
    {
      yield return new WaitForSeconds(AnimateOnStartDelay);
    }

    var bodyCoroutine = OnAnimationBody(text);
    yield return obj.StartCoroutine(bodyCoroutine);
  }
}

[System.Serializable]
public class SerialTEffect : TEffect
{
  public bool FixedDuration = false;
  public float CharactersPerSecond = 16.0f; // 200 wpm for comprehension reading * ~5 characters per word / 60 secs
  public float Duration = 1.0f;
}

[System.Serializable]
public class BlinkingEffect : TEffect
{
  public float DurationOnOff = 0.5f;
  protected override IEnumerator OnAnimationBody(TextMeshProUGUI text)
  {
    float timer = 0.0f;

    while(true)
    {
      timer += Time.deltaTime;

      if (timer > DurationOnOff)
      {
        timer -= DurationOnOff;

        text.text = text.text.Length > 1 ? " " : _content;
      }

      yield return null;
    }
  }
}

[System.Serializable]
public class TypeOutEffect : SerialTEffect
{
  protected override IEnumerator OnAnimationStart(TextMeshProUGUI text)
  {
    text.text = "";
    yield break;
  }

  protected override IEnumerator OnAnimationBody(TextMeshProUGUI text)
  {
    float speed = CharactersPerSecond;

    if (FixedDuration)
    {
      speed = _content.Length / Duration;
    }

    int displayedCharacters = 0;
    float charactersWritten = 0.0f;

    while (displayedCharacters != _content.Length)
    {
      charactersWritten += Time.deltaTime * speed;
      var wantedCharacters = Mathf.FloorToInt(charactersWritten);
      if (wantedCharacters != displayedCharacters)
      {
        displayedCharacters = wantedCharacters;
        text.text = _content.Substring(0, displayedCharacters);
      }

      yield return null;
    }
  }
}

[System.Serializable]
public class SerialScrambleEffect : SerialTEffect
{
  public float ScramblingPerSecond = 10.0f;
  public float FixingDelay = 0.0f;

  protected List<char> CharSet = new List<char>();
  protected System.Random random = new System.Random();

  SerialScrambleEffect()
  {
    for (int c = 'a'; c <= 'z'; c++)
    {
      CharSet.Add((char)c);
    }

    for (int c = '0'; c <= '9'; c++)
    {
      CharSet.Add((char)c);
    }

    CharSet.Add('.');
  }

  protected char RandomChar()
  {
    return CharSet[random.Next(CharSet.Count)];
  }

  protected string Scramble(string text)
  {
    var result = text.Select(c => {
      if (char.IsWhiteSpace(c)) return c;

      var r = RandomChar();
      if (char.IsUpper(c))
      {
        r = char.ToUpper(r);
      }

      while (c == r)
      {
        r = RandomChar();
        if (char.IsUpper(c))
        {
          r = char.ToUpper(r);
        }
      }

      return r;
    });
    return new string(result.ToArray());
  }

  protected override IEnumerator OnAnimationStart(TextMeshProUGUI text)
  {
    text.text = Scramble(text.text);
    yield break;
  }

  protected override IEnumerator OnAnimationBody(TextMeshProUGUI text)
  {
    float fixingSpeed = CharactersPerSecond;

    if (FixedDuration)
    {
      fixingSpeed = _content.Length / Duration;
    }

    float fixingDelayT = FixingDelay;
    float charactersFixed = 0.0f;
    float scramblingT = 0.0f;
    string fixedString = "";

    while (fixedString.Length != _content.Length)
    {
      if (fixingDelayT > 0.0f)
      {
        fixingDelayT -= Time.deltaTime;
      }
      else
      {
        charactersFixed += Time.deltaTime * fixingSpeed;

        var wantedCharacters = Mathf.FloorToInt(charactersFixed);
        if (wantedCharacters != fixedString.Length)
        {
          fixedString = _content.Substring(0, wantedCharacters);
        }
      }

      scramblingT += Time.deltaTime * ScramblingPerSecond;

      if (scramblingT > 1.0f)
      {
        scramblingT -= 1.0f;
        text.text = fixedString + Scramble(text.text.Substring(fixedString.Length));
      }

      yield return null;
    }

    text.text = fixedString;
  }
}

public class TextEffect : MonoBehaviour
{
  public TypeOutEffect TypeOut;
  public SerialScrambleEffect SerialScramble;
  public BlinkingEffect Blinking;

  private TextMeshProUGUI _uiText;
  private string _content;

  void Awake()
  {
    _uiText = GetComponent<TextMeshProUGUI>();
    _content = _uiText.text;
  }

  // Start is called before the first frame update
  void Start()
  {
    TypeOut.DoStart(this, _uiText);
    SerialScramble.DoStart(this, _uiText);
    Blinking.DoStart(this, _uiText);
  }

  public void AnimateTypeOut()
  {
    var coroutine = TypeOut.Animate(this, _uiText);
    StartCoroutine(coroutine);
  }

  public void AnimateSerialScramble()
  {
    var coroutine = SerialScramble.Animate(this, _uiText);
    StartCoroutine(coroutine);
  }

  public void AnimateBlinking()
  {
    var coroutine = Blinking.Animate(this, _uiText);
    StartCoroutine(coroutine);
  }

  public void SetText(string text)
  {
    _uiText.text = text;
  }

  // Update is called once per frame
  void Update()
  {

  }
}
