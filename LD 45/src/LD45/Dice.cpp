#include "Dice.h"

#include "EngineStyles.h"
#include "EngineTime.h"
#include "Game.h"
#include "Input.h"
#include "Sprite.h"
#include "Tooltip.h"

#include <cassert>
#include <unordered_set>

namespace LD45
{
  Dice::Dice(const DiceParams& params)
    : _state(0)
    , _rollingTimer(0.0f)
    , _currentSkill(0)
    , _wantedSkill(0)
    , _highlightCenterX(params.highlightCenterX)
    , _highlightCenterY(params.highlightCenterY)
    , _highlightedSideClicked({false, 0})
    , _tooltips(nullptr)
    , _activeTooltip(nullptr)
  {
    _skills = params.initialSkills;

    SpriteParams spriteParams;
    spriteParams.persistant = params.persistant;
    spriteParams.textureName = "dice";
    spriteParams.texture = nullptr;
    spriteParams.z = params.z-1;

    _backgroundSprite = GGame.Create<Sprite>(spriteParams);

    spriteParams.z = params.z+1;
    for (size_t i = 0; i < kDiceSides; i++)
    {
      spriteParams.z = params.z-1;
      spriteParams.textureName = "dice";
      _highlightBackgroundSprites[i] = GGame.Create<Sprite>(spriteParams);

      spriteParams.z = params.z;
      spriteParams.textureName = kDiceSkillTextures.find(params.initialSkills[i])->second;
      _skillSprites[i] = GGame.Create<Sprite>(spriteParams);

      _highlightSkillSprites[i] = GGame.Create<Sprite>(spriteParams);

      spriteParams.z = params.z;
      spriteParams.textureName = "diceGGlow";

      _highlightGreenGlowSprites[i] = GGame.Create<Sprite>(spriteParams);
    }

    spriteParams.textureName = "diceGGlow";
    _greenGlowSprite = GGame.Create<Sprite>(spriteParams);

    spriteParams.textureName = "diceYGlow";
    _yellowGlowSprite = GGame.Create<Sprite>(spriteParams);

    spriteParams.textureName = "diceBGlow";
    _blueGlowSprite = GGame.Create<Sprite>(spriteParams);

    Show(false);

    SetPosition(0, 0);
  }

  void Dice::Show(bool shown)
  {
    ICompositeObject::Show(shown);

    _backgroundSprite->Show(IsShown());

    for (size_t i = 0; i < kDiceSides; i++)
    {
      if (i == _currentSkill)
      {
        _skillSprites[i]->Show(IsShown());
      }
      else
      {
        _skillSprites[i]->Show(false);
      }

      if ((CheckState(kDiceState_Highlighted) || CheckState(kDiceState_PickingBuySide) || CheckState(kDiceState_ActiveSkillTarget))
        && !CheckState(kDiceState_OtherDiceActive)
        && !(CheckState(kDiceState_UsingActiveSkill)))
      {
        _highlightBackgroundSprites[i]->Show(IsShown());
        _highlightSkillSprites[i]->Show(IsShown());

        if (CheckState(kDiceState_PickingBuySide))
        {
          _highlightGreenGlowSprites[i]->Show(IsShown() && _skills[i] == kDiceSkill_Nothing && CheckState(kDiceState_PossibleBuyTarget));
        }
        else if (CheckState(kDiceState_ActiveSkillTarget))
        {
          _highlightGreenGlowSprites[i]->Show(IsShown());
        }
        else
        {
          _highlightGreenGlowSprites[i]->Show(false);
        }
      }
      else
      {
        _highlightBackgroundSprites[i]->Show(false);
        _highlightSkillSprites[i]->Show(false);
        _highlightGreenGlowSprites[i]->Show(false);
      }
    }

    _greenGlowSprite->Show(IsShown()
      && (CheckState(kDiceState_PossibleBuyTarget) || CheckState(kDiceState_PossibleActiveSkill) || CheckState(kDiceState_PossibleActiveSkillTarget))
      && !CheckState(kDiceState_PickingBuySide)
      && !CheckState(kDiceState_UsingActiveSkill)
      && !CheckState(kDiceState_ActiveSkillTarget)
      && !CheckState(kDiceState_OtherDiceActive));
    _yellowGlowSprite->Show(IsShown()
      && (CheckState(kDiceState_PickingBuySide) || CheckState(kDiceState_UsingActiveSkill))
      && !CheckState(kDiceState_OtherDiceActive));
    _blueGlowSprite->Show(IsShown()
      && CheckState(kDiceState_ActiveSkillTarget)
      && !CheckState(kDiceState_OtherDiceActive));
  }

