#pragma once

#include "IScene.h"

namespace LD45
{
  class Button;

  class MainMenuScene : public IScene
  {
  public:
    void Start() override;
    void Update() override;

    std::shared_ptr<IScene> DoReset() override { return std::make_shared<MainMenuScene>(); }

  private:
    void ApplyDisplayModeFromSettings();

    Button* _playButton;
    Button* _optionsButton;
    Button* _quitButton;
  };
}
