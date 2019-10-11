#pragma once

#include "IScene.h"

namespace CherEngine
{
  class Sprite;
}

namespace LD42
{
  class ZoomScene : public CherEngine::IScene
  {
  public:
    ZoomScene(CherEngine::Game& game);
    void DoLoad() override;
    void DoRestart() override;
    void Update() override;
  private:
    CherEngine::Sprite* _pcWindow;
    int32_t _index;
    float _timer;
    bool _firstTime;
  };
}