  void Dice::RollingUpdate()
  {
    if (CheckState(kDiceState_Rolling))
    {
      _rollingFrameCount++;
      if (_rollingFrameCount == _rollingFrameCountMax)
      {
        _rollingFrameCount = 0;
        _rollingFrameCountMax++;
        if (_rollingNextHide)
        {
          _skillSprites[_currentSkill]->Show(false);
        }
        else
        {
          _skillSprites[_currentSkill]->Show(false);
          _currentSkill = GGame.RandomNumber(0, kDiceSides - 1);
          _skillSprites[_currentSkill]->Show(true);
        }
        _rollingNextHide = !_rollingNextHide;
      }

      _rollingTimer -= GTime.deltaTime;
      if (_rollingTimer <= 0.0f)
      {
        _currentSkill = _wantedSkill;
        Show(IsShown());

        RemoveState(kDiceState_Rolling);
      }
    }
  }

  bool Dice::IsHovered(const size_t side) const
  {
    const auto hoveredSprite = GGame.GetHoveredSprite();
    bool result = false;

    if (side == _currentSkill)
    {
      result = hoveredSprite == _backgroundSprite
        || hoveredSprite == _skillSprites[side]
        || hoveredSprite == _greenGlowSprite
        || hoveredSprite == _blueGlowSprite
        || hoveredSprite == _yellowGlowSprite;
    }

    result = result
      || hoveredSprite == _highlightBackgroundSprites[side]
      || hoveredSprite == _highlightGreenGlowSprites[side]
      || hoveredSprite == _highlightSkillSprites[side];

    return result;
  }

  bool Dice::IsHoveredActive() const
  {
    return IsHovered(_currentSkill);
  }

  bool Dice::IsHoveredAnything() const
  {
    for (size_t i = 0; i < kDiceSides; i++)
    {
      if (IsHovered(i)) return true;
    }
    return false;
  }

  void Dice::HoverUpdate()
  {
    if (!CheckState(kDiceState_Highlighted))
    {
      if (IsHoveredActive() && !CheckState(kDiceState_OtherDiceActive))
      {
        AddState(kDiceState_Highlighted);
      }
    }
    else
    {
      if (!IsHoveredAnything())
      {
        RemoveState(kDiceState_Highlighted);
      }
    }
  }

  void Dice::TargetingUpdate()
  {
    _highlightedSideClicked = {false, 0};

    if (CheckState(kDiceState_PossibleActiveSkillTarget) && IsHoveredAnything() && GInput.MouseButtonPressed(SDL_BUTTON_LEFT))
    {
      AddState(kDiceState_ActiveSkillTarget);
    }
    else if (CheckState(kDiceState_PossibleActiveSkill) && IsHoveredAnything() && GInput.MouseButtonPressed(SDL_BUTTON_LEFT))
    {
      AddState(kDiceState_UsingActiveSkill);
    }
    else if (CheckState(kDiceState_PossibleBuyTarget) && IsHoveredAnything() && GInput.MouseButtonPressed(SDL_BUTTON_LEFT))
    {
      AddState(kDiceState_PickingBuySide);
    }

    if ((CheckState(kDiceState_PickingBuySide) || CheckState(kDiceState_ActiveSkillTarget)) && GInput.MouseButtonPressed(SDL_BUTTON_LEFT))
    {
      const auto hoveredSprite = GGame.GetHoveredSprite();

      for (size_t i = 0; i < kDiceSides; i++)
      {
        if ((hoveredSprite == _highlightBackgroundSprites[i]
          || hoveredSprite == _highlightSkillSprites[i]
          || hoveredSprite == _highlightGreenGlowSprites[i])
          && (!CheckState(kDiceState_PickingBuySide) || _skills[i] == kDiceSkill_Nothing))
        {
          _highlightedSideClicked = { true, i };
          break;
        }
      }
    }
  }
  void Dice::TooltipUpdate()
  {
    if (_activeTooltip != nullptr)
    {
      _activeTooltip->Show(false);
      _activeTooltip = nullptr;
    }

    if (_tooltips && IsShown() && !CheckState(kDiceState_Rolling))
    {
      for (size_t i = 0; i < kDiceSides; i++)
      {
        if (_tooltips->count(_skills[i]) > 0 && IsHovered(i))
        {
          _activeTooltip = (*_tooltips)[_skills[i]];
          _activeTooltip->Show(true);
          const auto centerX = i == _currentSkill ? GetCenterX() : _highlightBackgroundSprites[i]->GetCenterX();
          const auto y = i == _currentSkill ? GetY() : _highlightBackgroundSprites[i]->GetY();
          const auto toolTipX = centerX > GGame.GetHalfWidth() ? centerX + 5 - _activeTooltip->GetWidth() : centerX + 5;
          const auto toolTipY = y > GGame.GetHalfHeight() ? y - (10 + _activeTooltip->GetHeight()) : y + GetHeight() + 10;
          _activeTooltip->SetPosition(toolTipX, toolTipY);
        }
      }
    }
  }

  void Dice::Update()
  {
    HoverUpdate();
    RollingUpdate();
    TargetingUpdate();
    TooltipUpdate();
  }

