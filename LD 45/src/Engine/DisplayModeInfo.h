#pragma once

#include <SDL.h>

#include <cstdint>
#include <functional>

namespace LD45
{
  struct DisplayModeInfo
  {
    int32_t width;
    int32_t height;

    int32_t displaysSupported;

    bool native;

    std::string name;

    SDL_Rect scaledRect;

    SDL_Rect windowRect;

    int32_t distanceFromNative;

    bool operator==(const DisplayModeInfo& other) const
    {
      return width == other.width && height == other.height;
    }
  };

  using DisplayModeInfoKey = size_t;
}

namespace std
{
  template<>
  struct hash<LD45::DisplayModeInfo>
  {
    size_t operator()(const LD45::DisplayModeInfo& obj) const
    {
      return static_cast<size_t>(std::pow(10,6)) * obj.width + obj.height;
    }
  };
}




