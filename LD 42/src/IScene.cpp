#include "IScene.h"
#include "Game.h"
#include "Sprite.h"

namespace CherEngine
{
  IScene::IScene(Game& game)
    : _game(game)
    , _loaded(false)
  {

  }

  Sprite* IScene::CreateSprite(const char* texture_name)
  {
    auto sprite = _game.CreateSprite(texture_name);
    _sprites.push_back(sprite);
    return sprite;
  }

  Sprite* IScene::CreateBoxSprite(const char* texture_name, int32_t width, int32_t height, int32_t gridSize)
  {
    auto sprite = _game.CreateBoxSprite(texture_name, width, height, gridSize);
    _sprites.push_back(sprite);
    return sprite;
  }

  Sprite* IScene::CreateTextBox(const char* font_name, const char* text, int32_t width, int32_t height)
  {
    auto sprite = _game.CreateTextBox(font_name, text, width, height);
    _sprites.push_back(sprite);
    return sprite;
  }

  Sprite* IScene::CreateTooltip(const char* texture_name, const char* font_name, const char* text,
    int32_t width, int32_t height, int32_t gridSize, const Padding& padding)
  {
    auto sprite = _game.CreateTooltip(texture_name, font_name, text, width, height, gridSize, padding);
    _sprites.push_back(sprite);
    return sprite;
  }

  void IScene::DisableAllSprites()
  {
    for (auto sprite : _sprites)
    {
      sprite->RegisterOnHover(nullptr);
      sprite->RegisterOnHoverExit(nullptr);
      sprite->RegisterOnDoubleClick(nullptr);
      sprite->RegisterOnClick(nullptr);
      sprite->SetVisible(false);
    }
  }

  void IScene::Load()
  {
    _loaded = true;
    DoLoad();
    DoRestart();
  }

  void  IScene::Unload()
  {
    _sprites.clear();

    _loaded = false;
  }

  bool IScene::WasLoaded()
  {
    return _loaded;
  }
}