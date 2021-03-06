#include "Game.h"

#include "Time.h"
#include "IScene.h"
#include "Input.h"
#include "Sprite.h"
#include "Camera.h"
#include "Audio.h"
#include "Settings.h"
#include "Style.h"

#include "Text.h"
#include "FTC.h"
#include "Button.h"
#include "BoxSprite.h"
#include "Slider.h"
#include "Checkbox.h"
#include "Dropdown.h"

#include <SDL_image.h>

#include <string>
#include <unordered_map>
#include <sstream>

namespace
{
  const char kDefaultTextureName[] = "default";
  const int32_t kDefaultTextureSize = 100;
}

namespace LD43
{
  Game GGame;

  Game::Game()
    : _re(std::random_device()())
    , _quit(false)
    , _hoveredSprite(nullptr)
    , _fpsCount(0)
    , _fpsTimer(0.0f)
    , _fullscreenChangedWanted(false)
    , _fullscreen(false)
    , _currentMode(-1)
  {
  }

  bool Game::Initialize(const Style& style)
  {
    _renderResolutionWidth  = style.nativeResolutionWidth;
    _renderResolutionHeight = style.nativeResolutionHeight;
    _scaledBufferRect = { 0, 0, _renderResolutionWidth, _renderResolutionHeight };
    _windowBufferRect = { 0, 0, _renderResolutionWidth, _renderResolutionHeight };

    if (SDL_Init(SDL_INIT_VIDEO | SDL_INIT_AUDIO) != 0)
    {
      return false;
    }

    _window = SDL_CreateWindow(style.windowName.c_str(), 50, 50,
      _windowBufferRect.w, _windowBufferRect.h, SDL_WINDOW_SHOWN);

    if (_window == nullptr)
    {
      return false;
    }

    _renderer = SDL_CreateRenderer(_window, -1, SDL_RENDERER_ACCELERATED | SDL_RENDERER_PRESENTVSYNC);
    if (_renderer == nullptr)
    {
      return false;
    }

    SDL_RendererInfo info;
    if (SDL_GetRendererInfo(_renderer, &info) != 0
      || info.num_texture_formats == 0)
    {
      return false;
    }

    _nativeTextureFormats = info.texture_formats[0];

    SDL_SetHint(SDL_HINT_RENDER_SCALE_QUALITY, "best");
    _nativeRenderBuffer = SDL_CreateTexture(_renderer, _nativeTextureFormats,
      SDL_TEXTUREACCESS_TARGET, _renderResolutionWidth, _renderResolutionHeight);

    if (_nativeRenderBuffer == nullptr)
    {
      return false;
    }

    CollectDisplayModes(style);

    if (TTF_Init() == -1)
    {
      return false;
    }

    if ((IMG_Init(IMG_INIT_PNG) & IMG_INIT_PNG) == 0)
    {
      return false;
    }

    {
      auto imageSurface = SDL_CreateRGBSurface(0, 100, 100, 32, 0, 0, 0, 0);
      SDL_FillRect(imageSurface, NULL, SDL_MapRGB(imageSurface->format, 255, 0, 220));
      const auto format = imageSurface->format->format;
      auto imageTexture = SDL_CreateTextureFromSurface(_renderer, imageSurface);
      SDL_FreeSurface(imageSurface);

      Texture texture = {
        imageTexture,
        kDefaultTextureSize,
        kDefaultTextureSize,
        { 0, 0, kDefaultTextureSize, kDefaultTextureSize },
        {},
        kDefaultTextureName,
        format,
        false
      };

      _textures[kDefaultTextureName] = std::make_shared<Texture>(texture);
    }

    if (!GSettings.Init())
    {
      return false;
    }

    GCamera.SetResolution(_renderResolutionWidth, _renderResolutionHeight);

    if (!GAudio.Init())
    {
      return false;
    }

    LoadAssets(style);

    _persistentScene = std::make_shared<IScene>();
    _currentScene = _persistentScene;

    _fpsText = Create<FTC>(style.fpsStyle);
    _fpsText->SetPositon(5, 5);
    _fpsText->SetIntValue(0, 0);

    _clearColor = style.backgroundColor;

    return true;
  }

