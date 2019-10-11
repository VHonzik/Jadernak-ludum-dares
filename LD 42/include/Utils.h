#pragma once

#include <algorithm>

namespace CherEngine
{
  float Lerp(float min, float max, float t)
  {
    return min + (max - min) * t;
  }

  template <typename T> int Sign(T val)
  {
    return (T(0) < val) - (val < T(0));
  }

  float MoveTowards(float origin, float destination, float maxStep)
  {
    if (std::abs(destination - origin) > maxStep)
    {
      return origin + Sign(destination - origin) * maxStep;
    }
    else
    {
      return destination;
    }
  }
}
