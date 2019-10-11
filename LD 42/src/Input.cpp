#include "Input.h"
#include "Time.h"

namespace CherEngine
{
  Input GInput;

  Input::Input()
    : _leftMouseButton(kKeyState_Up)
    , _rightMouseButton(kKeyState_Up)
    , _leftMBDoubleClickTimer(-1.0f)
    , _leftMBDoubleClick(false)
  {
  }

  void Input::ProcessMessage(const SDL_Event& event)
  {
    if (event.type == SDL_KEYDOWN)
    {
      const auto key = event.key.keysym.sym;
      auto found = _keyStates.find(key);
      if (found == _keyStates.end())
      {
        _keyStates[key] = kKeyState_Pressed;
      }
      else
      {
        if (found->second == kKeyState_Pressed)
        {
          found->second = kKeyState_Down;
        }
        else if (found->second == kKeyState_Up)
        {
          _keyStates[key] = kKeyState_Pressed;
        }
      }
    }
    else if (event.type == SDL_KEYUP)
    {
      const auto key = event.key.keysym.sym;
      _keyStates[key] = kKeyState_Up;
    }
    else if (event.type == SDL_MOUSEMOTION)
    {
      SDL_GetMouseState(&MouseX, &MouseY);
    }
    else if (event.type == SDL_MOUSEBUTTONDOWN)
    {
      if (event.button.button == SDL_BUTTON_LEFT && _leftMouseButton == kKeyState_Up)
      {
        _leftMouseButton = kKeyState_Pressed;
        if (_leftMBDoubleClickTimer < 0.0f)
        {
          _leftMBDoubleClickTimer = 0.5f;
        }
        else
        {
          _leftMBDoubleClick = true;
          _leftMBDoubleClickTimer = -1.0f;
        }

      }
      else if (event.button.button == SDL_BUTTON_RIGHT && _rightMouseButton == kKeyState_Up)
      {
        _rightMouseButton = kKeyState_Pressed;
      }
    }
    else if (event.type == SDL_MOUSEBUTTONUP)
    {
      if (event.button.button == SDL_BUTTON_LEFT)
      {
        _leftMouseButton = kKeyState_Up;
      }
      else if (event.button.button == SDL_BUTTON_RIGHT)
      {
        _rightMouseButton = kKeyState_Up;
      }
    }
  }

  bool Input::KeyDown(SDL_Keycode key)
  {
    auto found = _keyStates.find(key);
    if (found == _keyStates.end())
    {
      _keyStates[key] = kKeyState_Up;
      return false;
    }
    else
    {
      return found->second > kKeyState_Up;
    }
  }

  bool Input::KeyPressed(SDL_Keycode key)
  {
    auto found = _keyStates.find(key);
    if (found == _keyStates.end())
    {
      _keyStates[key] = kKeyState_Up;
      return false;
    }
    else
    {
      return found->second == kKeyState_Pressed;
    }
  }

  void Input::Update()
  {
    for (auto& keyState : _keyStates)
    {
      if (keyState.second == kKeyState_Pressed)
      {
        keyState.second = kKeyState_Down;
      }
    }

    if (_leftMBDoubleClickTimer > 0.0f)
    {
      _leftMBDoubleClickTimer -= GTime.deltaTime;
    }

    if (_leftMouseButton == kKeyState_Pressed)
    {
      _leftMouseButton = kKeyState_Down;
    }

    if (_rightMouseButton == kKeyState_Pressed)
    {
      _rightMouseButton = kKeyState_Down;
    }

    if (_leftMBDoubleClick)
    {
      _leftMBDoubleClick = false;
    }
  }

  bool Input::LMBDown()
  {
    return _leftMouseButton > kKeyState_Up;
  }

  bool Input::LMBPressed()
  {
    return _leftMouseButton == kKeyState_Pressed;
  }

  bool Input::LMBDoubleClick()
  {
    return _leftMBDoubleClick;
  }
}