#include "Upgrade.h"

#include "EngineStyles.h"
#include "FTC.h"
#include "Game.h"
#include "Input.h"
#include "Sprite.h"
#include "Tooltip.h"

namespace
{
  const auto kCostOffset = -26;
}

namespace LD45
{
  Upgrade::Upgrade(const UpgradeParams& params)
    : _skill(params.skill)
    , _amount(params.count)
    , _cost(params.cost)
    , _pressed(false)
    , _released(false)
    , _down(false)
    , _showingAfford(false)
    , _buying(false)
  {
    SpriteParams spriteParams;
    spriteParams.persistant = params.persistant;
    spriteParams.textureName = "upgrade";
    spriteParams.texture = nullptr;
    spriteParams.z = params.z-1;

    _background = GGame.Create<Sprite>(spriteParams);

    spriteParams.z = params.z+1;
    spriteParams.textureName = kDiceSkillTextures.find(params.skill)->second;

    _skillSprite = GGame.Create<Sprite>(spriteParams);

    spriteParams.z = params.z;
    spriteParams.textureName = "upgradeGGlow";

    _canBuyGlow = GGame.Create<Sprite>(spriteParams);

    spriteParams.textureName = "upgradeYGlow";
    _buyingGlow = GGame.Create<Sprite>(spriteParams);

    FTCParams ftcParams;
    ftcParams.persistant = false;
    ftcParams.font = nullptr;
    ftcParams.fontName = kVeraFont;
    ftcParams.fontSize = 32;
    ftcParams.format = "#";
    ftcParams.defaultColor = kGoldColor;
    ftcParams.z = params.z;

    _costText = GGame.Create<FTC>(ftcParams);
    _costText->SetIntValue(0, _cost);

    ftcParams.format = "#";
    ftcParams.defaultColor = kLightGreyColor;
    ftcParams.fontSize = 16;
    _amountText = GGame.Create<FTC>(ftcParams);
    _amountText->SetStringValue(0, std::to_string(_amount));
    _amountText->SetVerticalAlign(kVerticalAlignment_Top);
    _amountText->SetHorizontalAlign(kHorizontalAlignment_Left);

    SetPosition(0, 0);
    Show(true);
  }

  void Upgrade::Show(bool shown)
  {
    ICompositeObject::Show(shown);

    _background->Show(shown && _amount > 0);
    _skillSprite->Show(shown && _amount > 0);
    _costText->Show(shown && _amount > 0);
    _amountText->Show(shown && _amount > 0);
    _canBuyGlow->Show(shown && _showingAfford);
    _buyingGlow->Show(shown && _buying);
  }

  void Upgrade::Update()
  {
    if (_pressed)
    {
      _pressed = false;
    }

    if (_released)
    {
      _released = false;
    }

    const auto hoveredSprite = GGame.GetHoveredSprite();
    _hovered = hoveredSprite == _background || hoveredSprite == _skillSprite;
    if (!_down && _hovered && GInput.MouseButtonPressed(SDL_BUTTON_LEFT))
    {
      _down = true;
      _pressed = true;
    }
    else if (!_pressed && _down && !_hovered)
    {
      _down = false;
      _pressed = false;
    }
    else if (_down && !GInput.MouseButtonDown(SDL_BUTTON_LEFT))
    {
      _down = false;
      _pressed = false;
      _released = true;
    }

    if (_activeTooltip != nullptr)
    {
      _activeTooltip->Show(false);
      _activeTooltip = nullptr;
    }

    if (IsShown() && _tooltips && _tooltips->count(_skill) > 0 && _hovered)
    {
      _activeTooltip = (*_tooltips)[_skill];
      _activeTooltip->Show(true);
      const auto centerX = GetCenterX();
      const auto y = GetY();
      const auto toolTipX = centerX > GGame.GetHalfWidth() ? centerX + 5 - _activeTooltip->GetWidth() : centerX + 5;
      const auto toolTipY = y > GGame.GetHalfHeight() ? y - (10 + _activeTooltip->GetHeight()) : y + GetHeight() + 10;
      _activeTooltip->SetPosition(toolTipX, toolTipY);
    }
  }

  int32_t Upgrade::GetWidth() const { return _background->GetWidth(); }
  int32_t Upgrade::GetHeight() const { return _background->GetHeight(); }
  int32_t Upgrade::GetX() const { return _background->GetX(); }
  int32_t Upgrade::GetY() const { return _background->GetY(); }
  int32_t Upgrade::GetCenterX() const { return _background->GetCenterX(); }
  int32_t Upgrade::GetCenterY() const { return _background->GetCenterY(); }

  void Upgrade::SetPosition(uint32_t x, uint32_t y)
  {
    _background->SetPosition(x, y);
    _skillSprite->SetPosition(x, y);
    _canBuyGlow->SetPosition(x, y);
    _buyingGlow->SetPosition(x, y);
    _costText->SetPositon(GetX() + kCostOffset, GetY() + kCostOffset);
    _amountText->SetPositon(GetX() + GetWidth(), GetY() + GetHeight());
  }

  void Upgrade::SetCenterPosition(uint32_t x, uint32_t y)
  {
    _background->SetCenterPosition(x, y);
    _skillSprite->SetCenterPosition(x, y);
    _canBuyGlow->SetCenterPosition(x, y);
    _buyingGlow->SetCenterPosition(x, y);
    _costText->SetPositon(GetX() + kCostOffset, GetY() + kCostOffset);
    _amountText->SetPositon(GetX() + GetWidth(), GetY() + GetHeight());
  }

  void Upgrade::HideAfford()
  {
    _showingAfford = false;
    _canBuyGlow->Show(false);
  }

  void Upgrade::UpdateCanAfford(const int32_t gold)
  {
    bool canAfford = CanAfford(gold);
    _showingAfford = canAfford;
    //_costText->SetValueColor(0, gold >= _cost ? kGreenColor : kRedColor);
    _canBuyGlow->Show(canAfford);
  }

  void Upgrade::CancelBuy()
  {
    _buying = false;
    _buyingGlow->Show(false);
  }

  void Upgrade::ConfirmBuy()
  {
    _buying = false;
    _buyingGlow->Show(false);

    _amount--;
    _amountText->SetStringValue(0, std::to_string(_amount));

    if (_amount == 0)
    {
      Show(false);
    }
  }

  void Upgrade::EnterBuy()
  {
    _buying = true;
    _buyingGlow->Show(true);
  }
}