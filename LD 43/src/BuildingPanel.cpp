#include "BuildingPanel.h"

#include "Sprite.h"
#include "BoxSprite.h"
#include "EngineStyles.h"
#include "Game.h"
#include "BuildingIcon.h"
#include "Input.h"

namespace LD43
{
  BuildingPanel::BuildingPanel(const BuildingPanelParams& params)
    : _building(false)
    , _cursorVisible(false)
    , _buildingIndex(-1)
  {
    SpriteParams spriteParams;
    spriteParams.persistant = false;
    spriteParams.texture = nullptr;
    spriteParams.z = 10;
    spriteParams.textureName = "selectWhite";

    _greySelector = GGame.Create<Sprite>(spriteParams);
    _greySelector->Tint(kLightGreyColor);
    _greySelector->SetCenterPosition(GGame.GetHalfWidth(), GGame.GetHalfHeight());

    _redSelector = GGame.Create<Sprite>(spriteParams);
    _redSelector->Tint(kRedColor);
    _redSelector->SetCenterPosition(GGame.GetHalfWidth(), GGame.GetHalfHeight());
    _redSelector->Show(false);

    _greenSelector = GGame.Create<Sprite>(spriteParams);
    _greenSelector->Tint(kGreenColor);
    _greenSelector->SetCenterPosition(GGame.GetHalfWidth(), GGame.GetHalfHeight());
    _greenSelector->Show(false);

    BoxSpriteParams boxParams = kOptionsGreyPanel;
    boxParams.z = params.z;
    boxParams.width = 400;
    boxParams.height = 150;

    _panel = GGame.Create<BoxSprite>(boxParams);
    _panel->SetPosition(GGame.GetWidth() - _panel->GetWidth(),
      GGame.GetHeight());

    _icons.resize(2);
    _previewIcons.resize(2);

    BuildingIconParams iconParams;
    iconParams.persistant = false;
    iconParams.iconName = "sawMill";
    iconParams.title = "Farm";
    iconParams.description = "Produced organic material.";
    iconParams.keybind = kSettingsIDs_keyBind1;
    iconParams.woodCost = 100;
    iconParams.crystalCost = 0;
    iconParams.z = params.z + 1;
    iconParams.index = 0;

    _icons[0] = GGame.Create<BuildingIcon>(iconParams);
    _icons[0]->SetPosition(_panel->GetX(), _panel->GetY());

    iconParams.iconName = "livingQuarters";
    iconParams.title = "Living quarters";
    iconParams.description = "Provides population for your base.";
    iconParams.keybind = kSettingsIDs_keyBind2;
    iconParams.woodCost = 150;
    iconParams.crystalCost = 0;
    iconParams.index = 1;

    _icons[1] = GGame.Create<BuildingIcon>(iconParams);
    _icons[1]->SetPosition(_panel->GetX(), _panel->GetY());

    spriteParams.z = 15;
    spriteParams.textureName = "sawMill";
    _previewIcons[0] = GGame.Create<Sprite>(spriteParams);
    _previewIcons[0]->SetAlpha(0.5f);
    _previewIcons[0]->Show(false);

    spriteParams.textureName = "livingQuarters";
    _previewIcons[1] = GGame.Create<Sprite>(spriteParams);
    _previewIcons[1]->SetAlpha(0.5f);
    _previewIcons[1]->Show(false);
  }

  void BuildingPanel::Update()
  {
    for (int32_t i = 0; i < _icons.size(); i++)
    {
      if (!_building && _icons[i]->Hovered() && _icons[i]->CanAfford() && GInput.MouseButtonPressed(SDL_BUTTON_LEFT))
      {
        _building = true;
        _buildingIndex = i;
        _icons[i]->Building(true);
        Show(_shown);
      }
    }
  }

  void BuildingPanel::Show(bool shown)
  {
    _shown = shown;
    _panel->Show(shown);

    for (int32_t i = 0; i < _icons.size(); i++)
    {
      _icons[i]->Show(shown && (!_building || _buildingIndex == i));
    }

    _greySelector->Show(shown && _cursorVisible && !_building);
    _redSelector->Show(shown && _cursorVisible && _building && _cursorOccupied);
    _greenSelector->Show(shown && _cursorVisible && _building && !_cursorOccupied);

    for (int32_t i = 0; i < _previewIcons.size(); i++)
    {
      auto& icon = _previewIcons[i];
      icon->Show(shown && _cursorVisible && _building && i == _buildingIndex);
    }
  }

  int32_t BuildingPanel::GetX() const
  {
    return _panel->GetX();
  }

  int32_t BuildingPanel::GetY() const
  {
    return _panel->GetY();
  }

  int32_t BuildingPanel::GetWidth() const
  {
    return _panel->GetWidth();
  }

  int32_t BuildingPanel::GetHeight() const
  {
    return _panel->GetHeight();
  }

  void BuildingPanel::UpdateCursorPos(const std::pair<int32_t, int32_t>& center, bool visible, bool occupied)
  {
    if (visible)
    {
      _greySelector->SetCenterPosition(center.first, center.second);
      _greenSelector->SetCenterPosition(center.first, center.second);
      _redSelector->SetCenterPosition(center.first, center.second);
      for (auto& icon : _previewIcons)
      {
        icon->SetCenterPosition(center.first, center.second);
      }
    }

    if (_cursorVisible != visible)
    {
      _cursorVisible = visible;
      Show(_shown);
    }

    if (_cursorOccupied != occupied)
    {
      _cursorOccupied = occupied;
      Show(_shown);
    }
  }

  void BuildingPanel::SetPosition(int32_t x, int32_t y)
  {
    _panel->SetPosition(x, y);

    for (auto& icon : _icons)
    {
      icon->SetPosition(_panel->GetX(), _panel->GetY());
    }
  }
}