  bool Game::LoadAssets(const Style& style)
  {
    bool result = true;
    for (const auto& texture : style.textures)
    {
      result &= LoadTexture(std::get<0>(texture).c_str(), std::get<1>(texture).c_str(), std::get<2>(texture));
    }

    for (const auto& font : style.fonts)
    {
      result &= LoadFont(style, font.first.c_str(), font.second.c_str());
    }

    for (const auto& sound : style.sounds)
    {
      result &= GAudio.LoadSound(sound.first.c_str(), sound.second.c_str());
    }

    for (const auto& cursor : style.cursors)
    {
      result &= LoadCursor(std::get<0>(cursor).c_str(), std::get<1>(cursor).c_str(),
        std::get<2>(cursor), std::get<3>(cursor));
    }

    return result;
  }

  bool Game::LoadFont(const Style& style, const char* assetName, const char* fontFile)
  {
    for (const auto size : style.fontSizes)
    {
      const auto key = assetName + std::to_string(size);
      const auto ttfFont = TTF_OpenFont(fontFile, size);

      if (ttfFont == nullptr)
      {
        return false;
      }

      _fonts[key] = { assetName, ttfFont, size };
    }

    return true;
  }

  TTF_Font* Game::FindFont(const std::string& fontName, const uint32_t size) const
  {
    const auto key = fontName + std::to_string(size);
    const auto found = _fonts.find(key);
    if (found == _fonts.end())
    {
      return nullptr;
    }
    else
    {
      return found->second.ttfFont;
    }
  }

  uint32_t Game::GetPixel(SDL_Surface* surface, int32_t x, int32_t y)
  {
    const auto bytesPerPixel = surface->format->BytesPerPixel;
    const auto pixels = reinterpret_cast<uint8_t*>(surface->pixels);
    const auto pixel = pixels + y * surface->pitch + x * bytesPerPixel;

    switch (bytesPerPixel)
    {
    case 1:
      return *pixel;
    case 2:
      return *(reinterpret_cast<uint16_t*>(pixel));
    case 4:
      return *(reinterpret_cast<uint32_t*>(pixel));
    }

    return 0;
  }

  void Game::GetBoundingBoxAndHitArray(SDL_Surface* surface,
    SDL_Rect& boundingBox, std::vector<bool>& hitArray, bool hitsRequired)
  {
    if (hitsRequired)
    {
      const auto lockedRequired = SDL_MUSTLOCK(surface);
      if (lockedRequired)
      {
        SDL_LockSurface(surface);
      }

      int32_t minX = surface->w, maxX = 0, minY = surface->h, maxY = 0;
      hitArray.resize(surface->w * surface->h, false);

      for (int32_t x = 0; x < surface->w; x++)
      {
        for (int32_t y = 0; y < surface->h; y++)
        {
          const auto pixel = GetPixel(surface, x, y);
          uint8_t r, g, b, a;
          SDL_GetRGBA(pixel, surface->format, &r, &g, &b, &a);

          const auto hit = a > 0;
          hitArray[x + y * surface->w] = hit;

          if (hit)
          {
            minX = std::min(x, minX);
            maxX = std::max(x, maxX);
            minY = std::min(y, minY);
            maxY = std::max(y, maxY);
          }
        }
      }

      boundingBox = { minX, minY, maxX - minX, maxY - minY };

      if (lockedRequired)
      {
        SDL_UnlockSurface(surface);
      }
    }
    else
    {
      boundingBox = { 0, 0, surface->w, surface->h };
    }
  }

  bool Game::LoadTexture(const char* assetName, const char* textureFile, bool hitsRequired)
  {
    auto imageSurface = IMG_Load(textureFile);

    if (imageSurface == nullptr)
    {
      return false;
    }

    int32_t width = imageSurface->w, height = imageSurface->h;
    const auto format = imageSurface->format->format;

    SDL_Rect boundingBox = {};
    std::vector<bool> hitArray;
    GetBoundingBoxAndHitArray(imageSurface, boundingBox, hitArray, hitsRequired);

    auto imageTexture = SDL_CreateTextureFromSurface(_renderer, imageSurface);

    SDL_FreeSurface(imageSurface);

    if (imageTexture == nullptr)
    {
      return false;
    }

    _textures[assetName] = std::make_shared<Texture>(
      imageTexture, width, height, boundingBox, hitArray, assetName, format, false
     );

    return true;
  }

