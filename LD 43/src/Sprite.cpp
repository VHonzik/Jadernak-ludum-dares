#include "Sprite.h"

#include "Camera.h"
#include "Game.h"
#include "Texture.h"
#include "Utils.h"

#include <cassert>

#include <SDL.h>

namespace LD43
{
  Sprite::Sprite(const SpriteParams& params)
    : _preloaded(false)
    , _shown(true)
    , _z(params.z)
    , _alpha(1.0f)
  {
    if (params.texture)
    {
      _textureDescription = params.texture;
    }
    else
    {
      _textureDescription = GGame.FindTexture(params.textureName);
    }

    assert(_textureDescription);

    _screenTransform = { 0, 0, _textureDescription->width, _textureDescription->height };
    _texture = _textureDescription->texture;

    _boundingBox = _textureDescription->boundingBox;
  }

  void Sprite::Preload(SDL_Renderer* renderer)
  {
    _preloaded = DoPreload(renderer);
  }

  void Sprite::Render(SDL_Renderer* renderer)
  {
    if (_shown)
    {
      DoRender(renderer);
    }
  }

  void Sprite::DoRender(SDL_Renderer* renderer)
  {
    SDL_Rect destination = _screenTransform;
    if (_masked)
    {
      SDL_Rect interesection;
      if (SDL_IntersectRect(&destination, &_mask, &interesection) != SDL_FALSE)
      {
        SDL_Rect source = { interesection.x - destination.x, interesection.y - destination.y,
          interesection.w, interesection.h };
        SDL_RenderCopy(renderer, _texture, &source, &interesection);
      }
    }
    else
    {
      SDL_RenderCopy(renderer, _texture, nullptr, &destination);
    }
  }


  void Sprite::SetPosition(uint32_t x, uint32_t y)
  {
    _screenTransform.x = x;
    _screenTransform.y = y;
  }

  void Sprite::SetCenterPosition(uint32_t x, uint32_t y)
  {
    _screenTransform.x = x - _screenTransform.w / 2;
    _screenTransform.y = y - _screenTransform.h / 2;
  }

  void Sprite::SetWidth(int32_t width)
  {
    if (_screenTransform.w != width)
    {
      _screenTransform.w = width;
      _boundingBox = { 0, 0, _screenTransform.w, _screenTransform.h };
    }
  }

  void Sprite::SetHeight(int32_t height)
  {
    if (_screenTransform.h != height)
    {
      _screenTransform.h = height;
      _boundingBox = { 0, 0, _screenTransform.w, _screenTransform.h };
    }
  }

  void Sprite::Tint(const SDL_Color& tintColor)
  {
    if (!_textureDescription->isCopy)
    {
      _textureDescription = GGame.CopyTexture(_textureDescription);
      _texture = _textureDescription->texture;
    }

    SDL_SetTextureColorMod(_texture, tintColor.r, tintColor.g, tintColor.b);
  }

  void Sprite::Clean()
  {
  }

  bool Sprite::HasHitTest() const
  {
    return _textureDescription->hitArray.size() > 0;
  }

  bool Sprite::HitTest(uint32_t x, uint32_t y) const
  {
    assert(HasHitTest());
    return _textureDescription->hitArray[x + y * _screenTransform.w];
  }

  const SDL_Rect& Sprite::GetBoundingBox() const
  {
    return _boundingBox;
  }

  const std::string& Sprite::GetTextureName() const
  {
    return _textureDescription->name;
  }

  void Sprite::SetMask(const SDL_Rect& mask)
  {
    _masked = true;
    _mask = mask;
  }

  void Sprite::SetAlpha(float alpha)
  {
    if (_alpha != Clamp01(alpha))
    {
      if (!_textureDescription->isCopy)
      {
        _textureDescription = GGame.CopyTexture(_textureDescription);
        _texture = _textureDescription->texture;
      }

      _alpha = Clamp01(alpha);
      auto aplhaMod = static_cast<uint8_t>(_alpha * 255.0f);
      SDL_SetTextureAlphaMod(_texture, aplhaMod);
    }
  }

  float Sprite::GetAlpha() const
  {
    return _alpha;
  }

  void Sprite::Show(bool shown)
  {
    _shown = shown;
  }
}