#include "Game.h"

#include "Camera.h"
#include "Time.h"
#include "Input.h"
#include "Sprite.h"
#include "IScene.h"

#include <SDL_image.h>
#include <SDL_ttf.h>

namespace CherEngine
{
  Game::Game()
    : _re(std::random_device()())
  {
  }

  bool Game::Initialize(const char* windowName, int32_t resolutionWidth, int32_t resolutionHeight)
  {
    if (SDL_Init(SDL_INIT_VIDEO) != 0)
    {
      return false;
    }

    _window = SDL_CreateWindow(windowName, 50, 50, resolutionWidth, resolutionHeight, SDL_WINDOW_SHOWN);

    if (_window == nullptr)
    {
      return false;
    }

    _renderer = SDL_CreateRenderer(_window, -1, SDL_RENDERER_ACCELERATED | SDL_RENDERER_PRESENTVSYNC);
    if (_renderer == nullptr)
    {
      return false;
    }

    if ((IMG_Init(IMG_INIT_PNG) & IMG_INIT_PNG) == 0)
    {
      return false;
    }

    if (Mix_OpenAudio(22050, MIX_DEFAULT_FORMAT, 2, 4096) == -1)
    {
      return false;
    }

    GCamera.SetResolution(resolutionWidth, resolutionHeight);

    return true;
  }

  void Game::Start()
  {
    GTime.Start();

    auto quit = false;
    while (!quit)
    {
      GTime.Tick();

      SDL_Event event;
      while (SDL_PollEvent(&event))
      {
        if (event.type == SDL_QUIT)
        {
          quit = true;
        }
        else
        {
          GInput.ProcessMessage(event);
        }
      }

      if (_currentScene != nullptr)
      {
        _currentScene->Update();
      }

      for (auto& sprite : _sprites)
      {
        sprite->Update();
      }

      SDL_SetRenderDrawColor(_renderer, 0, 0, 0, 255);
      SDL_RenderClear(_renderer);
      for (auto& sprite : _sprites)
      {
        sprite->Render(_renderer);
      }

      SDL_RenderPresent(_renderer);

      GInput.Update();
    }
  }

  void Game::CleanUp()
  {
    for (auto& scene : _scenes)
    {
      scene.second->Unload();
    }

    for (auto& texture : _textures)
    {
      SDL_DestroyTexture(texture.second.texture);
    }

    for (auto& cursor : _cursors)
    {
      SDL_FreeCursor(cursor.second.cursor);
      SDL_FreeSurface(cursor.second.surface);
    }

    for (auto& font : _fonts)
    {
      FC_FreeFont(font.second);
    }

    for (auto& sound : _sounds)
    {
      Mix_FreeChunk(sound.second);
    }

    if (_window != nullptr)
    {
      SDL_DestroyWindow(_window);
    }

    if (_renderer != nullptr)
    {
      SDL_DestroyRenderer(_renderer);
    }

    _scenes.clear();
    _textures.clear();
    _cursors.clear();
    _fonts.clear();
    _sounds.clear();

    SDL_Quit();
  }

  void Game::StartScene(const char* sceneName)
  {
    if (_currentScene != nullptr)
    {
      _currentScene->DisableAllSprites();
    }

    auto find = _scenes.find(sceneName);
    if (find != _scenes.end())
    {
      auto& scene = find->second;
      if (scene->WasLoaded())
      {
        scene->DoRestart();
      }
      else
      {
        scene->Load();
      }
      _currentScene = scene.get();
    }
  }

  bool Game::LoadTextures(const std::vector<std::pair<std::string, std::string>>& textures)
  {
    bool result = true;
    for (const auto& texture : textures)
    {
      auto imageSurface = IMG_Load((kAssetsFolder+texture.second).c_str());

      if (imageSurface == nullptr)
      {
        result = false;
        continue;
      }

      int32_t width = imageSurface->w, height = imageSurface->h;

      auto imageTexture = SDL_CreateTextureFromSurface(_renderer, imageSurface);

      SDL_FreeSurface(imageSurface);

      if (imageTexture == nullptr)
      {
        result = false;
        continue;
      }

      _textures[texture.first.c_str()] = { imageTexture, width, height };
    }

    {
      auto imageSurface = SDL_CreateRGBSurface(0, 100, 100, 32, 0, 0, 0, 0);
      SDL_FillRect(imageSurface, NULL, SDL_MapRGB(imageSurface->format, 255, 0, 220));
      auto imageTexture = SDL_CreateTextureFromSurface(_renderer, imageSurface);
      SDL_FreeSurface(imageSurface);
      _textures[kDefaultTextureName] = { imageTexture, 100, 100 };
    }

    return result;
  }

