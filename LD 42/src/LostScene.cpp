#include "LostScene.h"
#include "Game.h"
#include "Sprite.h"

namespace LD42
{
  LostScene::LostScene(CherEngine::Game& game)
    : CherEngine::IScene(game)
  {
  }

  void LostScene::DoLoad()
  {
    _bgsprite = CreateSprite("youlost");
  }

  void LostScene::DoRestart()
  {
    _bgsprite->SetVisible(true);

    _bgsprite->RegisterOnClick([&](CherEngine::Sprite*) {
      _game.StartScene("desktopScene");
    });
  }
}

