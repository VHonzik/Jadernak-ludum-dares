#pragma once

#include <SDL.h>
#include <SDL_ttf.h>

#include "Texture.h"
#include "ITextObject.h"
#include "ICompositeObject.h"
#include "Sprite.h"
#include "DisplayModeInfo.h"

#include <memory>
#include <array>
#include <vector>
#include <unordered_map>
#include <random>

namespace LD45
{
  struct FontDescription
  {
    std::string name;
    TTF_Font* ttfFont;
    uint32_t size;
  };

  struct CursorDescription
  {
    SDL_Cursor* cursor;
    SDL_Surface* surface;
  };

  struct Renderable
  {
    void* data;
    int z;
    std::function<void(void*, SDL_Renderer*)> render;

    static bool Compare(const Renderable& a, const Renderable& b)
    {
      return a.z < b.z;
    }
  };

  template<typename T>
  Renderable make_renderable(T* object);

  class IScene;
  class Text;
  class FTC;
  class Button;

  struct Style;

  class Game
  {
  public:
    Game();
    bool Initialize(const Style& style);
    void CleanUp();
    void Start();

    bool LoadFont(const Style& style, const char* assetName, const char* fontFile);
    TTF_Font* FindFont(const std::string& fontName, const uint32_t size) const;

    bool LoadTexture(const char* assetName, const char* textureFile, bool hitsRequired);
    std::shared_ptr<Texture> FindTexture(const std::string& textureName) const;
    std::shared_ptr<Texture> CopyTexture(const std::shared_ptr<Texture>& textureDesc);

    bool LoadCursor(const char* assetName, const char* textureFile,
      int32_t centerX, int32_t centerY);

    void AddScene(const std::string& name, const std::shared_ptr<IScene>& scene);
    void PlayScene(std::shared_ptr<IScene>& scene);
    void PlayScene(const std::string& name);
    void End() { _quit = true; }

    void SetFullscreen(const bool fullscreen);
    void SetDisplayMode(const int32_t index);

    bool CreateSolidColorTexture(const std::string& name, const int32_t width,
      const int32_t height, const SDL_Color& color);
    Sprite* CreateSolidColorSprite(const uint32_t width, const uint32_t height,
      const SDL_Color& color, int32_t z);
    void DestroySprite(Sprite* sprite);

    void SetCursor(const std::string& name);

    bool RandomBool();
    int32_t RandomNumber(int32_t min, int32_t max);
    size_t RandomNumber(size_t min, size_t max);
    float RandomNumber(float min, float max);
    float RandomNumber();

    int32_t GetWidth() const { return _renderResolutionWidth; }
    int32_t GetHeight() const { return _renderResolutionHeight; }

    int32_t GetHalfWidth() const { return GetWidth() / 2; }
    int32_t GetHalfHeight() const { return GetHeight() / 2; }

    const Sprite* GetHoveredSprite() const { return _hoveredSprite; }
    SDL_Renderer* GetRenderer() { return _renderer; }
    bool IsFullscreen() const { return _fullscreen; }

    const int32_t GetCurrentDisplayMode() const { return _currentMode; }
    const std::vector<DisplayModeInfo>& GetDisplayModes() const { return _displayModes; }

    template<typename Class, typename CreationStruct>
    std::enable_if_t<std::is_base_of_v<ITextObject, Class>, std::add_pointer_t<Class>> Create(const CreationStruct& params)
    {
      const auto& scene = params.persistant ? _persistentScene : _currentScene;
      auto result = _textObjects[scene].emplace_back(std::make_unique<Class>(params)).get();
      result->Preload(_renderer);
      _renderables[scene].push_back(make_renderable<ITextObject>(result));
      SortRenderables(scene);
      return static_cast<std::add_pointer_t<Class>>(result);
    }

    template<typename Class, typename CreationStruct>
    std::enable_if_t<std::is_base_of_v<ICompositeObject, Class>, std::add_pointer_t<Class>> Create(const CreationStruct& params)
    {
      const auto& scene = params.persistant ? _persistentScene : _currentScene;
      auto& result = _compositeObjects[scene].emplace_back(std::make_unique<Class>(params));
      return static_cast<std::add_pointer_t<Class>>(result.get());
    }

    template<typename Class, typename CreationStruct>
    std::enable_if_t<std::is_base_of_v<Sprite, Class>, std::add_pointer_t<Class>> Create(const CreationStruct& params)
    {
      const auto& scene = params.persistant ? _persistentScene : _currentScene;
      auto result = _sprites[scene].emplace_back(std::make_unique<Class>(params)).get();
      _renderables[scene].push_back(make_renderable<Sprite>(result));
      SortRenderables(scene);
      return static_cast<std::add_pointer_t<Class>>(result);
    }

  private:
    bool LoadAssets(const Style& style);

    uint32_t GetPixel(SDL_Surface* surface, int32_t x, int32_t y);
    void GetBoundingBoxAndHitArray(SDL_Surface* surface,
      SDL_Rect& boundingBox, std::vector<bool>& hitArray, bool hitsRequired);

    void CollectDisplayModes(const Style& style);
    void Update();
    void UpdateTextObject(std::unique_ptr<LD45::ITextObject> & textObject);
    void SetHoveredSprite(Sprite* sprite);
    std::string HashSolidColorTexture(const uint32_t width, const uint32_t height,
      const SDL_Color& color);

    void SortRenderables(const std::shared_ptr<IScene>& scene);

    SDL_Window* _window;
    SDL_Renderer* _renderer;
    uint32_t _nativeTextureFormats;
    SDL_Texture* _nativeRenderBuffer;

    std::shared_ptr<IScene> _currentScene;
    std::shared_ptr<IScene> _persistentScene;
    std::unordered_map<std::string, std::shared_ptr<IScene>> _scenes;

    std::unordered_map<std::shared_ptr<IScene>,std::vector<std::unique_ptr<ITextObject>>> _textObjects;
    std::unordered_map<std::shared_ptr<IScene>, std::vector<std::unique_ptr<Sprite>>> _sprites;
    std::unordered_map<std::shared_ptr<IScene>,
      std::vector<std::unique_ptr<ICompositeObject>>> _compositeObjects;

    std::unordered_map<std::shared_ptr<IScene>, std::vector<Renderable>> _renderables;

    std::unordered_map<std::string, FontDescription> _fonts;

    std::unordered_map<std::string, std::shared_ptr<Texture>> _textures;
    std::vector<std::shared_ptr<Texture>> _textureCopies;

    std::unordered_map<std::string, CursorDescription> _cursors;

    std::vector<Sprite*> _possibleSprites;
    Sprite* _hoveredSprite;

    SDL_Color _clearColor;

    FTC* _fpsText;
    float _fpsTimer;
    int _fpsCount;

    std::mt19937 _re;

    bool _quit;

    int32_t _renderResolutionWidth;
    int32_t _renderResolutionHeight;

    SDL_Rect _scaledBufferRect;
    SDL_Rect _windowBufferRect;

    bool _fullscreen;
    bool _fullscreenChangedWanted;

    int32_t _currentMode;
    std::vector<DisplayModeInfo> _displayModes;
  };

  extern Game GGame;
}
