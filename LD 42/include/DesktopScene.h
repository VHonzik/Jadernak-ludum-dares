#pragma once

#include "IScene.h"

namespace CherEngine
{
  class Sprite;
}

namespace LD42
{
  extern bool SteamRevealed;

  class DesktopScene : public CherEngine::IScene
  {
  public:
    DesktopScene(CherEngine::Game& game);
    void DoLoad() override;
    void DoRestart() override;
  private:
    CherEngine::Sprite* _bgsprite;
    CherEngine::Sprite* _pcsprite;
    CherEngine::Sprite* _pchoversprite;
    CherEngine::Sprite* _steamsprite;
    CherEngine::Sprite* _steamhoversprite;
  };
}