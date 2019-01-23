#include "DesktopScene.h"
#include "Game.h"
#include "Sprite.h"

namespace LD42
{
  bool SteamRevealed = false;

  DesktopScene::DesktopScene(CherEngine::Game& game)
    : CherEngine::IScene(game)
  {
  }

  void DesktopScene::DoLoad()
  {
    _bgsprite = CreateSprite("emptydesktop");

    _pcsprite = CreateSprite("thispc");
    _pcsprite->SetCenterPosition(-512 + 111, -384 + 141);

    _pchoversprite = CreateSprite("thispc_hover");
    _pchoversprite->SetCenterPosition(-512 + 111, -384 + 141);


    _steamsprite = CreateSprite("steam");
    _steamsprite->SetCenterPosition(-512 + 342, -384 + 141);
    _steamhoversprite = CreateSprite("steam_hover");
    _steamhoversprite->SetCenterPosition(-512 + 342, -384 + 141);
  }

  void DesktopScene::DoRestart()
  {
    _bgsprite->SetVisible(true);
    _pcsprite->SetVisible(true);
    _pchoversprite->SetVisible(false);

    _pcsprite->RegisterOnHover([&](CherEngine::Sprite*) {
      _pcsprite->SetVisible(false);
      _pchoversprite->SetVisible(true);
    });

    _pchoversprite->RegisterOnHoverExit([&](CherEngine::Sprite*) {
      _pcsprite->SetVisible(true);
      _pchoversprite->SetVisible(false);
    });

    _pchoversprite->RegisterOnDoubleClick([&](CherEngine::Sprite*) {
      _game.StartScene("zoomScene");
    });

    if (SteamRevealed)
    {
      _steamsprite->SetVisible(true);
      _steamhoversprite->SetVisible(false);

      _steamsprite->RegisterOnHover([&](CherEngine::Sprite*) {
        _steamsprite->SetVisible(false);
        _steamhoversprite->SetVisible(true);
      });

      _steamhoversprite->RegisterOnHoverExit([&](CherEngine::Sprite*) {
        _steamsprite->SetVisible(true);
        _steamhoversprite->SetVisible(false);
      });

      _steamhoversprite->RegisterOnDoubleClick([&](CherEngine::Sprite*) {
        _game.StartScene("steamScene");
      });
    }
    else
    {
      _steamsprite->SetVisible(false);
      _steamhoversprite->SetVisible(false);
    }
  }
}

