#pragma once

#include "ICompositeObject.h"
#include "LD45Styles.h"


#include <array>
#include <cinttypes>

namespace LD45
{
  class Sprite;
  class Tooltip;
  class FTC;

  struct DiceBuyParams
  {
    bool                              persistant;

    std::array<DiceSkill, kDiceSides> skills;
    int32_t                           cost;
    size_t                            count;

    int32_t                           highlightCenterX;
    int32_t                           highlightCenterY;

    int32_t                           z;
  };

  class DiceBuy : public ICompositeObject
  {
  public:
    DiceBuy(const DiceBuyParams& params);

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

    void HideAfford();
    void UpdateCanAfford(const int32_t gold);
    bool CanAfford(const int32_t gold) const { return true; }

    void SetTooltips(std::unordered_map<DiceSkill, Tooltip*>* tooltips) { _tooltips = tooltips; }

    void SetDiceBought(bool bought);

    bool Pressed() { return _pressed; }
    bool Down() { return _down; }
    bool Released() { return _released; }
    bool DiceBought() { return _diceBought; }

    size_t GetAmount() { return _amount; }

    void EnterBuy();
    void ConfirmBuy();

    std::array<DiceSkill, kDiceSides> GetSkills() const { return _skills; }

  private:
    void TooltipUpdate();
    bool IsHovered(const size_t side) const;

    Sprite* _backgroundSprite;
    std::array<DiceSkill, kDiceSides> _skills;
    Sprite* _skillSprite;
    Sprite* _canBuyGlow;
    Sprite* _buyingGlow;

    std::array<Sprite*, kDiceSides> _highlightSkillSprites;
    std::array<Sprite*, kDiceSides> _highlightBackgroundSprites;

    int32_t _highlightCenterX;
    int32_t _highlightCenterY;

    size_t _amount;
    FTC* _amountText;
    FTC* _costText;

    bool _pressed;
    bool _released;
    bool _down;
    bool _hovered;

    bool _diceBought;
    bool _showingAfford;
    bool _buying;

    std::unordered_map<DiceSkill, Tooltip*>* _tooltips;
    Tooltip* _activeTooltip;
  };
}
