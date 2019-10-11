#pragma once

#include "SDL.h"

namespace CherEngine
{
  class Camera
  {
  private:
    int32_t _resolutionWidth;
    int32_t _resolutionHeight;
    int32_t _x;
    int32_t _y;

  public:
    Camera();

    bool IsMouseInside(SDL_Rect& rect);

    void SetResolution(int32_t width, int32_t height);
    void Transform(SDL_Rect& rect);
    void TransformMouse(int32_t& x, int32_t& y);
    void Translate(int32_t x, int32_t y);
  };

  extern Camera GCamera;
}
