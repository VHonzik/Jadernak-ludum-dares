using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAudio : MonoBehaviour
{
  public Sprite BackgroundTexture;
  public Sprite BackgroundHoverTexture;
  public Sprite BackgroundPressedTexture;
  public Sprite IconPlayTexture;
  public Sprite IconPauseTexture;

  public AudioClip CardAudioClip;
  public AudioClip SwipeLeftAudioClip;
  public AudioClip SwipeRightAudioClip;

  private SpriteRenderer BackgroundSprite;
  private SpriteRenderer IconSprite;
  private Collider2D ButtonCollider;

  private Swipe swipe;

  private bool hovered = false;
  private bool pressing = false;

  // Start is called before the first frame update
  void Start()
  {
    BackgroundSprite = transform.Find("ButtonBackground").GetComponent<SpriteRenderer>();
    IconSprite = transform.Find("ButtonIcon").GetComponent<SpriteRenderer>();
    ButtonCollider = transform.Find("ButtonBackground").GetComponent<Collider2D>();

    BackgroundSprite.sprite = BackgroundTexture;
    IconSprite.sprite = IconPlayTexture;

    swipe = GetComponent<Swipe>();
    swipe.OnSwipeStateChanged += Swipe_OnSwipeStateChanged;

    if (CardAudioClip != null)
    {
      if(GameMananger.Instance.CardWithAudioSpawned(CardAudioClip))
      {
        IconSprite.sprite = IconPauseTexture;
      }
    }
  }

  private void Swipe_OnSwipeStateChanged(SwipeState previousState, SwipeState newState)
  {
    if (newState == SwipeState.SwipedLeft && SwipeLeftAudioClip != null)
    {
      GameMananger.Instance.ScheduleNextCardAudio(SwipeLeftAudioClip);
    }
    else if (newState == SwipeState.SwipedRight && SwipeRightAudioClip != null)
    {
      GameMananger.Instance.ScheduleNextCardAudio(SwipeRightAudioClip);
    }
  }

  // Update is called once per frame
  void Update()
  {
    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
    var hover = hit.collider ? hit.collider == ButtonCollider : false;

    if (hover != hovered)
    {
      hovered = hover;
      if (!pressing)
      {
        BackgroundSprite.sprite = hovered ? BackgroundHoverTexture : BackgroundTexture;
      }
    }

    if (Input.GetMouseButtonDown(0) && hovered)
    {
      BackgroundSprite.sprite = BackgroundPressedTexture;
      pressing = true;
    }
    else if (Input.GetMouseButtonUp(0) && pressing)
    {
      pressing = false;
      if (hovered)
      {
        OnClick();
        BackgroundSprite.sprite = BackgroundHoverTexture;
      }
      else
      {
        BackgroundSprite.sprite = BackgroundTexture;
      }
    }

    if (IconSprite.sprite == IconPauseTexture && GameMananger.Instance.AudioState != AudioState.Playing)
    {
      IconSprite.sprite = IconPlayTexture;
    }
  }

  private void OnClick()
  {
    if (GameMananger.Instance.AudioState == AudioState.Playing)
    {
      GameMananger.Instance.PauseCardAudio();
      IconSprite.sprite = IconPlayTexture;
    }
    else if (GameMananger.Instance.AudioState == AudioState.Paused)
    {
      GameMananger.Instance.UnpauseCardAudio();
      IconSprite.sprite = IconPauseTexture;
    }
    else
    {
      if (CardAudioClip != null)
      {
        GameMananger.Instance.PlayCardAudio(CardAudioClip);
        IconSprite.sprite = IconPauseTexture;
      }
    }
  }
}
