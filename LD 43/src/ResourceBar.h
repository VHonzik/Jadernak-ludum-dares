#pragma once

#include "ICompositeObject.h"
#include <cstdint>

namespace LD43
{
  struct ResourceBarParams
  {
    bool persistant;
    int32_t initialWood;
    int32_t initialCrystal;
    int32_t z;
  };

  class BoxSprite;
  class Sprite;
  class FTC;

  class ResourceBar : public ICompositeObject
  {
  public:
    ResourceBar(const ResourceBarParams& params);

    static ResourceBar* Get() { return _one_only; }

    int32_t GetWood() const { return _wood; }
    int32_t GetCrystal() const { return _crystal; }

    bool CanAfford(int32_t wood, int32_t crystal);

    void SetY(int32_t y);
    int32_t GetHeight() const;
  private:
    BoxSprite* _panel;
    Sprite* _woodIcon;
    FTC* _woodText;
    Sprite* _crystalIcon;
    FTC* _crystalText;

    static ResourceBar* _one_only;

    int32_t _wood;
    int32_t _crystal;
  };
}
