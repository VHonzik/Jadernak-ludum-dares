#include "BuildingIcon.h"

#include "Game.h"
#include "Sprite.h"
#include "FTC.h"
#include "EngineStyles.h"
#include "Input.h"
#include "ResourceBar.h"

namespace LD43
{
  BuildingIcon::BuildingIcon(const BuildingIconParams& params)
    : _hovered(false)
    , _woodCost(params.woodCost)
    , _crystalCost(params.crystalCost)
    , _canAfford(true)
    , _building(false)
  {
    if (ResourceBar::Get())
    {
      _canAfford = ResourceBar::Get()->CanAfford(_woodCost, _crystalCost);
    }

    SpriteParams spriteParams;
    spriteParams.persistant = params.persistant;
    spriteParams.texture = nullptr;
    spriteParams.z = params.z;
    spriteParams.textureName = params.iconName;

    _icon = GGame.Create<Sprite>(spriteParams);
    _icon->SetWidth(60);
    _icon->SetHeight(70);
    _icon->Show(_canAfford);

    _iconDisabled = GGame.Create<Sprite>(spriteParams);
    _iconDisabled->SetWidth(60);
    _iconDisabled->SetHeight(70);
    _iconDisabled->Tint(kDarkGreyColor);
    _iconDisabled->Show(!_canAfford);

    FTCParams ftcParams;
    ftcParams.persistant = params.persistant;
    ftcParams.font = nullptr;
    ftcParams.fontName = kVeraFontBold;
    ftcParams.fontSize = 16;
    ftcParams.format = "# (#)";
    ftcParams.defaultColor = k50GreyColor;

    _title = GGame.Create<FTC>(ftcParams);
    _title->SetHorizontalAlign(kHorizontalAlignment_Left);
    _title->SetVerticalAlign(kVerticalAlignment_Top);
    _title->SetStringValue(0, params.title);
    _title->SetStringValue(1, GInput.GetKeyName(GSettings.Get<int>(params.keybind)));
    _title->Show(false);

    TextParams textParams = kVera1650Grey;
    textParams.persistant = params.persistant;
    textParams.text = params.description;
    _description = GGame.Create<Text>(textParams);
    _description->SetHorizontalAlign(kHorizontalAlignment_Left);
    _description->SetVerticalAlign(kVerticalAlignment_Top);

    spriteParams.z = params.z;
    spriteParams.textureName = "woodResource";
    _woodCostIcon = GGame.Create<Sprite>(spriteParams);
    _woodCostIcon->SetWidth(11);
    _woodCostIcon->SetHeight(21);

    ftcParams.format = "#";
    ftcParams.fontName = kVera1650Grey.fontName;
    ftcParams.fontSize = kVera1650Grey.fontSize;
    _woodCostText = GGame.Create<FTC>(ftcParams);
    _woodCostText->SetHorizontalAlign(kHorizontalAlignment_Left);
    _woodCostText->SetVerticalAlign(kVerticalAlignment_Center);
    _woodCostText->SetIntValue(0, _woodCost);

    spriteParams.textureName = "crystalResource";
    _crystalCostIcon = GGame.Create<Sprite>(spriteParams);
    _crystalCostIcon->SetWidth(19);
    _crystalCostIcon->SetHeight(18);

    _crystalCostText = GGame.Create<FTC>(ftcParams);
    _crystalCostText->SetHorizontalAlign(kHorizontalAlignment_Left);
    _crystalCostText->SetVerticalAlign(kVerticalAlignment_Center);
    _crystalCostText->SetIntValue(0, _crystalCost);

    spriteParams.textureName = "selectWhite";
    _selectorHover = GGame.Create<Sprite>(spriteParams);
    _selectorHover->SetWidth(60);
    _selectorHover->SetHeight(70);
    _selectorHover->Show(false);
    _selectorHover->Tint(kGreenColor);

    _offsetX = params.index * (5 + _icon->GetWidth());
  }

  void BuildingIcon::SetPosition(int32_t x, int32_t y)
  {
    _icon->SetPosition(x + 10 + _offsetX, y + 10);
    _selectorHover->SetPosition(x + 10 + _offsetX, y + 10);
    _iconDisabled->SetPosition(x + 10 + _offsetX, y + 10);
    _title->SetPositon(x + 10, _icon->GetY() + _icon->GetHeight() + 10);
    _description->SetPosition(x + 10, _icon->GetY() + _icon->GetHeight() + 40);
    _woodCostIcon->SetPosition(_title->GetX() + _title->Width + 10,
      _title->GetY());
    _woodCostText->SetPositon(_woodCostIcon->GetX() + _woodCostIcon->GetWidth() + 10, _woodCostIcon->GetCenterY());
    _crystalCostIcon->SetPosition(_woodCostText->GetX() + _woodCostText->Width + 10, _title->GetY());
    _crystalCostText->SetPositon(_crystalCostIcon->GetX() + _crystalCostIcon->GetWidth() + 10,
      _crystalCostIcon->GetCenterY());
  }

  void BuildingIcon::Show(bool shown)
  {
    _shown = shown;
    _icon->Show(shown && _canAfford);
    _iconDisabled->Show(shown && !_canAfford);
    _selectorHover->Show(shown && (_hovered || _building));
    _title->Show(shown && (_hovered || _building));
    _description->Show(shown && (_hovered || _building));
    _woodCostIcon->Show(shown && (_hovered || _building));
    _woodCostText->Show(shown && (_hovered || _building));
    _crystalCostIcon->Show(shown && (_hovered || _building) && _crystalCost > 0);
    _crystalCostText->Show(shown && (_hovered || _building) && _crystalCost > 0);
  }

  void BuildingIcon::Update()
  {
    bool changed = false;
    bool hovered = GGame.GetHoveredSprite() == _icon;
    if (_hovered != hovered)
    {
      _hovered = hovered;
      changed = true;
    }

    if (ResourceBar::Get())
    {
      bool canAfford = ResourceBar::Get()->CanAfford(_woodCost, _crystalCost);
      if (_canAfford != canAfford)
      {
        _canAfford = canAfford;
        changed = true;
      }
    }

    if (changed)
    {
      Show(_shown);
    }
  }

  void BuildingIcon::Building(bool building)
  {
    if (_building != building)
    {
      _building = building;
      Show(_shown);
    }
  }
}