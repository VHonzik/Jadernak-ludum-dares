#include "DiceBuy.h"

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
  DiceBuy::DiceBuy(const DiceBuyParams& params)
    : _highlightCenterX(params.highlightCenterX)
    , _highlightCenterY(params.highlightCenterY)
    , _amount(params.count)
    , _showingAfford(false)
    , _diceBought(false)
    , _buying(false)
  {
    _skills = params.skills;

    SpriteParams spriteParams;
    spriteParams.persistant = params.persistant;
    spriteParams.textureName = "dice";
    spriteParams.texture = nullptr;
    spriteParams.z = params.z - 1;

    _backgroundSprite = GGame.Create<Sprite>(spriteParams);

    spriteParams.z = params.z;
    spriteParams.textureName = "diceGGlow";

    _canBuyGlow = GGame.Create<Sprite>(spriteParams);

    spriteParams.textureName = "diceYGlow";
    _buyingGlow = GGame.Create<Sprite>(spriteParams);
    _buyingGlow->Show(false);

    for (size_t i = 0; i < kDiceSides; i++)
    {
      spriteParams.z = params.z - 1;
      spriteParams.textureName = "dice";
      _highlightBackgroundSprites[i] = GGame.Create<Sprite>(spriteParams);

      spriteParams.z = params.z;
      spriteParams.textureName = kDiceSkillTextures.find(params.skills[i])->second;

      if (i == 0) _skillSprite = GGame.Create<Sprite>(spriteParams);

      _highlightSkillSprites[i] = GGame.Create<Sprite>(spriteParams);
    }

    FTCParams ftcParams;
    ftcParams.persistant = false;
    ftcParams.font = nullptr;
    ftcParams.fontName = kVeraFont;
    ftcParams.fontSize = 32;
    ftcParams.format = "#";
    ftcParams.defaultColor = kLightGreyColor;
    ftcParams.fontSize = 16;

    _amountText = GGame.Create<FTC>(ftcParams);
    _amountText->SetStringValue(0, std::to_string(_amount));
    _amountText->SetVerticalAlign(kVerticalAlignment_Top);
    _amountText->SetHorizontalAlign(kHorizontalAlignment_Left);

    ftcParams.fontSize = 32;
    ftcParams.format = "#";
    ftcParams.defaultColor = kGoldColor;

    _costText = GGame.Create<FTC>(ftcParams);
    _costText->SetStringValue(0, "0");

    Show(true);

    SetPosition(0, 0);
  }

  void DiceBuy::Show(bool shown)
  {
    ICompositeObject::Show(shown);

    _backgroundSprite->Show(IsShown() && _amount > 0);
    _skillSprite->Show(IsShown() && _amount > 0);
    _amountText->Show(shown && _amount > 0);
    _costText->Show(shown && _amount > 0);
    _canBuyGlow->Show(shown && _showingAfford && _amount > 0 && !_diceBought && !_buying);
    _buyingGlow->Show(shown && _buying && _amount > 0 && !_diceBought);

    for (size_t i = 0; i < kDiceSides; i++)
    {
      _highlightBackgroundSprites[i]->Show(IsShown() && _hovered && _amount > 0);
      _highlightSkillSprites[i]->Show(IsShown() && _hovered && _amount > 0);
    }
  }

  bool DiceBuy::IsHovered(const size_t side) const
  {
    const auto hoveredSprite = GGame.GetHoveredSprite();
    bool result = false;

    if (side == 0)
    {
      result = hoveredSprite == _backgroundSprite
        || hoveredSprite == _skillSprite;
    }

    result = result
      || hoveredSprite == _highlightBackgroundSprites[side]
      || hoveredSprite == _highlightSkillSprites[side];

    return result;
  }

  void DiceBuy::TooltipUpdate()
  {
    if (_activeTooltip != nullptr)
    {
      _activeTooltip->Show(false);
      _activeTooltip = nullptr;
    }

    if (_tooltips && IsShown())
    {
      for (size_t i = 0; i < kDiceSides; i++)
      {
        if (_tooltips->count(_skills[i]) > 0 && IsHovered(i))
        {
          _activeTooltip = (*_tooltips)[_skills[i]];
          _activeTooltip->Show(true);
          const auto centerX = i == 0 ? GetCenterX() : _highlightBackgroundSprites[i]->GetCenterX();
          const auto y = i == 0 ? GetY() : _highlightBackgroundSprites[i]->GetY();
          const auto toolTipX = centerX > GGame.GetHalfWidth() ? centerX + 5 - _activeTooltip->GetWidth() : centerX + 5;
          const auto toolTipY = y > GGame.GetHalfHeight() ? y - (10 + _activeTooltip->GetHeight()) : y + GetHeight() + 10;
          _activeTooltip->SetPosition(toolTipX, toolTipY);
        }
      }
    }
  }


  void DiceBuy::Update()
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
    const auto newHovered =
      hoveredSprite == _backgroundSprite
      || hoveredSprite == _skillSprite
      || hoveredSprite == _canBuyGlow;

    if (newHovered != _hovered)
    {
      _hovered = newHovered;
      Show(IsShown());
    }

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

    TooltipUpdate();
  }

  int32_t DiceBuy::GetWidth() const { return _backgroundSprite->GetWidth(); }
  int32_t DiceBuy::GetHeight() const { return _backgroundSprite->GetHeight(); }
  int32_t DiceBuy::GetX() const { return _backgroundSprite->GetX(); }
  int32_t DiceBuy::GetY() const { return _backgroundSprite->GetY(); }
  int32_t DiceBuy::GetCenterX() const { return _backgroundSprite->GetCenterX(); }
  int32_t DiceBuy::GetCenterY() const { return _backgroundSprite->GetCenterY(); }

  void DiceBuy::SetPosition(uint32_t x, uint32_t y)
  {
    _backgroundSprite->SetPosition(x, y);
    _canBuyGlow->SetPosition(x, y);
    _buyingGlow->SetPosition(x, y);

    _highlightBackgroundSprites[0]->SetCenterPosition(_highlightCenterX - GetWidth() * 3 / 2, _highlightCenterY - GetHeight() * 3 / 4);
    _highlightBackgroundSprites[1]->SetCenterPosition(_highlightCenterX, _highlightCenterY - GetHeight() * 3 / 4);
    _highlightBackgroundSprites[2]->SetCenterPosition(_highlightCenterX + GetWidth() * 3 / 2, _highlightCenterY - GetHeight() * 3 / 4);
    _highlightBackgroundSprites[3]->SetCenterPosition(_highlightCenterX - GetWidth() * 3 / 2, _highlightCenterY + GetHeight() * 3 / 4);
    _highlightBackgroundSprites[4]->SetCenterPosition(_highlightCenterX, _highlightCenterY + GetHeight() * 3 / 4);
    _highlightBackgroundSprites[5]->SetCenterPosition(_highlightCenterX + GetWidth() * 3 / 2, _highlightCenterY + GetHeight() * 3 / 4);

    const auto centerX = _backgroundSprite->GetCenterX();
    const auto centerY = _backgroundSprite->GetCenterY();
    _skillSprite->SetCenterPosition(centerX, centerY);

    for (size_t i = 0; i < kDiceSides; i++)
    {
      _highlightSkillSprites[i]->SetCenterPosition(_highlightBackgroundSprites[i]->GetCenterX(), _highlightBackgroundSprites[i]->GetCenterY());
    }

    _costText->SetPositon(GetX() + kCostOffset, GetY() + kCostOffset);
    _amountText->SetPositon(GetX() + GetWidth(), GetY() + GetHeight());
  }

  void DiceBuy::SetCenterPosition(uint32_t x, uint32_t y)
  {
    _backgroundSprite->SetCenterPosition(x, y);
    SetPosition(_backgroundSprite->GetX(), _backgroundSprite->GetY());
    _amountText->SetPositon(GetX() + GetWidth(), GetY() + GetHeight());
    _costText->SetPositon(GetX() + kCostOffset, GetY() + kCostOffset);
    _canBuyGlow->SetCenterPosition(x, y);
    _buyingGlow->SetCenterPosition(x, y);
  }

  void DiceBuy::HideAfford()
  {
    _showingAfford = false;
    Show(IsShown());
  }

  void DiceBuy::UpdateCanAfford(const int32_t gold)
  {
    _showingAfford = true;
    Show(IsShown());
  }

  void DiceBuy::SetDiceBought(bool bought)
  {
    _diceBought = bought;
    _costText->SetValueColor(0, _diceBought ? kRedColor : kGoldColor);
    _costText->SetStringValue(0, _diceBought ? "X" : "0");
    Show(IsShown());
  }

  void DiceBuy::EnterBuy()
  {
    _buying = true;
    Show(IsShown());
  }

  void DiceBuy::ConfirmBuy()
  {
    _buying = false;
    _amount--;
    _amountText->SetStringValue(0, std::to_string(_amount));

    if (_amount == 0)
    {
      Show(false);
    }
  }
}