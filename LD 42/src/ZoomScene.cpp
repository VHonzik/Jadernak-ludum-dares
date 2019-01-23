#include "ZoomScene.h"

#include "Time.h"
#include "Input.h"
#include "Camera.h"
#include "Utils.h"
#include "Game.h"
#include "Sprite.h"

#include "DesktopScene.h"

#include <algorithm>

using namespace CherEngine;

namespace
{
  const float kScales[4] = {1.5f, 3.0f, 6.0f, 12.0f };
  const float kCenterX[4] = { -50.0f, -200.0f, -800.0f, -2400.0f };
  const float kCenterY[4] = { -100.0f, -600.0f, -1800.0f, -4000.0f };

  const float kInitialDelay = 1.0f;
  const float kExitDelay = 1.8f;
  const float kZoomDelay = 1.5f;
}

namespace LD42
{
  ZoomScene::ZoomScene(CherEngine::Game& game)
    : IScene(game)
    , _firstTime(true)
  {
  }

  void ZoomScene::DoLoad()
  {
    _pcWindow = CreateSprite("thispcwindow");
  }

  void ZoomScene::DoRestart()
  {
    _pcWindow->SetVisible(true);
    _pcWindow->SetScale(1.0f);
    _pcWindow->SetCenterPosition(0, 0);
    _index = -1;
    _timer = kInitialDelay;
  }

  void ZoomScene::Update()
  {
    _timer -= GTime.deltaTime;

    if (_timer <= 0.0f && _index < 3)
    {
      _index++;
      _timer += kZoomDelay;
      _pcWindow->SetScale(kScales[_index]);
      _pcWindow->SetCenterPosition(static_cast<int32_t>(kCenterX[_index]), static_cast<int32_t>(kCenterY[_index]));

      if (_index == 3)
      {
        _game.PlaySound("chordlong");
        _timer += kExitDelay;
      }
      else
      {
        _game.PlaySound("chord");
      }
    }
    else if (_timer <= 0.0f && _index == 3)
    {
      if (_firstTime)
      {
        SteamRevealed = true;
        _game.StartScene("steamScene");
      }
      else
      {
        _game.StartScene("desktopScene");
      }

      _firstTime = false;

    }

  }
}