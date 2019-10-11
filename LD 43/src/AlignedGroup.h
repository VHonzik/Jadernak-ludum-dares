#pragma once

#include "Aligment.h"

#include <cstdint>
#include <vector>

namespace LD43
{
  enum GroupDirection
  {
    kGroupDirection_Vertical,
    kGroupDirection_Horizontal
  };

  enum GroupElementType
  {
    kGroupElementType_Button,
    kGroupElementType_Slider,
    kGroupElementType_BoxSprite,
  };

  class AlignedGroup
  {
  public:
    AlignedGroup();
    AlignedGroup(const GroupDirection direction,
      const HorizontalAlignment horizontalAlignment, const VerticalAlignment verticalAlignment,
      const int32_t spacing);
    AlignedGroup(const GroupDirection direction,
      const HorizontalAlignment horizontalAlignment,const VerticalAlignment verticalAlignment,
      const int32_t spacing, const int32_t x, const int32_t y);

    template<typename... Args>
    void AddVA(Args... args)
    {
      AddRecurse(args...);
      Align();
    }

    template<class InputIt>
    void Add(InputIt first, InputIt last)
    {
      for (auto it = first; it != last; ++it)
      {
        AddRecurse(*it);
      }
      Align();
    }

    void Align();

    void SetParameters(const HorizontalAlignment horizontalAlignment,
      const VerticalAlignment verticalAlignment, const int32_t spacing);
    void SetPosition(const int32_t x, const int32_t y);

    using GroupElement = std::pair<void*, GroupElementType>;

  private:
    template<typename T>
    void AddRecurse(T* element);

    template<typename Type, typename... Args>
    void AddRecurse(Type* first, Args... args)
    {
      AddRecurse(first);
      AddRecurse(args...);
    }

    void CalculateDimensions();
    void AlignVerticalX();
    void AlignVerticalY();

    void AlignHorizontalX();
    void AlignHorizontalY();

    static int32_t GetElementHeight(const GroupElement& element);
    static int32_t GetElementWidth(const GroupElement& element);
    static int32_t GetElementY(const GroupElement& element);
    static int32_t GetElementX(const GroupElement& element);
    static int32_t GetElementCenterX(const GroupElement& element);
    static int32_t GetElementCenterY(const GroupElement& element);
    static void SetElementPosition(GroupElement& element, int32_t x, int32_t y);
    static void SetElementCenterPosition(GroupElement& element, int32_t x, int32_t y);

    GroupDirection _direction;
    int32_t _spacing;
    HorizontalAlignment _horizontalAlignment;
    VerticalAlignment _verticalAlignment;
    std::vector<GroupElement> _elements;

    int32_t _width;
    int32_t _height;

    int32_t _x;
    int32_t _y;
  };
}
