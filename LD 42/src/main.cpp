#include <SDL.h>
#include <SDL_image.h>

#include "Game.h"

#include "DesktopScene.h"
#include "ZoomScene.h"
#include "SteamScene.h"
#include "LostScene.h"
#include "WonScene.h"

namespace
{
  const int32_t kResolutionWidth = 1024;
  const int32_t kResolutionHeight = 768;
}

int32_t main(int32_t argc, char* argv[])
{
  CherEngine::Game game;

  std::vector<std::pair<std::string, std::string>> textures =
  {
    { "emptydesktop", "desktop.png" },
    { "thispc", "thispc.png" },
    { "thispc_hover" ,"thispc_hover.png" },
    { "thispcwindow", "thispcwindow.png" },
    { "steam_hover" ,"steam_selected.png" },
    { "steam", "steam.png" },
    { "cursorsprite", "cursor.png" },
    { "spacebar_bg", "spacebar_3.png" },
    { "spacebar_blue", "spacebar_2.png" },
    { "spacebar_red", "spacebar_1.png" },
    { "youlost", "youlost.png" },
    { "youwon", "youwon.png" },
    { "gaben", "gaben.png" },
    { "steamsale", "steamsale.png" },
   };

  std::vector<std::string> iconsNames = { "xcom", "subnautica", "bayonetta", "civ", "divinity", "orwell", "oxygen" };

  for (const auto& icon : iconsNames)
  {
    for (int32_t i = 0; i < 8; i++)
    {
      char name[128];
      char filename[128];
      std::snprintf(name, 128, "%s%d", icon.c_str(), i);
      std::snprintf(filename, 128, "%s_%d.png", icon.c_str(), i + 1);
      textures.push_back({ name, filename });
    }
  }

  if (game.Initialize("LD42", kResolutionWidth, kResolutionHeight)
    && game.LoadTextures(textures)
    && game.LoadCursors({ { "cursor", "cursor.png" } })
    && game.LoadSounds({ {"recycle", "recycle.wav"}, {"logoff", "logoff.wav" },{ "hardwarefail", "hardwarefail.wav" },
      { "aaa", "aaa.wav" }, {"saletime", "saletime.wav" },{ "chord", "chord.wav" },{ "chordlong", "chordlong.wav" },
      {"summersale", "summersale.wav"} })
    )
  {
    game.SetCursor("cursor");
    game.AddScene<LD42::DesktopScene>("desktopScene");
    game.AddScene<LD42::ZoomScene>("zoomScene");
    game.AddScene<LD42::SteamScene>("steamScene");
    game.AddScene<LD42::LostScene>("lostScene");
    game.AddScene<LD42::WonScene>("wonScene");
    game.StartScene("desktopScene");
    game.Start();
  }

  game.CleanUp();

  return 0;
}