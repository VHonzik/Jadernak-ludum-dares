#pragma once

#include <SDL.h>

#include <vector>
#include <memory>

namespace LD43
{
  struct Texture;

  struct SpriteParams
  {
    bool persistant;
    std::string textureName;
    std::shared_ptr<Texture> texture;
    int32_t z;
  };

  class Sprite
  {
  public:
    Sprite(const SpriteParams& params);
    virtual void DoRender(SDL_Renderer* renderer);
    virtual bool DoPreload(SDL_Renderer* renderer) { return true; };
    virtual void Update() {};
    virtual bool RequiresPreload() const { return !_preloaded; };
    virtual void Clean();
    virtual void Tint(const SDL_Color& tintColor);

    virtual void SetAlpha(float alpha);
    virtual float GetAlpha() const;

    virtual const SDL_Rect& GetBoundingBox() const;
    virtual bool HitTest(uint32_t x, uint32_t y) const;
    virtual bool HasHitTest() const;

    void Preload(SDL_Renderer* renderer);
    void Render(SDL_Renderer* renderer);

    void Show(bool shown);
    bool IsShown() const { return _shown; }

    void SetPosition(uint32_t x, uint32_t y);
    void SetCenterPosition(uint32_t x, uint32_t y);

    int32_t GetX() const { return _screenTransform.x; }
    int32_t GetY() const { return _screenTransform.y; }

    int32_t GetCenterX() const { return _screenTransform.x + _screenTransform.w / 2; }
    int32_t GetCenterY() const { return _screenTransform.y + _screenTransform.h / 2; }

    int32_t GetWidth() const { return _screenTransform.w; }
    int32_t GetHeight() const { return _screenTransform.h; }

    virtual void SetWidth(int32_t width);
    virtual void SetHeight(int32_t height);

    void SetMask(const SDL_Rect& mask);

    int GetZ() const { return _z; }
    const SDL_Rect& GetScreenTransform() const { return _screenTransform; }
    const std::string& GetTextureName() const;
    const std::shared_ptr<Texture>& GetTextureDescription() const { return _textureDescription; }

  protected:
    std::shared_ptr<Texture> _textureDescription;

    SDL_Rect _screenTransform;
    SDL_Rect _boundingBox;

    SDL_Texture* _texture;

    bool _masked;
    SDL_Rect _mask;

    bool _preloaded;
    bool _shown;

    int _z;
    float _alpha;
  };
}
