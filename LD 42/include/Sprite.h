#pragma once

#include <SDL.h>
#include <SDL_FontCache.h>
#include <functional>

namespace CherEngine
{
  class Sprite
  {
  protected:
    SDL_Texture* _texture;
    FC_Font* _font;

    std::string _text;

    SDL_Rect _rect;

    int32_t _width;
    int32_t _height;

    int32_t _x;
    int32_t _y;

    int32_t _gridSize;

    float _alpha;
    float _scale;
    float _widthPercentage;

    bool _visible;

    bool _autoWidth;

    bool _attached;
    Sprite* _parent;

    Sprite* _tooltip;

    std::function<void(SDL_Renderer*)> _renderFunction;

    bool _hover;
    std::function<void(Sprite*)> _onHover;
    std::function<void(Sprite*)> _onHoverExit;
    std::function<void(Sprite*)> _onClick;
    std::function<void(Sprite*)> _onDoubleClick;

  public:
    Sprite();
    Sprite(SDL_Texture* texture, int32_t width, int32_t height);
    Sprite(SDL_Texture* texture, int32_t width, int32_t height, int32_t gridSize);
    Sprite(FC_Font* texture, int32_t width, int32_t height, const char* text);

    int32_t CenterX;
    int32_t CenterY;

    void SetAsSimpleSprite();
    void SetAsBoxSprite();
    void SetAsTextBox();

    void AttachTo(Sprite* parent);
    void AddTooltip(Sprite& tooltip);

    void SetCenterPosition(int32_t x, int32_t y);
    void SetVisible(bool visible);
    bool GetVisible() const { return _visible; }
    void SetText(const char* text);
    void SetScale(float scale);
    void SetWidthPercentage(float widthPc);
    void SetAlpha(float alpha);

    bool IsInside(int32_t x, int32_t y);

    void Update();
    void Render(SDL_Renderer* renderer) const;

    void RenderSimple(SDL_Renderer* renderer) const;
    void RenderBoxSprite(SDL_Renderer* renderer) const;
    void RenderTextBox(SDL_Renderer* renderer) const;

    void RegisterOnHover(const std::function<void(Sprite*)>& onHover);
    void RegisterOnHoverExit(const std::function<void(Sprite*)>& onHoverExit);
    void RegisterOnClick(const std::function<void(Sprite*)>& onClick);
    void RegisterOnDoubleClick(const std::function<void(Sprite*)>& onDoubleClick);
  };
}
