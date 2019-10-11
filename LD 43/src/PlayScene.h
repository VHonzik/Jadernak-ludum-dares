#pragma once

#include "IScene.h"

#include <vector>

namespace LD43
{
  class Sprite;
  class FTC;
  class BoxSprite;
  class Text;
  class BuildingIcon;
  class ResourceBar;
  class BuildingPanel;

  class PlayScene : public IScene
  {
  public:
    void Start() override;
    void Update() override;
  private:
    std::vector<Sprite*> _backgroundSprites;
    Sprite* _landingPlatform;
    Sprite* _spacecraft;

    ResourceBar* _resourceBar;
    BuildingPanel* _buildingPanel;

    int32_t _rows;
    int32_t _columns;

    bool _landingFinished;
    float _landingTimer;
    std::pair<int32_t, int32_t> _landingDestination;

    float _uiSlideTimer;
    bool _uiSlideFinished;

    std::pair<int32_t, int32_t> VectorIndexToXY(int32_t index) const;
    std::pair<int32_t, int32_t> XYToCenterXY(int32_t x, int32_t y) const;
    int32_t CenterXYToVectorIndex(int32_t x, int32_t y) const;
    bool IsIndexFullyVisible(int32_t index) const;
    std::pair<int32_t, int32_t> VectorIndexToRowColumn(int32_t index) const;
    int32_t RowColumnToVectorIndex(int32_t row, int32_t column) const;

    void LandingIntroUpdate();
    void UISlideIntroUpdate();
  };
}