  bool Game::LoadFonts(const std::vector<FontDescription>& fonts)
  {
    bool result = true;
    for (const auto& font : fonts)
    {
      FC_Font* fcfont = FC_CreateFont();

      if (fcfont == nullptr)
      {
        result = false;
        continue;
      }

      if (!FC_LoadFont(fcfont, _renderer, font.file.c_str(), font.size, font.color, font.style))
      {
        result = false;
        continue;
      }

      _fonts[font.name.c_str()] = fcfont;
    }

    return true;
  }

  bool Game::LoadCursors(const std::vector<std::pair<std::string, std::string>>& cursors)
  {
    bool result = true;
    for (const auto& cursor : cursors)
    {
      auto imageSurface = IMG_Load((kAssetsFolder + cursor.second).c_str());

      if (imageSurface == nullptr)
      {
        result = false;
        continue;
      }

      auto sdlCursor = SDL_CreateColorCursor(imageSurface, 0, 0);

      _cursors[cursor.first.c_str()] = { sdlCursor, imageSurface };
    }

    return result;
  }

  bool Game::LoadSounds(const std::vector<std::pair<std::string, std::string>>& sounds)
  {
    bool result = true;
    for (const auto& sound : sounds)
    {
      auto soundWav = Mix_LoadWAV((kAssetsFolder + sound.second).c_str());

      if (soundWav == nullptr)
      {
        result = false;
        continue;
      }

      _sounds[sound.first.c_str()] = soundWav;
    }

    return result;
  }

  void Game::SetCursor(const char* cursorName)
  {
    SDL_ShowCursor(SDL_DISABLE);
    auto cursor = _cursors.find(cursorName);
    if (cursor != _cursors.end())
    {
      SDL_SetCursor(cursor->second.cursor);
      SDL_ShowCursor(SDL_ENABLE);
    }
  }

  void Game::PlaySound(const char* soundName)
  {
    auto sound = _sounds.find(soundName);
    if (sound != _sounds.end())
    {
      Mix_PlayChannel(-1, sound->second, 0);
    }
  }

  Sprite* Game::CreateSprite(const char* texture_name)
  {
    auto texture = _textures.find(texture_name);
    if (texture == _textures.end())
    {
      texture = _textures.find(kDefaultTextureName);
    }

    auto& sprite = _sprites.emplace_back(std::make_unique<Sprite>(texture->second.texture, texture->second.width, texture->second.height));
    sprite->SetAsSimpleSprite();
    return sprite.get();
  }

  Sprite* Game::CreateBoxSprite(const char* texture_name, int32_t width, int32_t height, int32_t gridSize)
  {
    auto texture = _textures.find(texture_name);
    if (texture == _textures.end())
    {
      texture = _textures.find(kDefaultTextureName);
    }

    auto& sprite = _sprites.emplace_back(std::make_unique<Sprite>(texture->second.texture, width, height, gridSize));
    sprite->SetAsBoxSprite();

    return sprite.get();
  }

  Sprite* Game::CreateTextBox(const char* font_name, const char* text, int32_t width, int32_t height)
  {
    auto font = _fonts.find(font_name);

    auto& sprite = _sprites.emplace_back(std::make_unique<Sprite>(font->second, width, height, text));
    sprite->SetAsTextBox();

    return sprite.get();
  }

  Sprite* Game::CreateTooltip(const char* texture_name, const char* font_name, const char* text, int32_t width, int32_t height,
    int32_t gridSize, Padding padding)
  {
    auto texture = _textures.find(texture_name);
    if (texture == _textures.end())
    {
      texture = _textures.find(kDefaultTextureName);
    }

    auto& box = _sprites.emplace_back(std::make_unique<Sprite>(texture->second.texture, width, height, gridSize));
    box->SetAsBoxSprite();

    auto font = _fonts.find(font_name);

    auto& textSprite = _sprites.emplace_back(std::make_unique<Sprite>(font->second, width - (padding.left + padding.right), height - (padding.top + padding.bottom), text));
    textSprite->SetAsTextBox();
    textSprite->SetCenterPosition(padding.left, padding.top);

    box->AttachTo(textSprite.get());

    return textSprite.get();
  }

  void Game::HideAllSprites()
  {
    for (auto& sprite : _sprites)
    {
      sprite->SetVisible(false);
    }
  }

  int32_t Game::RandomNumber(int32_t min, int32_t max)
  {
    std::uniform_int_distribution<int> uniform_dist(min, max);
    return uniform_dist(_re);
  }

  int32_t Game::RandomPoisson()
  {
    std::poisson_distribution<int32_t> poisson_dist(1);
    return poisson_dist(_re);
  }
}