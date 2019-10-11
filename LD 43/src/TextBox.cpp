#include "TextBox.h"

namespace LD43
{
  TextBox::TextBox(TTF_Font* font, const std::string& text,
    uint32_t width, SDL_Color color)
    : _font(font)
    , _color(color)
    , _width(width)
    , _height(0)
    , _text(text)
    , _cachedTexture(nullptr)
    , _x(0)
    , _y(0)
  {
  }

  bool TextBox::DoPreload(SDL_Renderer* renderer)
  {
    if (_text.size() == 0)
    {
      _width = 0;
      _height = 0;
      return true;
    }

    auto surface = TTF_RenderText_Blended_Wrapped(_font, _text.c_str(), _color, _width);

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
      _height = 0;
      return false;
    }
  }

  void TextBox::DoRender(SDL_Renderer* renderer)
  {
    if (_cachedTexture != nullptr)
    {
      SDL_Rect destination = { _x , _y , _width, _height };
      SDL_RenderCopy(renderer, _cachedTexture, nullptr, &destination);
    }
  }

  void TextBox::Clean()
  {
    RemoveCache();
  }

  void TextBox::RemoveCache()
  {
    SDL_DestroyTexture(_cachedTexture);
    _cachedTexture = nullptr;
    _preloaded = false;
  }

  void TextBox::SetPosition(int32_t x, int32_t y)
  {
    _x = x;
    _y = y;
  }

  void TextBox::SetText(const std::string& text)
  {
    _text = text;
    RemoveCache();
  }

}
