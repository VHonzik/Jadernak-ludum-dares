#include "OptionsMenuScene.h"

#include "Game.h"
#include "Input.h"
#include "Slider.h"
#include "Audio.h"
#include "Button.h"
#include "Settings.h"
#include "EngineSettings.h"
#include "Dropdown.h"
#include "EngineStyles.h"

#include <sstream>

namespace LD43
{
  OptionsMenuScene::OptionsMenuScene()
    : _pickingKeybindSlot(nullptr)
    , _pickingKeybind(false)
    , _activeSection(0)
    , _activeSectionY(222)
    , _passiveSectionY(224)
    , _keybindRow1(kGroupDirection_Vertical, kHorizontalAlignment_Right,
      kVerticalAlignment_Center, 10)
    , _keybindRow2(kGroupDirection_Vertical, kHorizontalAlignment_Left,
      kVerticalAlignment_Center, 10)
    , _slidersRow(kGroupDirection_Vertical, kHorizontalAlignment_Center,
      kVerticalAlignment_Center, 75)
  {
  }

  void OptionsMenuScene::Start()
  {
    auto box = GGame.Create<BoxSprite>(kOptionsGreyPanel);
    box->SetCenterPosition(GGame.GetHalfWidth(), GGame.GetHalfHeight());

    auto buttonParams = kOptionsSectionTitleButton;
    buttonParams.text = "Audio";

    _sectionTitles[0] = GGame.Create<Button>(buttonParams);
    _sectionTitles[0]->SetCenterPosition(GGame.GetHalfWidth() - 170,
      GGame.GetHalfHeight() - _activeSectionY);
    _sectionTitles[0]->Disable(true);

    buttonParams.text = "Video";
    _sectionTitles[1] = GGame.Create<Button>(buttonParams);
    _sectionTitles[1]->SetCenterPosition(GGame.GetHalfWidth(),
      GGame.GetHalfHeight() - _passiveSectionY);

    buttonParams.text = "Controls";
    _sectionTitles[2] = GGame.Create<Button>(buttonParams);
    _sectionTitles[2]->SetCenterPosition(GGame.GetHalfWidth() + 170,
      GGame.GetHalfHeight() - _passiveSectionY);

    auto sliderParams = kOptionsSlider;
    sliderParams.initialValue = GAudio.GetMusicVolume();
    _musicVolume = GGame.Create<Slider>(sliderParams);

    auto textParams = kVeraBold1650Grey;
    textParams.text = "Music volume:";
    _musicVolumeDescription = GGame.Create<Text>(textParams);
    _musicVolumeDescription->SetHorizontalAlign(kHorizontalAlignment_Right);
    _musicVolumeDescription->SetVerticalAlign(kVerticalAlignment_Center);

    sliderParams.initialValue = GAudio.GetSoundVolume();
    _soundVolume = GGame.Create<Slider>(sliderParams);

    textParams.text = "Sound volume: ";
    _soundVolumeDescription = GGame.Create<Text>(textParams);
    _soundVolumeDescription->SetHorizontalAlign(kHorizontalAlignment_Right);
    _soundVolumeDescription->SetVerticalAlign(kVerticalAlignment_Center);

    _slidersRow.SetPosition(GGame.GetHalfWidth() + 60, GGame.GetHalfHeight());
    _slidersRow.AddVA(_musicVolume, _soundVolume);

    _musicVolumeDescription->SetPosition(_musicVolume->GetX() - 20, _musicVolume->GetCenterY());
    _soundVolumeDescription->SetPosition(_soundVolume->GetX() - 20, _soundVolume->GetCenterY());

    buttonParams = kMainMenuBlueButton;
    buttonParams.width = 250;
    buttonParams.text = "BACK";
    _backButton = GGame.Create<Button>(buttonParams);
    _backButton->SetCenterPosition(GGame.GetHalfWidth(), GGame.GetHeight() - 100);

    std::ostringstream descrStr;

    buttonParams = kOptionsKeybindButton;
    textParams = kVeraBold1250Grey;
    for (int row = 0; row <= kSettingsIDs_keyBind12 - kSettingsIDs_keyBind1; row++)
    {
      const auto key = GInput.GetKeyName(GSettings.Get<int>(
        static_cast<SettingsIDs>(kSettingsIDs_keyBind1 + row)));

      buttonParams.text = key;
      auto button = GGame.Create<Button>(buttonParams);
      button->Show(false);
      _keybindingButtons.push_back(button);

      descrStr.str(std::string());
      descrStr << "Slot " << row+1;

      textParams.text = descrStr.str();
      auto description = GGame.Create<Text>(textParams);
      description->Show(false);
      description->SetHorizontalAlign(kHorizontalAlignment_Right);
      description->SetVerticalAlign(kVerticalAlignment_Center);
      _keybindingDescription.push_back(description);
    }

    const auto descripionsWidthHalf =
      _keybindingDescription[_keybindingDescription.size() - 1]->GetWidth() / 2;

    _keybindRow1.SetPosition(GGame.GetHalfWidth() - 60 + descripionsWidthHalf, GGame.GetHalfHeight());
    _keybindRow1.Add(std::begin(_keybindingButtons), std::next(std::begin(_keybindingButtons), 6));

    _keybindRow2.SetPosition(GGame.GetHalfWidth() + 60 + descripionsWidthHalf, GGame.GetHalfHeight());
    _keybindRow2.Add(std::next(std::begin(_keybindingButtons), 6), std::end(_keybindingButtons));

    for (size_t i = 0; i < 12; i++)
    {
      _keybindingDescription[i]->SetPosition(_keybindingButtons[i]->GetX() - 10,
        _keybindingButtons[i]->GetCenterY());
    }

    auto checkboxStyle = kBlueCheckbox;
    checkboxStyle.checked = GGame.IsFullscreen();
    _fullScreenCheckbox = GGame.Create<Checkbox>(checkboxStyle);
    _fullScreenCheckbox->Show(false);
    _fullScreenCheckbox->SetCenterPosition(GGame.GetHalfWidth(), GGame.GetHalfHeight() - 150);

    textParams = kVeraBold1650Grey;
    textParams.text = "Fullscreen:";
    _fullScreenDescription = GGame.Create<Text>(textParams);
    _fullScreenDescription->Show(false);
    _fullScreenDescription->SetHorizontalAlign(kHorizontalAlignment_Right);
    _fullScreenDescription->SetVerticalAlign(kVerticalAlignment_Center);
    _fullScreenDescription->SetPosition(_fullScreenCheckbox->GetX() - 20,
      _fullScreenCheckbox->GetCenterY());

    _resolutionsDropdown = GGame.Create<Dropdown>(kOptionsDropdown);

    std::vector<std::string> displayModeNames;
    const auto& modes = GGame.GetDisplayModes();

    for (int32_t m = 0; m < modes.size(); ++m)
    {
      displayModeNames.push_back(modes[m].name);
    }

    _resolutionsDropdown->AddEntries(std::begin(displayModeNames), std::end(displayModeNames));
    _resolutionsDropdown->SetIndex(GGame.GetCurrentDisplayMode());
    _resolutionsDropdown->SetCenterPosition(GGame.GetHalfWidth(), GGame.GetHalfHeight() -100);
    _resolutionsDropdown->Show(false);
  }

