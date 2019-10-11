#include <SDL.h>

#include <memory>

#include "Game.h"
#include "Audio.h"
#include "Settings.h"
#include "EngineSettings.h"
#include "EngineStyles.h"

#include "MainMenuScene.h"
#include "OptionsMenuScene.h"

#include "LD45Styles.h"
#include "PlayScene.h"
#include "AI.h"

using namespace LD45;

int32_t main(int32_t argc, char* argv[])
{
  auto& game = GGame;
  auto& settings = GSettings;

  settings.RegisterAll(std::begin(kEngineSettings), std::end(kEngineSettings));

  if (game.Initialize(kEngineStyle))
  {
    game.AddScene(kMainMenuScene, std::make_shared<MainMenuScene>());
    game.AddScene(kOptionsMenuScene, std::make_shared<OptionsMenuScene>());
    game.AddScene(kPlayScene, std::make_shared<PlayScene>());
    game.PlayScene(kMainMenuScene);
    //game.PlayScene(kPlayScene);
    game.Start();
  }

  game.CleanUp();

  return 0;
}