using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatDisplay : MonoBehaviour, SpaceStationStatListener
{
  public SpaceStationStat Stat;
  public bool LeftAlign;

  private TextMeshProUGUI _text;
  private GameState _gameState;

  public void OnStatChanged(SpaceStationStat stat, SpaceStationStatData statData)
  {
    if (stat == Stat)
    {
      _text.text = statData.Format(LeftAlign);
    }
  }

  // Start is called before the first frame update
  void Awake()
  {
    _text = GetComponent<TextMeshProUGUI>();
    _gameState = FindObjectOfType<GameState>();
  }

  void Start()
  {
    _gameState.RegisterSpaceStationStatListener(this);
  }

  // Update is called once per frame
  void Update()
  {

  }
}