  int32_t Dice::GetWidth() const { return _backgroundSprite->GetWidth(); }
  int32_t Dice::GetHeight() const { return _backgroundSprite->GetHeight(); }
  int32_t Dice::GetX() const { return _backgroundSprite->GetX(); }
  int32_t Dice::GetY() const { return _backgroundSprite->GetY(); }
  int32_t Dice::GetCenterX() const { return _backgroundSprite->GetCenterX(); }
  int32_t Dice::GetCenterY() const { return _backgroundSprite->GetCenterY(); }

  void Dice::SetPosition(uint32_t x, uint32_t y)
  {
    _backgroundSprite->SetPosition(x, y);

    _highlightBackgroundSprites[0]->SetCenterPosition(_highlightCenterX - GetWidth() * 3 / 2, _highlightCenterY - GetHeight() * 3 / 4);
    _highlightBackgroundSprites[1]->SetCenterPosition(_highlightCenterX, _highlightCenterY - GetHeight() * 3 / 4);
    _highlightBackgroundSprites[2]->SetCenterPosition(_highlightCenterX + GetWidth() * 3 / 2, _highlightCenterY - GetHeight() * 3 / 4);
    _highlightBackgroundSprites[3]->SetCenterPosition(_highlightCenterX - GetWidth() * 3 / 2, _highlightCenterY + GetHeight() * 3 / 4);
    _highlightBackgroundSprites[4]->SetCenterPosition(_highlightCenterX, _highlightCenterY + GetHeight() * 3 / 4);
    _highlightBackgroundSprites[5]->SetCenterPosition(_highlightCenterX + GetWidth() * 3 / 2, _highlightCenterY + GetHeight() * 3 / 4);

    const auto centerX = _backgroundSprite->GetCenterX();
    const auto centerY = _backgroundSprite->GetCenterY();

    for (size_t i = 0; i < kDiceSides; i++)
    {
      _highlightSkillSprites[i]->SetCenterPosition(_highlightBackgroundSprites[i]->GetCenterX(), _highlightBackgroundSprites[i]->GetCenterY());
      _highlightGreenGlowSprites[i]->SetCenterPosition(_highlightBackgroundSprites[i]->GetCenterX(), _highlightBackgroundSprites[i]->GetCenterY());
      _skillSprites[i]->SetCenterPosition(centerX, centerY);
    }

    _greenGlowSprite->SetCenterPosition(centerX, centerY);
    _yellowGlowSprite->SetCenterPosition(centerX, centerY);
    _blueGlowSprite->SetCenterPosition(centerX, centerY);
  }

  void Dice::SetCenterPosition(uint32_t x, uint32_t y)
  {
    _backgroundSprite->SetCenterPosition(x, y);
    SetPosition(_backgroundSprite->GetX(), _backgroundSprite->GetY());
  }

  void Dice::AnimateRoll(const size_t finalSide, const float duration)
  {
    _rollingFrameCount = 0;
    _rollingFrameCountMax = 1;
    _wantedSkill = finalSide;
    AddState(kDiceState_Rolling);
    _rollingTimer = duration;
    _rollingNextHide = false;
  }

  DiceSkill Dice::GetSkill(const size_t side) const
  {
    return _skills[side];
  }

  DiceSkill Dice::GetCurrentSkill() const
  {
    if (CheckState(kDiceState_Rolling))
    {
      return _skills[_wantedSkill];
    }
    return _skills[_currentSkill];
  }

  DiceSkill Dice::Roll(const float duration)
  {
    if (!IsShown())
    {
      Show(true);
    }

    std::unordered_set<size_t> possibleSides;
    for (size_t i = 0; i < kDiceSides; i++)
    {
      if (_skills[i] != kDiceSkill_Reroll) possibleSides.insert(i);
    }

    const auto wantedSide = possibleSides.size() == 0 ? 0 : *std::next(std::cbegin(possibleSides), GGame.RandomNumber(size_t{ 0 }, possibleSides.size() - 1));
    AnimateRoll(wantedSide, duration);
    return _skills[wantedSide];
  }

  void Dice::AddState(const DiceState state)
  {
    _state |= state;

    Show(IsShown());
  }

  void Dice::RemoveState(const DiceState state)
  {
    _state = _state & ~state;

    Show(IsShown());
  }

  std::pair<bool, size_t> Dice::SidePicked() const
  {
    return _highlightedSideClicked;
  }

  void Dice::ReplaceSide(const size_t side, const DiceSkill newSkill)
  {
    _skills[side] = newSkill;

    SpriteParams spriteParams;
    spriteParams.persistant = false;
    spriteParams.texture = nullptr;
    spriteParams.z = _skillSprites[side]->GetZ();
    spriteParams.textureName = kDiceSkillTextures.find(newSkill)->second;

    GGame.DestroySprite(_skillSprites[side]);
    _skillSprites[side] = GGame.Create<Sprite>(spriteParams);

    GGame.DestroySprite(_highlightSkillSprites[side]);
    _highlightSkillSprites[side] = GGame.Create<Sprite>(spriteParams);

    SetPosition(GetX(), GetY());
    Show(IsShown());
  }
}