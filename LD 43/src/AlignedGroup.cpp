#include "AlignedGroup.h"

#include "Button.h"
#include "Slider.h"
#include "BoxSprite.h"

#include <cassert>
#include <numeric>
#include <algorithm>

namespace LD43
{
  AlignedGroup::AlignedGroup()
    : AlignedGroup(kGroupDirection_Vertical,
      kHorizontalAlignment_Center, kVerticalAlignment_Center,
      0, 0, 0)
  {
  }

  template<>
  void AlignedGroup::AddRecurse<Button>(Button* element)
  {
    _elements.push_back(std::make_pair(static_cast<void*>(element), kGroupElementType_Button));
  }

  template<>
  void AlignedGroup::AddRecurse<Slider>(Slider* element)
  {
    _elements.push_back(std::make_pair(static_cast<void*>(element), kGroupElementType_Slider));
  }

  template<>
  void AlignedGroup::AddRecurse<BoxSprite>(BoxSprite* element)
  {
    _elements.push_back(std::make_pair(static_cast<void*>(element), kGroupElementType_BoxSprite));
  }

  AlignedGroup::AlignedGroup(const GroupDirection direction,
    const HorizontalAlignment horizontalAlignment, const VerticalAlignment verticalAlignment,
    const int32_t spacing)
    : AlignedGroup(direction, horizontalAlignment, verticalAlignment,
      spacing, 0, 0)
  {
  }

  AlignedGroup::AlignedGroup(const GroupDirection direction,
    const HorizontalAlignment horizontalAlignment, const VerticalAlignment verticalAlignment,
    const int32_t spacing, const int32_t x, const int32_t y)
    : _horizontalAlignment(horizontalAlignment)
    , _verticalAlignment(verticalAlignment)
    , _spacing(spacing)
    , _height(0)
    , _width(0)
    , _x(x)
    , _y(y)
    , _direction(direction)
  {
  }

  void AlignedGroup::SetParameters(const HorizontalAlignment horizontalAlignment,
    const VerticalAlignment verticalAlignment, const int32_t spacing)
  {
    _horizontalAlignment = horizontalAlignment;
    _verticalAlignment = verticalAlignment;
    _spacing = spacing;
    Align();
  }

  void AlignedGroup::SetPosition(const int32_t x, const int32_t y)
  {
    _x = x;
    _y = y;
    Align();
  }

  int32_t AlignedGroup::GetElementHeight(const GroupElement& element)
  {
    switch (element.second)
    {
    case kGroupElementType_Button:
      return static_cast<Button*>(element.first)->GetHeight();
    case kGroupElementType_Slider:
      return static_cast<Slider*>(element.first)->GetHeight();
    case kGroupElementType_BoxSprite:
      return static_cast<BoxSprite*>(element.first)->GetHeight();
    default:
      assert(false); // Unknown type in group
      return 0;
    }
  }

  int32_t AlignedGroup::GetElementWidth(const GroupElement& element)
  {
    switch (element.second)
    {
    case kGroupElementType_Button:
      return static_cast<Button*>(element.first)->GetWidth();
    case kGroupElementType_Slider:
      return static_cast<Slider*>(element.first)->GetWidth();
    case kGroupElementType_BoxSprite:
      return static_cast<BoxSprite*>(element.first)->GetWidth();
    default:
      assert(false); // Unknown type in group
      return 0;
    }
  }

  int32_t AlignedGroup::GetElementX(const GroupElement& element)
  {
    switch (element.second)
    {
    case kGroupElementType_Button:
      return static_cast<Button*>(element.first)->GetX();
    case kGroupElementType_Slider:
      return static_cast<Slider*>(element.first)->GetX();
    case kGroupElementType_BoxSprite:
      return static_cast<BoxSprite*>(element.first)->GetX();
    default:
      assert(false); // Unknown type in group
      return 0;
    }
  }


  int32_t AlignedGroup::GetElementY(const GroupElement& element)
  {
    switch (element.second)
    {
    case kGroupElementType_Button:
      return static_cast<Button*>(element.first)->GetY();
    case kGroupElementType_Slider:
      return static_cast<Slider*>(element.first)->GetY();
    case kGroupElementType_BoxSprite:
      return static_cast<BoxSprite*>(element.first)->GetY();
    default:
      assert(false); // Unknown type in group
      return 0;
    }
  }

  int32_t AlignedGroup::GetElementCenterX(const GroupElement& element)
  {
    switch (element.second)
    {
    case kGroupElementType_Button:
      return static_cast<Button*>(element.first)->GetCenterX();
    case kGroupElementType_Slider:
      return static_cast<Slider*>(element.first)->GetCenterX();
    case kGroupElementType_BoxSprite:
      return static_cast<BoxSprite*>(element.first)->GetCenterX();
    default:
      assert(false); // Unknown type in group
      return 0;
    }
  }

  int32_t AlignedGroup::GetElementCenterY(const GroupElement& element)
  {
    switch (element.second)
    {
    case kGroupElementType_Button:
      return static_cast<Button*>(element.first)->GetCenterY();
    case kGroupElementType_Slider:
      return static_cast<Slider*>(element.first)->GetCenterY();
    case kGroupElementType_BoxSprite:
      return static_cast<BoxSprite*>(element.first)->GetCenterY();
    default:
      assert(false); // Unknown type in group
      return 0;
    }
  }

