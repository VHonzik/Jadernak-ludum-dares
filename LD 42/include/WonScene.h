#pragma once

#include "IScene.h"

namespace CherEngine
{
  class Sprite;
}

namespace LD42
{
  class WonScene : public CherEngine::IScene
  {
  public:
    WonScene(CherEngine::Game& game);
    void DoLoad() override;
    void DoRestart() override;
  private:
    CherEngine::Sprite* _bgsprite;
  };
}