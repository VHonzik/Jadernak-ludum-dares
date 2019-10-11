#pragma once

#include "ICompositeObject.h"
#include "EngineSettings.h"

#include <string>

namespace LD43
{
  struct BuildingIconParams
  {
    bool persistant;
    std::string iconName;
    std::string title;
    std::string description;
    SettingsIDs keybind;
    int32_t woodCost;
    int32_t crystalCost;
    int32_t z;
    int32_t index;
  };

  class Sprite;
  class FTC;
  class Text;

  class BuildingIcon : public ICompositeObject
  {
  public:
    BuildingIcon(const BuildingIconParams& params);

    void Update() override;
    void Show(bool shown) override;

    void Building(bool building);

    void SetPosition(int32_t x, int32_t y);
    bool Hovered() const { return _hovered; }
    bool CanAfford() const { return _canAfford; }

  private:
    Sprite* _icon;
    Sprite* _iconDisabled;
    FTC* _title;
    Text* _description;
    Sprite* _woodCostIcon;
    FTC* _woodCostText;
    Sprite* _crystalCostIcon;
    FTC* _crystalCostText;
    Sprite* _selectorHover;

    int32_t _woodCost;
    int32_t _crystalCost;
    int32_t _offsetX;

    bool _hovered;
    bool _canAfford;
    bool _building;


  };
}
