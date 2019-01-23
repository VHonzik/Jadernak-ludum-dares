#include "BoxSprite.h"

#include "Texture.h"
#include "Game.h"

namespace LD43
{
  BoxSprite::BoxSprite(const BoxSpriteParams& params)
    : Sprite(params)
    , _cornerSize(params.cornerSize)
    , _boundingBox{0, 0, params.width , params.height}
    , _boundingBoxOverwritten(false)
  {
    _screenTransform.w = params.width;
    _screenTransform.h = params.height;
  }

  void BoxSprite::DoRender(SDL_Renderer* renderer)
  {
    if (_shown)
    {
      // TL corner
      auto destination = SDL_Rect{ _screenTransform.x, _screenTransform.y, _cornerSize, _cornerSize };
      auto source = SDL_Rect{ 0, 0, _cornerSize, _cornerSize };
      SDL_RenderCopy(renderer, _texture, &source, &destination);

      // TR corner
      source.x = _textureDescription->width - _cornerSize;
      destination.x = _screenTransform.x + _screenTransform.w - _cornerSize;
      SDL_RenderCopy(renderer, _texture, &source, &destination);

      // BR corner
      source.y = _textureDescription->height - _cornerSize;
      destination.y = _screenTransform.y + _screenTransform.h - _cornerSize;
      SDL_RenderCopy(renderer, _texture, &source, &destination);

      // BL corner
      source.x = 0;
      destination.x = _screenTransform.x;
      SDL_RenderCopy(renderer, _texture, &source, &destination);

      // T line
      destination = { _screenTransform.x + _cornerSize, _screenTransform.y,
        _screenTransform.w - 2 * _cornerSize, _cornerSize };
      source = SDL_Rect{ _cornerSize, 0, _textureDescription->width - 2 * _cornerSize, _cornerSize };
      SDL_RenderCopy(renderer, _texture, &source, &destination);

      // B line
      destination.y = _screenTransform.y + _screenTransform.h - _cornerSize;
      source.y = _textureDescription->height - _cornerSize;
      SDL_RenderCopy(renderer, _texture, &source, &destination);

      // L line
      destination = { _screenTransform.x, _screenTransform.y + _cornerSize,
        _cornerSize, _screenTransform.h - 2 * _cornerSize, };
      source = SDL_Rect{ 0, _cornerSize, _cornerSize, _textureDescription->height - 2 * _cornerSize };
      SDL_RenderCopy(renderer, _texture, &source, &destination);

      // R line
      destination.x = _screenTransform.x + _screenTransform.w - _cornerSize;
      source.x = _textureDescription->width - _cornerSize;
      SDL_RenderCopy(renderer, _texture, &source, &destination);

      // Center
      destination = { _screenTransform.x + _cornerSize, _screenTransform.y + _cornerSize,
        _screenTransform.w - 2 * _cornerSize, _screenTransform.h - 2 * _cornerSize };
      source = SDL_Rect{ _cornerSize, _cornerSize,
        _textureDescription->width - 2 * _cornerSize, _textureDescription->height - 2 * _cornerSize };
      SDL_RenderCopy(renderer, _texture, &source, &destination);
    }
  }

  void BoxSprite::SetBoundingBox(const SDL_Rect& box)
  {
    _boundingBox = box;
    _boundingBoxOverwritten = true;
  }

  const SDL_Rect& BoxSprite::GetBoundingBox() const
  {
    return _boundingBox;
  }


  void BoxSprite::SetWidth(int32_t width)
  {
    Sprite::SetWidth(width);
    if (!_boundingBoxOverwritten)
    {
      _boundingBox.w = width;
    }
  }

  void BoxSprite::SetHeight(int32_t height)
  {
    Sprite::SetHeight(height);
    if (!_boundingBoxOverwritten)
    {
      _boundingBox.h = height;
    }
  }
}