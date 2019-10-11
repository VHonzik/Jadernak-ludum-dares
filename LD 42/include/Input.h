#pragma once

#include <SDL.h>
#include <unordered_map>

namespace CherEngine
{
  enum KeyState
  {
    kKeyState_Up,
    kKeyState_Pressed,
    kKeyState_Down
  };

  class Input
  {
  public:
    Input();
    void ProcessMessage(const SDL_Event& event);
    void Update();

    bool KeyDown(SDL_Keycode key);
    bool KeyPressed(SDL_Keycode key);

    bool LMBDown();
    bool LMBPressed();
    bool LMBDoubleClick();

    int32_t MouseX;
    int32_t MouseY;

  private:
    std::unordered_map<SDL_Keycode, KeyState> _keyStates;
    KeyState _leftMouseButton;
    KeyState _rightMouseButton;
    float _leftMBDoubleClickTimer;
    bool _leftMBDoubleClick;
  };

  extern Input GInput;
}

