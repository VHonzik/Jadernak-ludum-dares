#include "Sprite.h"

#include "Camera.h"
#include "Input.h"

#include <SDL.h>
#include <SDL_image.h>
#include <SDL_FontCache.h>

#include <algorithm>

namespace CherEngine
{
  Sprite::Sprite()
    : _texture(nullptr)
    , _width(0)
    , _height(0)
    , CenterX(0)
    , CenterY(0)
    , _x(0)
    , _y(0)
    , _gridSize(0)
    , _visible(true)
    , _rect{ 0, 0, 0, 0 }
    , _attached(false)
    , _parent(nullptr)
    , _tooltip(nullptr)
    , _autoWidth(false)
    , _hover(false)
    , _scale (1.0f)
    , _widthPercentage(1.0f)
    , _alpha(1.0f)
  {
    _renderFunction = [](SDL_Renderer* renderer) {};
  }

  Sprite::Sprite(SDL_Texture* texture, int32_t width, int32_t height)
    : _texture(texture)
    , _width(width)
    , _height(height)
    , CenterX(0)
    , CenterY(0)
    , _x(-width / 2)
    , _y(-height / 2)
    , _visible(true)
    , _rect{ -width / 2, -height / 2, width, height }
    , _attached(false)
    , _parent(nullptr)
    , _tooltip(nullptr)
    , _autoWidth(false)
    , _hover(false)
    , _scale(1.0f)
    , _widthPercentage(1.0f)
    , _alpha(1.0f)
  {
  }

  Sprite::Sprite(SDL_Texture* texture, int32_t width, int32_t height, int32_t gridSize)
    : _texture(texture)
    , _width(width)
    , _height(height)
    , CenterX(0)
    , CenterY(0)
    , _x(-width / 2)
    , _y(-height / 2)
    , _gridSize(gridSize)
    , _visible(true)
    , _rect{ -width / 2, -height / 2, width, height }
    , _attached(false)
    , _parent(nullptr)
    , _tooltip(nullptr)
    , _autoWidth(false)
    , _hover(false)
    , _scale(1.0f)
    , _widthPercentage(1.0f)
    , _alpha(1.0f)
  {
  }

  Sprite::Sprite(FC_Font* font, int32_t width, int32_t height, const char* text)
    : _texture(nullptr)
    , _font(font)
    , _width(width)
    , _height(height)
    , CenterX(0)
    , CenterY(0)
    , _x(-width / 2)
    , _y(-height / 2)
    , _text(text)
    , _visible(true)
    , _rect{ -width / 2, -height / 2, width, height }
    , _attached(false)
    , _parent(nullptr)
    , _tooltip(nullptr)
    , _autoWidth(width < 0)
    , _hover(false)
    , _scale(1.0f)
    , _widthPercentage(1.0f)
    , _alpha(1.0f)
  {
    if (width < 0)
    {
      _width = FC_GetWidth(_font, _text.c_str());
      _x = (-_width / 2);
      _rect = { -_width / 2, -_height / 2, _width, _height };
    }
  }

  void Sprite::SetAsSimpleSprite()
  {
    _renderFunction = [&](SDL_Renderer* renderer)
    {
      this->RenderSimple(renderer);
    };
  }

  void Sprite::SetAsBoxSprite()
  {
    _renderFunction = [&](SDL_Renderer* renderer)
    {
      this->RenderBoxSprite(renderer);
    };
  }

  void Sprite::SetAsTextBox()
  {
    _renderFunction = [&](SDL_Renderer* renderer)
    {
      this->RenderTextBox(renderer);
    };
  }

  void Sprite::AttachTo(Sprite* parent)
  {
    if (parent != nullptr)
    {
      _attached = true;
      _parent = parent;
    }
    else
    {
      _attached = false;
      _parent = nullptr;
    }
  }

  void Sprite::AddTooltip(Sprite& tooltip)
  {
    _tooltip = &tooltip;
    _tooltip->SetVisible(false);
  }

