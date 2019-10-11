#pragma once

#include <SDL_ttf.h>
#include "ITextObject.h"

namespace LD43
{
  class TextBox : public ITextObject
  {
  public:
    TextBox(TTF_Font* font, const std::string& text,
      uint32_t width, SDL_Color color);

    bool DoPreload(SDL_Renderer* renderer) override;
    void DoRender(SDL_Renderer* renderer) override;

    void Clean() override;

    void SetPosition(int32_t x, int32_t y);
    void SetText(const std::string& text);

  private:
    void RemoveCache();

    TTF_Font* _font;
    SDL_Color _color;

    std::string _text;

    int32_t _x;
    int32_t _y;
    int32_t _width;
    int32_t _height;

    SDL_Texture* _cachedTexture;
  };
}
