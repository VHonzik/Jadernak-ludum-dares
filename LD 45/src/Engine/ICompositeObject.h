#pragma once

namespace LD45
{
  class ICompositeObject
  {
  public:
    ICompositeObject();

    virtual void Update() {};
    virtual void Show(bool shown) { _shown = shown; }
    bool IsShown() const { return _shown; }
  protected:
    bool _shown;
  };
}
