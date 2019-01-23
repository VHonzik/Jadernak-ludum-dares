#pragma once

#include "SDL.h"

#include <cstdarg>
#include <algorithm>
#include <cassert>

namespace LD43
{
  inline bool IsInsideRect(int32_t x, int32_t y, const SDL_Rect& rect)
  {
    return x >= rect.x && x <= rect.x + rect.w && y >= rect.y && y <= rect.y + rect.h;
  }

  template<typename T>
  int Sign(const T& val)
  {
    return (T{0} < val) - (val < T{ 0 });
  }

  inline int ClosestNumberToNDivisibleByM(int32_t n, int32_t m)
  {
    // find the quotient
    int32_t q = n / m;

    // 1st possible closest number
    int32_t n1 = m * q;

    // 2nd possible closest number
    int32_t n2 = (n * m) > 0 ? (m * (q + 1)) : (m * (q - 1));

    // if true, then n1 is the required closest number
    if (abs(n - n1) < abs(n - n2))
    {
      return n1;
    }

    // else n2 is the required closest number
    return n2;
  }

  inline void Log(const char* format, ...)
  {
    va_list args;
    va_start(args, format);

    char buffer[128];
    std::string newLineFormat = format;
    newLineFormat += '\n';
    std::snprintf(buffer, 128, newLineFormat.c_str(), args);
    //OutputDebugString(buffer);
  }

  template<typename T>
  inline T Clamp(const T& value, const T& minimum, const T& maximum)
  {
    return (std::min)(maximum, (std::max)(value, minimum));
  }

  template<typename T>
  inline T Clamp01(const T& value)
  {
    return Clamp(value, T{ 0 }, T{ 1 });
  }

  template<typename T>
  inline T MoveTowards(const T& current, const T& wanted, const T& maxChange)
  {
    assert(maxChange >= T{0});

    const auto diff = std::abs(current - wanted);
    if (diff <= std::abs(maxChange))
    {
      return wanted;
    }
    else
    {
      return current + Sign(wanted - current) * maxChange;
    }
  }

  template<typename T>
  inline T Interpolate(const T& start, const T& end, const float t)
  {
    return static_cast<T>(start + (end - start) * Clamp01(t));
  }
}
