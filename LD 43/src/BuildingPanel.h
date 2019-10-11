#pragma once

#include "ICompositeObject.h"
#include <utility>
#include <cstdint>
#include <vector>

namespace LD43
{
  class Sprite;
  class BoxSprite;
  class BuildingIcon;

  struct BuildingPanelParams
  {
    bool persistant;
    int32_t z;
  };

  class BuildingPanel : public ICompositeObject
  {
  public:
    BuildingPanel(const BuildingPanelParams& params);
    void Update() override;
    void Show(bool shown) override;

    void UpdateCursorPos(const std::pair<int32_t, int32_t>& center, bool visible, bool occupied);

    void SetPosition(int32_t x, int32_t y);
    int32_t GetX() const;
    int32_t GetY() const;
    int32_t GetWidth() const;
    int32_t GetHeight() const;

  private:
    Sprite* _greySelector;
    Sprite* _redSelector;
    Sprite* _greenSelector;
    std::vector<Sprite*> _previewIcons;

    BoxSprite* _panel;
    std::vector<BuildingIcon*> _icons;

    bool _cursorVisible;
    bool _cursorOccupied;
    bool _building;
    int32_t _buildingIndex;
  };
}
