#pragma once

#include <SDL.h>
#include <unordered_map>

namespace LD45
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
    void AfterUpdate();

    bool KeyDown(SDL_Keycode key);
    bool KeyPressed(SDL_Keycode key);

    bool MouseButtonDown(int32_t key);
    bool MouseButtonPressed(int32_t key);

    SDL_Keycode FirstKeyPressed() const;
    std::string GetKeyName(SDL_Keycode key) const;

    int32_t GetMouseX() const;
    int32_t GetMouseY() const;

    int32_t GetLastMouseX() const;
    int32_t GetLastMouseY() const;

    int32_t GetMouseWheelY() const { return _mouseWheelY; }

  private:
    std::unordered_map<SDL_Keycode, KeyState> _keyStates;
    std::unordered_map<int32_t, KeyState> _mouseButtonStates;

    int32_t _lastMouseX;
    int32_t _lastMouseY;
    int32_t _mouseX;
    int32_t _mouseY;

    int32_t _mouseWheelY;

    SDL_Keycode _firstKeyPressed;
  };

  extern Input GInput;
}

