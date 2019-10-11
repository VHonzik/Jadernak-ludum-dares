#include "WonScene.h"
#include "Game.h"
#include "Sprite.h"

namespace LD42
{
  WonScene::WonScene(CherEngine::Game& game)
    : CherEngine::IScene(game)
  {
  }

  void WonScene::DoLoad()
  {
    _bgsprite = CreateSprite("youwon");
  }

  void WonScene::DoRestart()
  {
    _bgsprite->SetVisible(true);

    _bgsprite->RegisterOnClick([&](CherEngine::Sprite*) {
      _game.StartScene("desktopScene");
    });
  }
}

