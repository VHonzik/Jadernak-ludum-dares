#pragma once

#include <SDL.h>

namespace LD43
{
  class FTC;
  class Sprite;

  class Camera
  {
  public:
    Camera();
    SDL_Rect Transform(const SDL_Rect& rect);

    void SetResolution(int32_t width, int32_t height);
    void SetDisplayMode(const SDL_Rect& scaledRect, const SDL_Rect& windowRect);

    int32_t WindowToRenderX(const int32_t x) const;
    int32_t WindowToRenderY(const int32_t y) const;

    bool IsMouseInside(const Sprite* sprite, bool preciseTest);

    void Update();

    void SetPosition(int x, int y);
    int GetX() const { return _x; }
    int GetY() const { return _y; }

  private:
    int _x;
    int _y;

    int32_t _resolutionWidth;
    int32_t _resolutionHeight;

    int32_t _scaledResolutionWidth;
    int32_t _scaledResolutionHeight;

    int32_t _windowResolutionWidth;
    int32_t _windowResolutionHeight;

    FTC* _mouse_position_ftc;
  };

  extern Camera GCamera;
}
