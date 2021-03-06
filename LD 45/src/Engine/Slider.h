#pragma once

#include "ICompositeObject.h"

#include <SDL.h>
#include <string>

namespace LD45
{
  class Sprite;
  class Text;

  struct SliderParams
  {
    bool persistant;
    std::string font;
    uint32_t fontSize;
    SDL_Color fontColor;
    std::string minTitle;
    std::string maxTitle;
    int32_t width;
    int32_t minMaxYMargin;
    std::string axisTexture;
    std::string axisEndTexture;
    std::string pointerTexture;
    int32_t z;
    float initialValue;
  };

  class Slider : public ICompositeObject
  {
  public:
    Slider(const SliderParams& params);

    void Update() override;

    void SetPosition(uint32_t x, uint32_t y);
    void SetCenterPosition(uint32_t x, uint32_t y);

    void Show(bool shown) override;

    int32_t GetX() const;
    int32_t GetY() const;

    int32_t GetCenterX() const;
    int32_t GetCenterY() const;

    int32_t GetWidth() const;
    int32_t GetHeight() const;

    float GetValue() const { return _value; }
    bool Changed() const { return _valueChanged; }
    bool Released() const { return _slidingReleased;  }

  private:
    Sprite* _pointer;
    Sprite* _axis;
    Sprite* _axisLEnd;
    Sprite* _axisREnd;

    Text* _minTitle;
    Text* _maxTitle;

    int32_t _width;
    int32_t _minMaxYMargin;

    bool _valueChanged;
    float _value;

    bool _sliding;
    bool _slidingReleased;
    int32_t _slidingOffset;
  };
}
