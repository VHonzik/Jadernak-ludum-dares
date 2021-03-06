#include "Text.h"
#include "Game.h"

#include <cassert>

namespace LD43
{
  Text::Text(const TextParams& params)
    : _text(params.text)
    , _cachedTexture(nullptr)
    , _color(params.color)
    , _masked(false)
    , _mask{0, 0, 0, 0}
    , _x(0)
    , _y(0)
  {
    if (params.font == nullptr)
    {
      _font = GGame.FindFont(params.fontName, params.fontSize);
    }
    else
    {
      _font = params.font;
    }

    assert(_font != nullptr);

    Preload(GGame.GetRenderer());
  }

  bool Text::DoPreload(SDL_Renderer* renderer)
  {
    if (_text.size() == 0)
    {
      _width = 0;
      _height = 0;
      return true;
    }

    auto surface = TTF_RenderText_Blended(_font, _text.c_str(), _color);

    if (surface != nullptr)
    {
      if (_color.a < 255)
      {
        SDL_SetSurfaceAlphaMod(surface, _color.a);
      }

      _width = surface->w;
      _height = surface->h;
      _cachedTexture = SDL_CreateTextureFromSurface(renderer, surface);
      SDL_FreeSurface(surface);

      return _cachedTexture != nullptr;
    }
    else
    {
      _width = 0;
      _height = 0;
      return false;
    }
  }

  void Text::SetMask(const SDL_Rect& mask)
  {
    _masked = true;
    _mask = mask;
  }

  void Text::SetPosition(int32_t x, int32_t y)
  {
    _x = x;
    _y = y;
  }

  void Text::DoRender(SDL_Renderer* renderer)
  {
    if (_cachedTexture != nullptr)
    {
      SDL_Rect destination = { 0 , 0 , _width, _height };

      switch (_textHorizontalAlign)
      {
      case kHorizontalAlignment_Left:
        destination.x = _x;
        break;
      case kHorizontalAlignment_Center:
        destination.x = _x - _width / 2;
        break;
      case kHorizontalAlignment_Right:
        destination.x = _x - _width;
        break;
      }

      switch (_textVerticalAlign)
      {
      case kVerticalAlignment_Top:
        destination.y = _y;
        break;
      case kVerticalAlignment_Center:
        destination.y = _y - _height / 2;
        break;
      case kVerticalAlignment_Bottom:
        destination.y = _y - _height;
        break;
      }

      if (_masked)
      {
        SDL_Rect interesection;
        if (SDL_IntersectRect(&destination, &_mask, &interesection) != SDL_FALSE)
        {
          SDL_Rect source = { interesection.x - destination.x, interesection.y - destination.y,
            interesection.w, interesection.h };
          SDL_RenderCopy(renderer, _cachedTexture, &source, &interesection);
        }
      }
      else
      {
        SDL_RenderCopy(renderer, _cachedTexture, nullptr, &destination);
      }
    }
  }

  void Text::SetText(const std::string& text)
  {
    _text = text;
    RemoveCache();
  }

  void Text::SetTextAndColor(const std::string& text, const SDL_Color& color)
  {
    _color = color;
    _text = text;
    RemoveCache();
  }

  void Text::SetColor(const SDL_Color& color)
  {
    _color = color;
    RemoveCache();
  }

  void Text::RemoveCache()
  {
    SDL_DestroyTexture(_cachedTexture);
    _cachedTexture = nullptr;
    _preloaded = false;
  }

  void Text::Clean()
  {
    SDL_DestroyTexture(_cachedTexture);
  }
}