#include "Camera.h"

#include "Input.h"
#include "Game.h"
#include "Sprite.h"
#include "Utils.h"

namespace LD45
{
  const int32_t kTileWidth = 256;
  const int32_t kTileWidthHalf = kTileWidth / 2;
  const int32_t kTileHeight = 272;
  const int32_t kTileHeightHalf = kTileHeight / 2;

  const int32_t kFloorSize = 19;
  const int32_t kFloorHeight = 169;

  Camera::Camera()
    : _x(0)
    , _y(0)
    , _resolutionWidth(0)
    , _resolutionHeight(0)
    , _mouse_position_ftc(nullptr)
  {
  }

  void Camera::SetResolution(int32_t width, int32_t height)
  {
    _resolutionWidth = _scaledResolutionWidth = _windowResolutionWidth =  width;
    _resolutionHeight = _scaledResolutionHeight = _windowResolutionHeight = height;
  }

  void Camera::SetDisplayMode(const SDL_Rect& scaledRect, const SDL_Rect& windowRect)
  {
    _scaledResolutionWidth = scaledRect.w;
    _scaledResolutionHeight = scaledRect.h;
    _windowResolutionWidth = windowRect.w;
    _windowResolutionHeight = windowRect.h;
  }

  void Camera::SetPosition(int x, int y)
  {
    _x = x;
    _y = y;
  }

  SDL_Rect Camera::Transform(const SDL_Rect& rect)
  {
    SDL_Rect result = { rect.x - _x, rect.y - _y, rect.w, rect.h };
    return result;
  }

  int32_t Camera::WindowToRenderX(const int32_t x) const
  {
    const auto diff = ((_windowResolutionWidth - _scaledResolutionWidth) * 0.5f);
    const auto minX = diff;
    const auto maxX = _windowResolutionWidth - diff;
    const auto clamped = Clamp(static_cast<float>(x), minX, maxX);
    const auto scalingFactor = static_cast<float>(_resolutionWidth) / _scaledResolutionWidth;
    return static_cast<int32_t>((clamped-diff) * scalingFactor);
  }

  int32_t Camera::WindowToRenderY(const int32_t y) const
  {
    const auto diff = ((_windowResolutionHeight - _scaledResolutionHeight) * 0.5f);
    const auto minY = diff;
    const auto maxY = _windowResolutionWidth - diff;
    const auto clamped = Clamp(static_cast<float>(y), minY, maxY);
    const auto scalingFactor = static_cast<float>(_resolutionHeight) / _scaledResolutionHeight;
    return static_cast<int32_t>((clamped - diff) * scalingFactor);
  }

  bool Camera::IsMouseInside(const Sprite* sprite, bool preciseTest)
  {
    if (preciseTest && !sprite->HasHitTest())
    {
      return true;
    }

    auto mouseX = GInput.GetMouseX();
    auto mouseY = GInput.GetMouseY();

    const auto tranform = sprite->GetScreenTransform();

    bool inside = false;

    if (preciseTest)
    {
      inside = IsInsideRect(mouseX, mouseY, tranform);

      if (inside)
      {
        inside = sprite->HitTest(mouseX - tranform.x, mouseY - tranform.y);
      }
    }
    else
    {
      SDL_Rect boundingBox = sprite->GetBoundingBox();
      SDL_Rect testingBox = { tranform.x + boundingBox.x,
        tranform.y + boundingBox.y, boundingBox.w, boundingBox.h };
      inside = IsInsideRect(mouseX, mouseY, testingBox);
    }

    return inside;
  }

  void Camera::Update()
  {

  }

  Camera GCamera;
}