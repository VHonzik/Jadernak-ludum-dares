#pragma once

#include <utility>
#include <vector>

namespace CherEngine
{
  class IScene;
  class Sprite;
}

namespace LD42
{
  const int32_t kElementsCount = 8;

  enum SplitDirection
  {
    kSplitDirection_Horizontal,
    kSplitDirection_Vertical,
    kSplitDirection_DiagonalLR,
    kSplitDirection_DiagonalRL,
  };

  class IconSprite
  {
  public:
    IconSprite(CherEngine::IScene& scene, const char* name, float weight);
    void Split(SplitDirection direction);
    CherEngine::Sprite* GetMasterSprite() const;
    CherEngine::Sprite* GetSecondMasterSprite() const;
    void SetVisible(bool visible);
    void Restore();
    float Weight;
  private:
    CherEngine::Sprite* _masterSprite;
    CherEngine::Sprite* _secondMasterSprite;
    std::vector<CherEngine::Sprite*> _sprites;
    bool _wasSplit;
  };
}