  std::shared_ptr<Texture> Game::CopyTexture(const std::shared_ptr<Texture>& textureDesc)
  {
    std::shared_ptr<Texture> result = std::make_shared<Texture>(*textureDesc);
    result->isCopy = true;
    result->texture = SDL_CreateTexture(_renderer, textureDesc->format, SDL_TEXTUREACCESS_TARGET,
      textureDesc->width, textureDesc->height);

    SDL_BlendMode mode;
    SDL_GetTextureBlendMode(textureDesc->texture, &mode);
    SDL_SetTextureBlendMode(result->texture, mode);

    SDL_SetRenderTarget(_renderer, result->texture);
    SDL_RenderCopy(_renderer, textureDesc->texture, nullptr, nullptr);
    SDL_SetRenderTarget(_renderer, nullptr);

    _textureCopies.push_back(result);

    return result;
  }

  bool Game::CreateSolidColorTexture(const std::string& name, const int32_t width,
    const int32_t height, const SDL_Color& color)
  {
    auto imageSurface = SDL_CreateRGBSurface(0, width, height, 32, 0, 0, 0, 0);
    SDL_FillRect(imageSurface, NULL, SDL_MapRGB(imageSurface->format, color.r, color.g, color.b));

    if (imageSurface == nullptr)
    {
      return false;
    }

    const auto format = imageSurface->format->format;

    auto imageTexture = SDL_CreateTextureFromSurface(_renderer, imageSurface);
    SDL_FreeSurface(imageSurface);

    if (imageTexture == nullptr)
    {
      return false;
    }

    _textures[name] = std::make_shared<Texture>(
      imageTexture,
      width,
      height,
      SDL_Rect{0, 0, width, height},
      std::vector<bool>{},
      name,
      format,
      false
    );

    return true;
  }

  void Game::SortSprites()
  {
    auto& sprites = _sprites[_currentScene];

    std::sort(std::begin(sprites), std::end(sprites),
      [](const std::unique_ptr<Sprite>& a, const std::unique_ptr<Sprite>& b)
    {
      return a->GetZ() < b->GetZ();
    });
  }

  std::string Game::HashSolidColorTexture(const uint32_t width, const uint32_t height,
    const SDL_Color& color)
  {
    char buffer[64];
    sprintf_s(buffer, "%ux%u-%u,%u,%u-HealSolidTexture", width, height,
    color.r, color.g, color.b);
    return buffer;
  }

  Sprite* Game::CreateSolidColorSprite(const uint32_t width, const uint32_t height,
    const SDL_Color& color, int32_t z)
  {
    if (!_currentScene)
    {
      return nullptr;
    }

    auto& sprites = _sprites[_currentScene];

    const auto& key = HashSolidColorTexture(width, height, color);

    auto textureFound = _textures.find(key);
    if (textureFound == std::end(_textures))
    {
      if (CreateSolidColorTexture(key, width, height, color))
      {
        textureFound = _textures.find(key);
      }
      else
      {
        return nullptr;
      }
    }

    auto& textureDesc = textureFound->second;

    SpriteParams params;
    params.persistant = false;
    params.textureName = "";
    params.texture = textureDesc;
    params.z = z;

    auto result = sprites.emplace_back(std::make_unique<Sprite>(params)).get();

    std::sort(std::begin(sprites), std::end(sprites),
      [](const std::unique_ptr<Sprite>& a, const std::unique_ptr<Sprite>& b)
    {
      return a->GetZ() < b->GetZ();
    });

    return result;
  }

