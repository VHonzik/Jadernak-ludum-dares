#pragma once

#include <vector>
#include <SDL.h>

namespace LD43
{
  struct Texture
  {
    Texture(SDL_Texture* itexture,
    int32_t iwidth,
    int32_t iheight,
    const SDL_Rect& iboundingBox,
    const std::vector<bool>& ihitArray,
    const std::string& iname,
    uint32_t iformat,
    bool iisCopy)
      : texture(itexture)
      , width(iwidth)
      , height(iheight)
      , boundingBox(iboundingBox)
      , hitArray(ihitArray)
      , name(iname)
      , format(iformat)
      , isCopy(iisCopy)
    {}

    Texture(const Texture& other)
      : texture(other.texture)
      , width(other.width)
      , height(other.height)
      , boundingBox(other.boundingBox)
      , hitArray(other.hitArray)
      , name(other.name)
      , format(other.format)
      , isCopy(other.isCopy)
    {}

    SDL_Texture* texture;
    int32_t width;
    int32_t height;
    SDL_Rect boundingBox;
    std::vector<bool> hitArray;
    std::string name;
    uint32_t format;
    bool isCopy;
  };
}



