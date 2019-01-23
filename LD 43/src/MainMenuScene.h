#pragma once

#include "IScene.h"

namespace LD43
{
  class Button;

  class MainMenuScene : public IScene
  {
  public:
    void Start() override;
    void Update() override;
  private:
    void ApplyDisplayModeFromSettings();

    Button* _playButton;
    Button* _optionsButton;
    Button* _quitButton;
  };
}
