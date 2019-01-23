#include "ITextObject.h"

namespace LD43
{
  ITextObject::ITextObject()
    : _preloaded(false)
    , _shown(true)
  {
  }

  void ITextObject::Preload(SDL_Renderer* renderer)
  {
    _preloaded = DoPreload(renderer);
  }

  void ITextObject::Render(SDL_Renderer* renderer)
  {
    if (_shown)
    {
      DoRender(renderer);
    }
  }
}