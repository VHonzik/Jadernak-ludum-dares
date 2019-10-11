#pragma once

#include <vector>
#include "Settings.h"
#include "SDL_keycode.h"

namespace LD45
{
  enum SettingsIDs
  {
    kSettingsIDs_musicVolume,
    kSettingsIDs_soundVolume,

    kSettingsIDs_keyBind1,
    kSettingsIDs_keyBind2,
    kSettingsIDs_keyBind3,
    kSettingsIDs_keyBind4,
    kSettingsIDs_keyBind5,
    kSettingsIDs_keyBind6,
    kSettingsIDs_keyBind7,
    kSettingsIDs_keyBind8,
    kSettingsIDs_keyBind9,
    kSettingsIDs_keyBind10,
    kSettingsIDs_keyBind11,
    kSettingsIDs_keyBind12,

    kSettingsIDs_fullScreen,
    kSettingsIDs_resolutionWidth,
    kSettingsIDs_resolutionHeight,
    kSettingsIDS_engineEnd
  };

  const std::vector<SettingsEntry> kEngineSettings =
  {
    SettingsEntry::Create(kSettingsIDs_musicVolume, 0.5f,
    "Music volume. Goes from 0.0 to 1.0, 0.0 being completely silent."),
    SettingsEntry::Create(kSettingsIDs_soundVolume, 0.5f,

    "Sounds volume. Goes from 0.0 to 1.0, 0.0 being completely silent."),
    SettingsEntry::Create(kSettingsIDs_keyBind1, SDLK_q,
    "Keybinding for slot 1. Uses internal engine key identifier, see https://wiki.libsdl.org/SDL_Keycode."),
    SettingsEntry::Create(kSettingsIDs_keyBind2, SDLK_w,
    "Keybinding for slot 2. Uses internal engine key identifier, see https://wiki.libsdl.org/SDL_Keycode."),
    SettingsEntry::Create(kSettingsIDs_keyBind3, SDLK_e,
    "Keybinding for slot 3. Uses internal engine key identifier, see https://wiki.libsdl.org/SDL_Keycode."),
    SettingsEntry::Create(kSettingsIDs_keyBind4, SDLK_r,
    "Keybinding for slot 4. Uses internal engine key identifier, see https://wiki.libsdl.org/SDL_Keycode."),
    SettingsEntry::Create(kSettingsIDs_keyBind5, SDLK_a,
    "Keybinding for slot 5. Uses internal engine key identifier, see https://wiki.libsdl.org/SDL_Keycode."),
    SettingsEntry::Create(kSettingsIDs_keyBind6, SDLK_s,
    "Keybinding for slot 6. Uses internal engine key identifier, see https://wiki.libsdl.org/SDL_Keycode."),
    SettingsEntry::Create(kSettingsIDs_keyBind7, SDLK_d,
    "Keybinding for slot 7. Uses internal engine key identifier, see https://wiki.libsdl.org/SDL_Keycode."),
    SettingsEntry::Create(kSettingsIDs_keyBind8, SDLK_f,
    "Keybinding for slot 8. Uses internal engine key identifier, see https://wiki.libsdl.org/SDL_Keycode."),
    SettingsEntry::Create(kSettingsIDs_keyBind9, SDLK_1,
    "Keybinding for slot 9. Uses internal engine key identifier, see https://wiki.libsdl.org/SDL_Keycode."),
    SettingsEntry::Create(kSettingsIDs_keyBind10, SDLK_2,
    "Keybinding for slot 10. Uses internal engine key identifier, see https://wiki.libsdl.org/SDL_Keycode."),
    SettingsEntry::Create(kSettingsIDs_keyBind11, SDLK_3,
    "Keybinding for slot 11. Uses internal engine key identifier, see https://wiki.libsdl.org/SDL_Keycode."),
    SettingsEntry::Create(kSettingsIDs_keyBind12, SDLK_4,
    "Keybinding for slot 12. Uses internal engine key identifier, see https://wiki.libsdl.org/SDL_Keycode."),
    SettingsEntry::Create(kSettingsIDs_fullScreen, false,
    "Fullscreen mode."),
    SettingsEntry::Create(kSettingsIDs_resolutionWidth, 1280,
    "Wanted resolution width. Minimum width is 800."),
    SettingsEntry::Create(kSettingsIDs_resolutionHeight, 720,
    "Wanted resolution height. Minimum height is 600."),
  };
}
