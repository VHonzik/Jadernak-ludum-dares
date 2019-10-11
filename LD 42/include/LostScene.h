#pragma once

#include "IScene.h"

namespace CherEngine
{
  class Sprite;
}

namespace LD42
{
  class LostScene : public CherEngine::IScene
  {
  public:
    LostScene(CherEngine::Game& game);
    void DoLoad() override;
    void DoRestart() override;
  private:
    CherEngine::Sprite* _bgsprite;
  };
}