  void OptionsMenuScene::AdjustSections()
  {
    for (auto title : _sectionTitles)
    {
      title->SetCenterPosition(title->GetCenterX(), GGame.GetHalfHeight() - _passiveSectionY);
      title->Disable(false);
    }

    _sectionTitles[_activeSection]->SetCenterPosition(_sectionTitles[_activeSection]->GetCenterX(),
      GGame.GetHalfHeight() - _activeSectionY);
    _sectionTitles[_activeSection]->Disable(true);

    const auto audioVisible = _sectionTitles[0]->Disabled();
    _musicVolume->Show(audioVisible);
    _musicVolumeDescription->Show(audioVisible);
    _soundVolume->Show(audioVisible);
    _soundVolumeDescription->Show(audioVisible);

    const auto controlsVisible = _sectionTitles[2]->Disabled();
    for (size_t i = 0; i < _keybindingButtons.size(); i++)
    {
      _keybindingButtons[i]->Show(controlsVisible);
      _keybindingDescription[i]->Show(controlsVisible);
    }

    const auto videoVisible = _sectionTitles[1]->Disabled();
    _fullScreenDescription->Show(videoVisible);
    _fullScreenCheckbox->Show(videoVisible);
    _resolutionsDropdown->Show(videoVisible);
  }

  void OptionsMenuScene::Update()
  {
    if (_musicVolume->Changed())
    {
      GAudio.SetMusicVolume(_musicVolume->GetValue());
    }

    if (_soundVolume->Released())
    {
      GAudio.PlaySound("uiBeep");
    }

    if (_soundVolume->Changed())
    {
      GAudio.SetSoundVolume(_soundVolume->GetValue());
    }

    if (_backButton->Released())
    {
      GSettings.Set(kSettingsIDs_musicVolume, _musicVolume->GetValue());
      GSettings.Set(kSettingsIDs_soundVolume, _soundVolume->GetValue());
      GGame.PlayScene(kMainMenuScene);
    }

    for (int32_t i = 0; i < _sectionTitles.size(); i++)
    {
      auto title = _sectionTitles[i];
      if (title->Released())
      {
        _activeSection = i;
        AdjustSections();
        break;
      }
    }

    if (_pickingKeybind)
    {
      const auto key = GInput.FirstKeyPressed();
      if (key != SDLK_UNKNOWN)
      {
        _pickingKeybind = false;
        _pickingKeybindSlot->Disable(false);
        _pickingKeybindSlot->SetText(GInput.GetKeyName(key));
        GSettings.Set(static_cast<SettingsIDs>(kSettingsIDs_keyBind1 + _pickingKeybindIndex),
          key);
      }
    }
    else
    {
      for (int32_t i = 0; i < _keybindingButtons.size(); ++i)
      {
        const auto keybindButton = _keybindingButtons[i];
        if (keybindButton->Released())
        {
          _pickingKeybindSlot = keybindButton;
          _pickingKeybind = true;
          _pickingKeybindSlot->Disable(true);
          _pickingKeybindIndex = i;
        }
      }
    }

    if (_fullScreenCheckbox->Changed())
    {
      GGame.SetFullscreen(_fullScreenCheckbox->Checked());
      GSettings.Set(kSettingsIDs_fullScreen, _fullScreenCheckbox->Checked());
    }

    if (_resolutionsDropdown->Changed())
    {
      const auto& mode = GGame.GetDisplayModes()[_resolutionsDropdown->GetIndex()];

      GSettings.Set(kSettingsIDs_resolutionWidth, mode.width);
      GSettings.Set(kSettingsIDs_resolutionHeight, mode.height);
      GGame.SetDisplayMode(_resolutionsDropdown->GetIndex());
    }
  }
}
