#pragma once

#include "ICompositeObject.h"

#include "LD45Styles.h"

#include <string>
#include <array>

namespace LD45
{
  class Sprite;
  class Tooltip;

  enum DiceState
  {
    kDiceState_Rolling = 0b1,
    kDiceState_Highlighted = 0b10,
    kDiceState_PossibleBuyTarget = 0b100,
    kDiceState_PickingBuySide = 0b1000,
    kDiceState_OtherDiceActive = 0b10000,
    kDiceState_AIDice = 0b100000,
    kDiceState_PossibleActiveSkill = 0b1000000,
    kDiceState_UsingActiveSkill = 0b10000000,
    kDiceState_PossibleActiveSkillTarget = 0b100000000,
    kDiceState_ActiveSkillTarget = 0b1000000000,
    kDiceState_ActiveSkillUsed = 0b10000000000,
  };

  struct DiceParams
  {
    bool persistant;

    std::array<DiceSkill, kDiceSides> initialSkills;

    int32_t highlightCenterX;
    int32_t highlightCenterY;

    int32_t z;
  };

  class Dice : public ICompositeObject
  {
  public:
    Dice(const DiceParams& params);

    void Show(bool shown) override;
    void Update() override;

    void SetPosition(uint32_t x, uint32_t y);
    void SetCenterPosition(uint32_t x, uint32_t y);

    int32_t GetWidth() const;
    int32_t GetHeight() const;

    int32_t GetX() const;
    int32_t GetY() const;

    int32_t GetCenterX() const;
    int32_t GetCenterY() const;

    void AnimateRoll(const size_t finalSide, const float duration);
    DiceSkill Roll(const float duration);

    DiceSkill GetCurrentSkill() const;
    DiceSkill GetSkill(const size_t side) const;

    int32_t GetState() const { return _state; }
    void AddState(const DiceState state);
    void RemoveState(const DiceState state);
    bool CheckState(const DiceState state) const { return (_state & state) != 0; }

    size_t EmptySideIndex() const
    {
      const auto found = std::find(std::cbegin(_skills), std::cend(_skills), kDiceSkill_Nothing);
      return std::distance(std::cbegin(_skills), found);
    }

    bool HasEmptySide() const
    {
      return std::find(std::cbegin(_skills), std::cend(_skills), kDiceSkill_Nothing) != std::cend(_skills);
    }

    size_t EmptySidesCount() const
    {
      return std::count_if(std::cbegin(_skills), std::cend(_skills), [](const DiceSkill& skill) { return skill == kDiceSkill_Nothing; });
    }

    bool RolledActiveSkill() const
    {
      return GetCurrentSkill() == kDiceSkill_Trash || GetCurrentSkill() == kDiceSkill_UpGold;
    }

    std::pair<bool, size_t> SidePicked() const;
    void ReplaceSide(const size_t side, const DiceSkill newSkill);

    void SetTooltips(std::unordered_map<DiceSkill, Tooltip*>* tooltips) { _tooltips = tooltips; }

  private:
    void RollingUpdate();
    void HoverUpdate();
    void TargetingUpdate();
    void TooltipUpdate();

    bool IsHoveredActive() const;
    bool IsHoveredAnything() const;
    bool IsHovered(const size_t side) const;

    Sprite* _backgroundSprite;
    std::array<DiceSkill, kDiceSides> _skills;
    std::array<Sprite*, kDiceSides> _skillSprites;
    Sprite* _greenGlowSprite;
    Sprite* _yellowGlowSprite;
    Sprite* _blueGlowSprite;

    std::array<Sprite*, kDiceSides> _highlightGreenGlowSprites;
    std::array<Sprite*, kDiceSides> _highlightSkillSprites;
    std::array<Sprite*, kDiceSides> _highlightBackgroundSprites;

    size_t _currentSkill;

    float _rollingTimer;
    size_t _wantedSkill;
    int32_t _rollingFrameCount;
    int32_t _rollingFrameCountMax;
    bool _rollingNextHide;

    int32_t _state;

    int32_t _highlightCenterX;
    int32_t _highlightCenterY;

    std::pair<bool, size_t> _highlightedSideClicked;

    std::unordered_map<DiceSkill, Tooltip*>* _tooltips;
    Tooltip* _activeTooltip;
  };
}
