#pragma once

#include "FTC.h"

namespace LD45
{
  struct Style
  {
    uint32_t nativeResolutionWidth;
    uint32_t nativeResolutionHeight;

    int32_t maxResolutionFraction;

    std::string windowName;

    FTCParams fpsStyle;

    std::vector<uint32_t> fontSizes;

    SDL_Color backgroundColor;

    std::vector<std::tuple<std::string, std::string, bool>> textures;
    std::vector<std::pair<std::string, std::string>> fonts;
    std::vector<std::pair<std::string, std::string>> sounds;
    std::vector<std::tuple<std::string, std::string, int32_t, int32_t>> cursors;
  };
}
