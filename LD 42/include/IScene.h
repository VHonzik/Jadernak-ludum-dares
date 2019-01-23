#pragma once

#include <cstdint>
#include <vector>

namespace CherEngine
{
  class Game;
  class Sprite;
  struct Padding;

  class IScene
  {
  public:
    IScene(Game& game);

    void Load();

    virtual void DoLoad() {};
    virtual void DoRestart() {};
    virtual void Update() {};

    Sprite* CreateSprite(const char* texture_name);
    Sprite* CreateBoxSprite(const char* texture_name, int32_t width, int32_t height, int32_t gridSize);
    Sprite* CreateTextBox(const char* font_name, const char* text, int32_t width, int32_t height);
    Sprite* CreateTooltip(const char* texture_name, const char* font_name, const char* text,
      int32_t width, int32_t height, int32_t gridSize, const Padding& padding);

    void DisableAllSprites();
    void Unload();
    bool WasLoaded();

  protected:
    Game& _game;
    bool _loaded;

    std::vector<Sprite*> _sprites;
  };
}
