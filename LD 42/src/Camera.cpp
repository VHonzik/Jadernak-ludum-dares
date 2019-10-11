#include "Camera.h"

#include "SDL.h"
#include "Input.h"

namespace CherEngine
{
  Camera GCamera;

  Camera::Camera()
    : _resolutionWidth(0)
    , _resolutionHeight(0)
    , _x(0)
    , _y(0)
  {
  }

  void Camera::TransformMouse(int32_t& x, int32_t& y)
  {
    x = x - (_resolutionWidth / 2) - _x;
    y = y - (_resolutionHeight / 2) - _y;
  }

  void Camera::Transform(SDL_Rect& rect)
  {
    rect.x += (_resolutionWidth / 2) + _x;
    rect.y += (_resolutionHeight / 2) + _y;
  }

  void Camera::SetResolution(int32_t width, int32_t height)
  {
    _resolutionWidth = width;
    _resolutionHeight = height;
  }

  void Camera::Translate(int32_t x, int32_t y)
  {
    _x += x;
    _y += y;
  }

  bool Camera::IsMouseInside(SDL_Rect& rect)
  {
    auto mouseTransformedX = GInput.MouseX, mouseTransformedY = GInput.MouseY;
    TransformMouse(mouseTransformedX, mouseTransformedY);

    return mouseTransformedX >= rect.x && mouseTransformedX <= rect.x + rect.w && mouseTransformedY >= rect.y && mouseTransformedY <= rect.y + rect.h;
  }
}