  void Game::DestroySprite(Sprite* sprite)
  {
    if (!_currentScene)
    {
      return;
    }

    auto& sprites = _sprites[_currentScene];

    if (sprite->GetTextureDescription()->isCopy)
    {
      auto elementToRemove = std::remove_if(std::begin(_textureCopies), std::end(_textureCopies),
        [&](const std::shared_ptr<Texture>& element)
      {
        return element->texture == sprite->GetTextureDescription()->texture;
      });

      for (auto iter = elementToRemove; iter != std::end(_textureCopies); ++iter)
      {
        SDL_DestroyTexture((*iter)->texture);
      }

      _textureCopies.erase(elementToRemove, std::end(_textureCopies));
    }

    auto elementsToRemove = std::remove_if(std::begin(sprites), std::end(sprites),
      [&](const std::unique_ptr<Sprite>& element)
    {
      return element.get() == sprite;
    });

    for (auto iter = elementsToRemove; iter != std::end(sprites); ++iter)
    {
      (*iter)->Clean();
    }

    sprites.erase(elementsToRemove, std::end(sprites));
  }

  void Game::AddScene(const std::string& name, const std::shared_ptr<IScene>& scene)
  {
    auto& sceneResult = _scenes[name];
    sceneResult = scene;
  }

  void Game::PlayScene(std::shared_ptr<IScene>& scene)
  {
    _currentScene = scene;
    if (!_currentScene->GetInitialized())
    {
      _currentScene->SetInitialized(true);
      _currentScene->Start();
    }
  }

  void Game::PlayScene(const std::string& name)
  {
    _possibleSprites.clear();
    _hoveredSprite = nullptr;

    auto found = _scenes.find(name);
    if (found != _scenes.end())
    {
      PlayScene(found->second);
    }
  }

  void Game::CleanUp()
  {
    for (auto& scene : _scenes)
    {
      for (auto& textObject : _textObjects[scene.second])
      {
        textObject->Clean();
      }
      _textObjects[scene.second].clear();

      for (auto& sprite : _sprites[scene.second])
      {
        sprite->Clean();
      }
      _sprites[scene.second].clear();
    }

    for (auto& texture : _textures)
    {
      SDL_DestroyTexture(texture.second->texture);
    }

    for (auto& texture : _textureCopies)
    {
      SDL_DestroyTexture(texture->texture);
    }
    _textureCopies.clear();

    for (const auto& font : _fonts)
    {
      TTF_CloseFont(font.second.ttfFont);
    }
    _fonts.clear();

    for (const auto& cursor : _cursors)
    {
      SDL_FreeCursor(cursor.second.cursor);
      SDL_FreeSurface(cursor.second.surface);
    }
    _cursors.clear();

    if (_window != nullptr)
    {
      SDL_DestroyWindow(_window);
    }

    if (_renderer != nullptr)
    {
      SDL_DestroyRenderer(_renderer);
    }

    GAudio.CleanUp();

    TTF_Quit();
    SDL_Quit();
  }

  void Game::SetHoveredSprite(Sprite* sprite)
  {
    if (_hoveredSprite != sprite)
    {
      if (_currentScene)
      {
        _currentScene->SpriteHovered(_hoveredSprite, sprite);
      }
    }

    _hoveredSprite = sprite;
  }