  void AlignedGroup::SetElementPosition(GroupElement& element, int32_t x, int32_t y)
  {
    switch (element.second)
    {
    case kGroupElementType_Button:
      return static_cast<Button*>(element.first)->SetPosition(x, y);
    case kGroupElementType_Slider:
      return static_cast<Slider*>(element.first)->SetPosition(x, y);
    case kGroupElementType_BoxSprite:
      return static_cast<BoxSprite*>(element.first)->SetPosition(x, y);
    default:
      assert(false); // Unknown type in group
      break;
    }
  }

  void AlignedGroup::SetElementCenterPosition(GroupElement& element, int32_t x, int32_t y)
  {
    switch (element.second)
    {
    case kGroupElementType_Button:
      return static_cast<Button*>(element.first)->SetCenterPosition(x, y);
    case kGroupElementType_Slider:
      return static_cast<Slider*>(element.first)->SetCenterPosition(x, y);
    case kGroupElementType_BoxSprite:
      return static_cast<BoxSprite*>(element.first)->SetCenterPosition(x, y);
    default:
      assert(false); // Unknown type in group
      break;
    }
  }

  void AlignedGroup::CalculateDimensions()
  {
    switch (_direction)
    {
    case kGroupDirection_Vertical:
    {
      _height = std::accumulate(std::cbegin(_elements), std::cend(_elements), 0,
        [](const int32_t& partial, const GroupElement& next)
      {
        return partial + GetElementHeight(next);
      });

      _height += std::max(0, static_cast<int32_t>(_elements.size() - 1)) * _spacing;

      if (_elements.size())
      {
        _width = GetElementWidth(*std::max_element(std::cbegin(_elements), std::cend(_elements),
          [](const GroupElement& a, const GroupElement& b)
        {
          return GetElementWidth(a) < GetElementWidth(b);
        }));
      }
      else
      {
        _width = 0;
      }

    }
    break;
    case kGroupDirection_Horizontal:
    {
      _width = std::accumulate(std::cbegin(_elements), std::cend(_elements), 0,
        [](const int32_t& partial, const GroupElement& next)
      {
        return partial + GetElementWidth(next);
      });

      _height += std::max(0, static_cast<int32_t>(_elements.size() - 1)) * _spacing;

      if (_elements.size())
      {
        _height = GetElementHeight(*std::max_element(std::cbegin(_elements), std::cend(_elements),
          [](const GroupElement& a, const GroupElement& b)
        {
          return GetElementHeight(a) < GetElementHeight(b);
        }));
      }
      else
      {
        _height = 0;
      }
    }
    default:
      assert(false); // Unknown direction type
      break;
    }
  }

  void AlignedGroup::AlignVerticalX()
  {
    for (auto& element : _elements)
    {
      switch (_horizontalAlignment)
      {
      case kHorizontalAlignment_Left:
        SetElementPosition(element, _x, GetElementY(element));
        break;
      case kHorizontalAlignment_Center:
        SetElementCenterPosition(element, _x, GetElementCenterY(element));
        break;
      case kHorizontalAlignment_Right:
        SetElementPosition(element, _x - GetElementWidth(element), GetElementY(element));
        break;
      default:
        assert(false); // Unknown horizontal alignment
        break;
      }
    }
  }

  void AlignedGroup::AlignVerticalY()
  {
    int32_t currentY;
    switch (_verticalAlignment)
    {
    case kVerticalAlignment_Center:
      currentY = _y - _height / 2;
      break;
    case kVerticalAlignment_Bottom:
      currentY = _y - _height;
      break;
    case kVerticalAlignment_Top:
      currentY = _y;
      break;
    default:
      assert(false); // Unknown vertical alignment
      break;
    }

    for (auto& element : _elements)
    {
      SetElementPosition(element, GetElementX(element), currentY);
      currentY += GetElementHeight(element) + _spacing;
    }
  }

  void AlignedGroup::AlignHorizontalX()
  {
    int32_t currentX;
    switch (_horizontalAlignment)
    {
    case kHorizontalAlignment_Left:
      currentX = _x;
    case kHorizontalAlignment_Center:
      currentX = _x - _width / 2;
      break;
    case kHorizontalAlignment_Right:
      currentX = _x - _width;
      break;
    default:
      assert(false); // Unknown horizontal alignment
      break;
    }

    for (auto& element : _elements)
    {
      SetElementPosition(element, currentX, GetElementY(element));
      currentX += GetElementWidth(element) + _spacing;
    }
  }

  void AlignedGroup::AlignHorizontalY()
  {
    for (auto& element : _elements)
    {
      switch (_verticalAlignment)
      {
      case kVerticalAlignment_Top:
        SetElementPosition(element, GetElementX(element), _y);
        break;
      case kVerticalAlignment_Center:
        SetElementCenterPosition(element, GetElementCenterX(element), _y);
        break;
      case kVerticalAlignment_Bottom:
        SetElementPosition(element, GetElementX(element), _y - GetElementHeight(element));
        break;
      default:
        assert(false); // Unknown vertical alignment
        break;
      }
    }
  }


  void AlignedGroup::Align()
  {
    CalculateDimensions();
    switch (_direction)
    {
    case LD43::kGroupDirection_Vertical:
      AlignVerticalX();
      AlignVerticalY();
      break;
    case LD43::kGroupDirection_Horizontal:
      AlignHorizontalX();
      AlignHorizontalY();
      break;
    default:
      assert(false); // Unknown direction
      break;
    }

  }
}