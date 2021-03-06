#pragma once

#include "ICompositeObject.h"

#include <cstdint>
#include <string>

namespace LD43
{
  class Sprite;

  struct CheckboxParams
  {
    bool persistant;
    std::string checkedTexture;
    std::string emptyTexture;
    int32_t z;
    bool checked;
  };

  class Checkbox : public ICompositeObject
  {
  public:
    Checkbox(const CheckboxParams& params);

    void Update() override;

    void SetPosition(int32_t x, int32_t y);
    void SetCenterPosition(int32_t x, int32_t y);

    void Show(bool shown) override;

    int32_t GetX() const;
    int32_t GetY() const;

    int32_t GetCenterX() const;
    int32_t GetCenterY() const;

    int32_t GetWidth() const;
    int32_t GetHeight() const;

    bool Changed() const { return _checkedChanged; }
    bool Checked() const { return _checked; }

  private:
    bool _checked;
    bool _checkedChanged;

    Sprite* _checkedSprite;
    Sprite* _emptySprite;
  };
}