  void Game::Update()
  {
    if (_fullscreenChangedWanted)
    {
      _fullscreenChangedWanted = false;
      _fullscreen = !_fullscreen;
      SDL_SetWindowFullscreen(_window, _fullscreen ? SDL_WINDOW_FULLSCREEN : 0);
      return;
    }

    GTime.Tick();

    _fpsCount++;
    _fpsTimer += GTime.deltaTime;
    if (_fpsTimer >= 1.0f)
    {
      _fpsTimer -= 1.0f;
      _fpsText->SetIntValue(0, _fpsCount);
      _fpsCount = 0;
    }

    GInput.Update();

    SDL_Event event;
    while (SDL_PollEvent(&event))
    {
      if (event.type == SDL_QUIT)
      {
        _quit = true;
      }
      else
      {
        GInput.ProcessMessage(event);
      }
    }

    if (GInput.KeyPressed(SDLK_ESCAPE))
    {
      End();
    }

    SDL_SetRenderDrawColor(_renderer, _clearColor.r, _clearColor.g, _clearColor.b, 255);
    SDL_RenderClear(_renderer);

    if (_currentScene)
    {
      _currentScene->Update();
    }

    GCamera.Update();

    SDL_SetRenderTarget(_renderer, _nativeRenderBuffer);
    SDL_RenderClear(_renderer);

    _possibleSprites.clear();
    for (auto& sprite : _sprites[_currentScene])
    {
      if (sprite->RequiresPreload())
      {
        sprite->Preload(_renderer);
      }

      if (!sprite->RequiresPreload())
      {
        sprite->Update();
        sprite->Render(_renderer);
        if (sprite->IsShown() && GCamera.IsMouseInside(sprite.get(), false)
          && GCamera.IsMouseInside(sprite.get(), true))
        {
          _possibleSprites.push_back(sprite.get());
        }
      }
    }

    if (_possibleSprites.size() > 0)
    {
      std::sort(std::begin(_possibleSprites), std::end(_possibleSprites),
        [](const Sprite* a, const Sprite* b) { return a->GetZ() > b->GetZ(); });

      SetHoveredSprite(_possibleSprites[0]);
    }
    else
    {
      SetHoveredSprite(nullptr);
    }

    for (auto& textObject : _textObjects[_currentScene])
    {
      UpdateTextObject(textObject);
    }

    if (_currentScene != _persistentScene)
    {
      for (auto& textObject : _textObjects[_persistentScene])
      {
        UpdateTextObject(textObject);
      }
    }

    for (auto& compositeObject : _compositeObjects[_currentScene])
    {
      compositeObject->Update();
    }

    SDL_SetRenderTarget(_renderer, nullptr);

    SDL_Rect finalRect = {
      (_windowBufferRect.w - _scaledBufferRect.w) / 2,
      (_windowBufferRect.h - _scaledBufferRect.h) / 2,
      _scaledBufferRect.w,
      _scaledBufferRect.h
    };
    SDL_RenderCopy(_renderer, _nativeRenderBuffer, nullptr,
      &finalRect);

    SDL_RenderPresent(_renderer);

    GInput.AfterUpdate();
  }

  void Game::UpdateTextObject(std::unique_ptr<LD43::ITextObject>& textObject)
  {
    if (textObject->RequiresPreload())
    {
      textObject->Preload(_renderer);
    }

    if (!textObject->RequiresPreload())
    {
      textObject->Update();
      textObject->Render(_renderer);
    }
  }

  void Game::Start()
  {
    GTime.Start();

    while (!_quit)
    {
      Update();
    }

    GSettings.WriteSettings();
  }

  bool Game::RandomBool()
  {
    return RandomNumber(0, 1) == 0;
  }

  int32_t Game::RandomNumber(int32_t min, int32_t max)
  {
    std::uniform_int_distribution<int32_t> uniform_dist(min, max);
    return uniform_dist(_re);
  }

  float Game::RandomNumber()
  {
    std::uniform_real_distribution<float> uniform_dist(0.0f, 1.0f);
    return uniform_dist(_re);
  }

  float Game::RandomNumber(float min, float max)
  {
    std::uniform_real_distribution<float> uniform_dist(min, max);
    return uniform_dist(_re);
  }

  void Game::SetFullscreen(const bool fullscreen)
  {
    if (_fullscreen != fullscreen)
    {
      _fullscreenChangedWanted = true;
    }
  }

