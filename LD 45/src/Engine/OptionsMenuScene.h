#pragma once

#include "IScene.h"

#include "AlignedGroup.h"

#include <vector>
#include <array>

namespace LD45
{
  class Slider;
  class Button;
  class Text;
  class Checkbox;
  class Dropdown;

  class OptionsMenuScene : public IScene
  {
  public:
    OptionsMenuScene();
    void Start() override;
    void Update() override;

    std::shared_ptr<IScene> DoReset() override { return std::make_shared<OptionsMenuScene>(); }
  private:
    void AdjustSections();

    std::array<Button*, 3> _sectionTitles;
    int32_t _activeSection;
    int32_t _activeSectionY;
    int32_t _passiveSectionY;

    Button* _backButton;

    AlignedGroup _slidersRow;
    Slider* _musicVolume;
    Text* _musicVolumeDescription;
    Slider* _soundVolume;
    Text* _soundVolumeDescription;

    AlignedGroup _keybindRow1;
    AlignedGroup _keybindRow2;
    std::vector<Text*> _keybindingDescription;
    std::vector<Button*> _keybindingButtons;
    bool _pickingKeybind;
    Button* _pickingKeybindSlot;
    int32_t _pickingKeybindIndex;

    Text* _fullScreenDescription;
    Checkbox* _fullScreenCheckbox;
    Dropdown* _resolutionsDropdown;
  };
}