  void Sprite::SetVisible(bool visible)
  {
    _visible = visible;
  }

  void  Sprite::SetText(const char* text)
  {
    _text = text;
    if (_autoWidth)
    {
      _width = FC_GetWidth(_font, _text.c_str());
      _x = (-_width / 2);
      _rect = { -_width / 2, -_height / 2, _width, _height };
    }
  }

  void Sprite::SetScale(float scale)
  {
    _scale = scale;
  }

  void Sprite::SetWidthPercentage(float widthPc)
  {
    _widthPercentage = widthPc;
  }

  void Sprite::SetAlpha(float alpha)
  {
    if (std::abs(alpha - _alpha) > 0.001f)
    {
      _alpha = std::max(std::min(1.0f, alpha), 0.0f);

      SDL_SetTextureAlphaMod(_texture, static_cast<uint8_t>(_alpha * 255));

      if (_alpha < 1.0f)
      {
        SDL_SetTextureBlendMode(_texture, SDL_BLENDMODE_BLEND);
      }
      else
      {
        SDL_SetTextureBlendMode(_texture, SDL_BLENDMODE_NONE);
      }
    }
  }

  void Sprite::Render(SDL_Renderer* renderer) const
  {
    if (_visible)
    {
      _renderFunction(renderer);
    }
  }

  void Sprite::Update()
  {
    bool mouseInside = GCamera.IsMouseInside(_rect);
    if (_tooltip != nullptr)
    {
      _tooltip->SetVisible(mouseInside);
    }

    if (_attached && _parent != nullptr)
    {
      _x = _parent->CenterX + CenterX + (-_width / 2);
      _y = _parent->CenterY + CenterY + (-_height / 2);
      _rect = { _x, _y, _width, _height };

      if (!_parent->GetVisible() && GetVisible())
      {
        SetVisible(_parent->_visible);
      }
    }

    if (!_hover && mouseInside)
    {
      _hover = true;
      if(_onHover) _onHover(this);
    }
    else if (_hover && !mouseInside)
    {
      _hover = false;
      if (_onHoverExit) _onHoverExit(this);
    }

    if (mouseInside && GInput.LMBPressed() && _onClick)
    {
      _onClick(this);
    }

    if (mouseInside && GInput.LMBDoubleClick() && _onDoubleClick)
    {
      _onDoubleClick(this);
    }
  }

  void Sprite::RenderSimple(SDL_Renderer* renderer) const
  {
    SDL_Rect rect = { _x , _y , static_cast<int32_t>(_scale * _width * _widthPercentage), static_cast<int32_t>(_scale * _height) };
    GCamera.Transform(rect);
    SDL_RenderCopy(renderer, _texture, nullptr, &rect);
  }

  void Sprite::SetCenterPosition(int32_t x, int32_t y)
  {
    CenterX = x;
    CenterY = y;
    if (!_attached || _parent == nullptr)
    {
      _x = CenterX + (-_width / 2);
      _y = CenterY + (-_height / 2);
    }
    else
    {
      _x = _parent->CenterX + CenterX + (-_width / 2);
      _y = _parent->CenterY + CenterY + (-_height / 2);
    }

    _rect = { _x, _y, _width, _height };
  }

  bool Sprite::IsInside(int32_t x, int32_t y)
  {
    return x >= _rect.x && x <= _rect.x + _rect.w && y >= _rect.y && y <= _rect.y + _rect.h;
  }