  void Game::CollectDisplayModes(const Style& style)
  {
    std::unordered_map<DisplayModeInfoKey, DisplayModeInfo> displayModes;

    const auto numDisplays = SDL_GetNumVideoDisplays();
    for (int32_t i = 0; i < numDisplays; i++)
    {
      std::string displayName = SDL_GetDisplayName(i);
      const auto numModes = SDL_GetNumDisplayModes(i);
      for (int32_t j = 0; j < numModes; j++)
      {
        SDL_DisplayMode mode;
        if (SDL_GetDisplayMode(i, j, &mode) == 0)
        {
          DisplayModeInfo info = { mode.w, mode.h, 1 << i };
          const auto key = std::hash<DisplayModeInfo>{}(info);
          auto found = displayModes.find(key);
          if (found != std::end(displayModes))
          {
            found->second.displaysSupported |= info.displaysSupported;
          }
          else
          {
            displayModes[key] = info;
          }
        }
      }
    }

    // Remove all: a) not supported by all displays, b) smaller then 800x600
    const auto mask = ~(~int32_t{} << numDisplays);
    for (auto it = displayModes.begin(); it != displayModes.end();)
    {
      const auto& info = it->second;
      if ((info.displaysSupported & mask) != mask || info.width < 800 || info.height < 600)
      {
        it = displayModes.erase(it);
      }
      else
      {
        it++;
      }
    }

    int32_t stepWidth = _renderResolutionWidth / style.maxResolutionFraction;
    int32_t stepHeight = _renderResolutionHeight / style.maxResolutionFraction;

    int32_t lowestResolutionWidth = _renderResolutionWidth;
    int32_t lowestResolutionHeight = _renderResolutionHeight;
    while (lowestResolutionWidth > 800 || lowestResolutionHeight > 600)
    {
      lowestResolutionWidth -= stepWidth;
      lowestResolutionHeight -= stepHeight;
    }

    std::stringstream stream;

    // Find out the last X/Nth of render resolution smaller than the mode resolution
    for (auto& it : displayModes)
    {
      auto& info = it.second;
      int32_t closestResolutionWidth = lowestResolutionWidth;
      int32_t closestResolutionHeight = lowestResolutionHeight;
      while (closestResolutionWidth <= info.width && closestResolutionHeight <= info.height)
      {
        closestResolutionWidth += stepWidth;
        closestResolutionHeight += stepHeight;
      }

      closestResolutionWidth -= stepWidth;
      closestResolutionHeight -= stepHeight;

      info.scaledRect = { 0, 0, closestResolutionWidth, closestResolutionHeight };
      info.windowRect = { 0, 0, info.width, info.height };
      info.native = info.width == _renderResolutionWidth && info.height == _renderResolutionHeight;
      info.distanceFromNative = std::abs(info.width - _renderResolutionWidth) +
        std::abs(info.height - _renderResolutionHeight);

      stream.str(std::string());
      stream << info.width << "x" << info.height << (info.native ? " (Native)" : "");
      info.name = stream.str();

      _displayModes.push_back(info);
    }

    std::sort(std::begin(_displayModes), std::end(_displayModes),
      [](const DisplayModeInfo& a, const DisplayModeInfo& b)
    {
      return a.width < b.width || (a.width == b.width && a.height < b.height);
    });

    bool found = false;
    int closestOne = 0;
    for (int32_t i = 0; i < _displayModes.size(); i++)
    {
      const auto& mode = _displayModes[i];
      if (mode.native)
      {
        found = true;
        _currentMode = i;
        break;
      }
      else if (mode.distanceFromNative < _displayModes[closestOne].distanceFromNative)
      {
        closestOne = i;
      }
    }

    if (!found)
    {
      _currentMode = closestOne;
      SetDisplayMode(_currentMode);
    }
  }

  void Game::SetDisplayMode(const int32_t index)
  {
    if (index != _currentMode)
    {
      const auto& displayMode = _displayModes[index];
      _scaledBufferRect = displayMode.scaledRect;
      _windowBufferRect = displayMode.windowRect;
      SDL_SetWindowSize(_window, _windowBufferRect.w, _windowBufferRect.h);
      GCamera.SetDisplayMode(_scaledBufferRect, _windowBufferRect);
      _currentMode = index;
    }
  }

  std::shared_ptr<Texture> Game::FindTexture(const std::string& textureName) const
  {
    auto textureFound = _textures.find(textureName);
    if (textureFound == std::end(_textures))
    {
      textureFound = _textures.find(kDefaultTextureName);
    }

    return textureFound->second;
  }

  bool Game::LoadCursor(const char* assetName, const char* textureFile, int32_t centerX, int32_t centerY)
  {
    auto imageSurface = IMG_Load(textureFile);

    if (imageSurface == nullptr)
    {
      return false;
    }

    auto sdlCursor = SDL_CreateColorCursor(imageSurface, centerX, centerY);

    if (sdlCursor == nullptr)
    {
      return false;
    }

    _cursors[assetName] = { sdlCursor, imageSurface };

    return true;
  }

  void Game::SetCursor(const std::string& name)
  {
    auto cursor = _cursors.find(name);
    if (cursor != _cursors.end())
    {
      SDL_SetCursor(cursor->second.cursor);
    }
  }
}