#pragma once

#include "ICompositeObject.h"

#include "LD45Styles.h"

#include <array>
#include <string>
#include <vector>

namespace LD45
{

  class FTC;
  class Sprite;
  class Tooltip;

  struct UpgradeParams
  {
    bool persistant;

    int32_t cost;
    DiceSkill skill;
    size_t count;
    int32_t z;
  };

  class Upgrade : public ICompositeObject
  {
  public:
    Upgrade(const UpgradeParams& params);

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
    bool CanAfford(const int32_t gold) const { return _cost <= gold && _amount > 0; }
    int32_t Cost() const { return _cost; }
    bool IsBuying() const { return _buying; }

    void EnterBuy();
    void CancelBuy();
    void ConfirmBuy();

    bool Pressed() { return _pressed; }
    bool Down() { return _down; }
    bool Released() { return _released; }

    DiceSkill GetSkill() const { return _skill; }

    bool IsEmpty() const { return _amount == 0; }

    void SetTooltips(std::unordered_map<DiceSkill, Tooltip*>* tooltips) { _tooltips = tooltips; }

  private:
    size_t _amount;
    Sprite* _background;
    Sprite* _skillSprite;
    Sprite* _canBuyGlow;
    Sprite* _buyingGlow;
    FTC* _costText;
    FTC* _amountText;
    int32_t _cost;

    DiceSkill _skill;

    bool _pressed;
    bool _released;
    bool _down;
    bool _hovered;

    bool _showingAfford;
    bool _buying;

    std::unordered_map<DiceSkill, Tooltip*>* _tooltips;
    Tooltip* _activeTooltip;
  };
}