  void Sprite::RenderBoxSprite(SDL_Renderer* renderer) const
  {
    // Fill BG
    auto fullRect = _rect;
    GCamera.Transform(fullRect);

    SDL_RenderFillRect(renderer, &fullRect);

    // Top left corner
    SDL_Rect dest_rect = { _x , _y , _gridSize * 2, _gridSize * 2 };
    SDL_Rect src_rect = { 0, 0, _gridSize * 2, _gridSize * 2 };
    GCamera.Transform(dest_rect);
    SDL_RenderCopy(renderer, _texture, &src_rect, &dest_rect);

    // Top line
    src_rect = { _gridSize * 2, 0, _gridSize, _gridSize };
    for (int32_t i = 0; i < (_width-_gridSize*4)/_gridSize; i++)
    {
      dest_rect = { _x + _gridSize * 2 + i * _gridSize, _y, _gridSize, _gridSize };
      GCamera.Transform(dest_rect);
      SDL_RenderCopy(renderer, _texture, &src_rect, &dest_rect);
    }

    // Top right
    dest_rect = { _x + _width - _gridSize * 2, _y, _gridSize * 2, _gridSize * 2 };
    src_rect = { _gridSize * 3, 0, _gridSize * 2, _gridSize * 2 };
    GCamera.Transform(dest_rect);
    SDL_RenderCopy(renderer, _texture, &src_rect, &dest_rect);

    // Right line
    src_rect = { _gridSize * 4, _gridSize * 2, _gridSize, _gridSize };
    for (int32_t i = 0; i < (_height - _gridSize * 4) / _gridSize; i++)
    {
      dest_rect = { _x + _width - _gridSize, _y + _gridSize * 2 + i * _gridSize, _gridSize, _gridSize };
      GCamera.Transform(dest_rect);
      SDL_RenderCopy(renderer, _texture, &src_rect, &dest_rect);
    }

    // Bottom right
    dest_rect = { _x + _width - _gridSize * 2, _y + _height - _gridSize * 2, _gridSize * 2, _gridSize * 2 };
    src_rect = { _gridSize * 3, _gridSize * 3, _gridSize * 2, _gridSize * 2 };
    GCamera.Transform(dest_rect);
    SDL_RenderCopy(renderer, _texture, &src_rect, &dest_rect);

    // Bottom line
    src_rect = { _gridSize * 2, _gridSize * 4, _gridSize, _gridSize };
    for (int32_t i = 0; i < (_width - _gridSize * 4) / _gridSize; i++)
    {
      dest_rect = { _x + _gridSize * 2 + i * _gridSize, _y + _height - _gridSize, _gridSize, _gridSize };
      GCamera.Transform(dest_rect);
      SDL_RenderCopy(renderer, _texture, &src_rect, &dest_rect);
    }

    // Bottom left
    dest_rect = { _x, _y + _height - _gridSize * 2, _gridSize * 2, _gridSize * 2 };
    src_rect = { 0, _gridSize * 3, _gridSize * 2, _gridSize * 2 };
    GCamera.Transform(dest_rect);
    SDL_RenderCopy(renderer, _texture, &src_rect, &dest_rect);

    // Left line
    src_rect = { 0, _gridSize * 2, _gridSize, _gridSize };
    for (int32_t i = 0; i < (_height - _gridSize * 4) / _gridSize; i++)
    {
      dest_rect = { _x, _y + _gridSize * 2 + i * _gridSize, _gridSize, _gridSize };
      GCamera.Transform(dest_rect);
      SDL_RenderCopy(renderer, _texture, &src_rect, &dest_rect);
    }
  }

  void Sprite::RenderTextBox(SDL_Renderer* renderer) const
  {
    SDL_Rect rect = { _x , _y , _width, _height };
    GCamera.Transform(rect);
    FC_DrawBox(_font, renderer, rect, _text.c_str(), "");
    //FC_Draw(_font, renderer, rect.x, rect.y, _text.c_str());
    //FC_GetWidth()
  }

  void Sprite::RegisterOnHover(const std::function<void(Sprite*)>& onHover)
  {
    _onHover = onHover;
  }

  void Sprite::RegisterOnHoverExit(const std::function<void(Sprite*)>& onHoverExit)
  {
    _onHoverExit = onHoverExit;
  }

  void Sprite::RegisterOnClick(const std::function<void(Sprite*)>& onClick)
  {
    _onClick = onClick;
  }

  void Sprite::RegisterOnDoubleClick(const std::function<void(Sprite*)>& onDoubleClick)
  {
    _onDoubleClick = onDoubleClick;
  }
}