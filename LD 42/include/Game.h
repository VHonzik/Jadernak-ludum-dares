#pragma once

#include "Sprite.h"

#include <SDL.h>
#include <SDL_FontCache.h>
#include <SDL_mixer.h>
#include <array>
#include <vector>
#include <unordered_map>
#include <random>

namespace CherEngine
{
  class IScene;

  struct Texture
  {
    SDL_Texture* texture;
    int32_t width;
    int32_t height;
  };

  struct FontDescription
  {
    std::string name;
    std::string file;
    uint32_t size;
    SDL_Color color;
    int32_t style;
  };

  struct Padding
  {
    int32_t left;
    int32_t right;
    int32_t top;
    int32_t bottom;
  };

  const Padding kDefaultPadding = { 5,5,8,5 };

  struct Cursor
  {
    SDL_Cursor* cursor;
    SDL_Surface* surface;
  };

  const char kDefaultTextureName[] = "default";

  const std::string kAssetsFolder = "assets\\";

  const int32_t kMaxSprites = 1000;

  class Game
  {
  public:
    Game();
    bool Initialize(const char* windowName, int32_t resolutionWidth, int32_t resolutionHeight);
    void Start();
    void CleanUp();

    bool LoadTextures(const std::vector<std::pair<std::string, std::string>>& textures);
    bool LoadFonts(const std::vector<FontDescription>& fonts);
    bool LoadCursors(const std::vector<std::pair<std::string, std::string>>& cursors);
    bool LoadSounds(const std::vector<std::pair<std::string, std::string>>& sounds);

    Sprite* CreateSprite(const char* texture_name);
    Sprite* CreateBoxSprite(const char* texture_name, int32_t width, int32_t height, int32_t gridSize);
    Sprite* CreateTextBox(const char* font_name, const char* text, int32_t width, int32_t height);
    Sprite* CreateTooltip(const char* texture_name, const char* font_name, const char* text,
      int32_t width, int32_t height, int32_t gridSize, Padding padding = kDefaultPadding);

    template <class Scene>
    void AddScene(const char* sceneName)
    {
      _scenes[sceneName] = std::make_unique<Scene>(*this);
    }

    void StartScene(const char* sceneName);
    void SetCursor(const char* cursorName);
    void PlaySound(const char* soundName);

    void HideAllSprites();

    int32_t RandomNumber(int32_t min, int32_t max);
    int32_t RandomPoisson();

  private:
    SDL_Window* _window;
    SDL_Renderer* _renderer;

    IScene* _currentScene;
    std::unordered_map<std::string, std::unique_ptr<IScene>> _scenes;

    std::vector<std::unique_ptr<Sprite>> _sprites;
    std::unordered_map<std::string, Texture> _textures;
    std::unordered_map<std::string, FC_Font*> _fonts;
    std::unordered_map<std::string, Cursor> _cursors;
    std::unordered_map<std::string, Mix_Chunk*> _sounds;

    std::mt19937 _re;
  };
}