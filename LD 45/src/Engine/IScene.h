#pragma once

#include <memory>

namespace LD45
{
  class Game;
  class Sprite;

  class IScene
  {
  public:
    virtual void Update() {};
    virtual void SpriteHovered(Sprite* oldSprite, Sprite* newSprite) {};
    virtual void Start() {};

    bool GetInitialized() const { return _initialized; }
    void SetInitialized(bool initialized) { _initialized = initialized; }
    bool GetResetWanted() const { return _resetWanted; }
    void Reset() { _resetWanted = true; }
    virtual std::shared_ptr<IScene> DoReset() { return std::make_shared<IScene>(); };

  private:
    bool _initialized = false;
    bool _resetWanted = false;
  };
}