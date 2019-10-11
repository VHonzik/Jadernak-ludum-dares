#pragma once

#include "Style.h"
#include "BuildHash.h"
#include "Text.h"
#include "BoxSprite.h"
#include "Button.h"
#include "Slider.h"
#include "Checkbox.h"
#include "Dropdown.h"
#include "ProgressBar.h"

namespace LD43
{
  const char kBuildSeperator = '.';

  const char kAppName[] = "LD43";
  const char kAuthor[] = "Vaclav Honzik";
  const int32_t kCopyrightYear = 2018;

  const int32_t kBuildMajorVersion = 0;
  const int32_t kBuildMinorVersion = 0;

  const char kMainMenuScene[] = "MainMenuScene";
  const char kOptionsMenuScene[] = "OptionsMenuScene";

  const SDL_Color kDarkGreyColor = { 30, 30, 30, 255 };
  const SDL_Color kBlackColor = { 0, 0, 0, 255 };
  const SDL_Color kRedColor = { 255, 0, 0, 255 };
  const SDL_Color kAzureColor = { 135, 206, 250, 255 };
  const SDL_Color kWhiteColor = { 255, 255, 255, 255 };
  const SDL_Color kLightGreyColor = { 200, 200, 200, 255 };
  const SDL_Color kLightBlueColor = { 86, 156, 205, 255 };
  const SDL_Color kOrangeColor = { 214, 157, 133, 255 };
  const SDL_Color kPurpleColor = { 189, 99, 197, 255 };
  const SDL_Color kLightGreenColor = { 78, 201, 176, 255 };
  const SDL_Color kLightRedColor = { 140, 47, 47, 255 };
  const SDL_Color kYellowColor = { 233, 213, 133, 255 };
  const SDL_Color kGoldColor = { 248, 237, 49, 255 };
  const SDL_Color kGreenColor = { 0, 147, 68, 255 };
  const SDL_Color kLightAzureColor = { 222, 246, 255, 255 };
  const SDL_Color k50GreyColor = { 153, 153, 153, 255 };

  const char kKennyFontSquare[] = "kennyfsquare";
  const char kVeraFont[] = "vera";
  const char kVeraFontBold[] = "verabold";

  const Style kEngineStyle =
  {
    //uint32_t nativeResolutionWidth;
    1280,
    //uint32_t nativeResolutionHeight;
    720,
    //int32_t maxResolutionFraction;
    8,
    //std::string windowName;
    kAppName,
    //FTCParams fpsStyle;
    {
      //bool persistant;
      true,
      //TTF_Font* font;
      nullptr,
      //std::string fontName;
      kKennyFontSquare,
      //uint32_t fontSize;
      12,
      //std::string format;
      "FPS: #",
      //SDL_Color defaultColor;
      kLightGreyColor,
    },

    //const std::vector<uint32_t> fontSizes;
    { 12, 16, 24, 32 },

    //SDL_Color backgroundColor;
    kDarkGreyColor,

    //std::vector<std::tuple<std::string, std::string, bool>> textures;
    {
      { "greyPanel", "assets/grey_panel.png", false},
      { "bluePanel", "assets/blue_panel.png", false},
      { "blueButton", "assets/blue_button09.png", false},
      { "blueButtonPressed", "assets/blue_button10.png", false},
      { "blueBorderButton", "assets/blue_button06.png", false},
      { "greyButton", "assets/grey_button10.png", false},
      { "greyButtonPressed", "assets/grey_button11.png", false},
      { "greySliderLine", "assets/grey_sliderHorizontal.png", false},
      { "greySliderLineVertical", "assets/grey_sliderVertical.png", false},
      { "greySliderEnd", "assets/grey_sliderEnd.png", false},
      { "greySliderPointer", "assets/grey_sliderDown.png", true},
      { "greyBox", "assets/grey_box.png", true},
      { "blueCheckbox", "assets/blue_boxCheckmark.png", true},
      { "dropdownField", "assets/dropdown_button.png", false},
      { "smallArrowDown", "assets/grey_arrowDownGrey.png", false},
      { "smallArrowUp", "assets/grey_arrowUpGrey.png", false},

      { "bgPlain", "assets/mars_plain.png", false},
      { "bgRock1", "assets/mars_rock1.png", false},
      { "bgRock2", "assets/mars_rock2.png", false},
      { "bgRock3", "assets/mars_rock3.png", false},
      { "trees", "assets/mars_trees.png", false},
      { "selectWhite", "assets/mars_select.png", false},
      { "blueCrystals", "assets/mars_bluecrystal.png", false},
      { "landingPlatform", "assets/mars-landing.png", false},
      { "spaceCraft", "assets/spacecraft.png", false},
      { "livingQuarters", "assets/scifi_living.png", false},
      { "woodResource", "assets/treeAlien_large.png", false},
      { "crystalResource", "assets/crystals1.png", false},
      { "sawMill", "assets/scifi_base.png", false},
    },
    //std::vector<std::pair<std::string, std::string>> fonts;
    {
      {kKennyFontSquare, "assets/KenneyPixelSquare.ttf"},
      {kVeraFontBold, "assets/VeraBd.ttf"},
      {kVeraFont, "assets/Vera.ttf" },
    },
    //std::vector<std::pair<std::string, std::string>> sounds;
    {
      { "uiBeep", "assets/UIBeepDoubleQuickDeepMuffledstereo.wav"},
      { "uiClick", "assets/UIClickDistinctShortmono.wav"}
    },
    //std::vector<std::pair<std::string, std::string>> cursors;
    {
      { "cursorGauntlet", "assets/cursorGauntlet.png", 9, 9 },
    }
  };

  const TextParams kVera16LightGrey =
  {
    //bool persistant;
    false,
    //TTF_Font* font;
    nullptr,
    //std::string fontName;
    kVeraFont,
    //uint32_t fontSize;
    16,
    //std::string text;
    "",
    //SDL_Color color;
    kLightGreyColor
  };

  const TextParams kVera1650Grey =
  {
    //bool persistant;
    false,
    //TTF_Font* font;
    nullptr,
    //std::string fontName;
    kVeraFont,
    //uint32_t fontSize;
    16,
    //std::string text;
    "",
    //SDL_Color color;
    k50GreyColor
  };

  const TextParams kVeraBold1650Grey =
  {
    //bool persistant;
    false,
    //TTF_Font* font;
    nullptr,
    //std::string fontName;
    kVeraFontBold,
    //uint32_t fontSize;
    16,
    //std::string text;
    "",
    //SDL_Color color;
    k50GreyColor
  };

  const TextParams kVeraBold1250Grey =
  {
    //bool persistant;
    false,
    //TTF_Font* font;
    nullptr,
    //std::string fontName;
    kVeraFontBold,
    //uint32_t fontSize;
    12,
    //std::string text;
    "",
    //SDL_Color color;
    k50GreyColor
  };

  const ButtonParams kMainMenuBlueButton =
  {
    //bool persistant;
    false,
    //std::string textureNormal;
    "blueButton",
    //std::string texturePressed;
    "blueButtonPressed",
    //std::string textureDisabled;
    "greyButtonPressed",

    //int32_t width;
    250,
    //int32_t height;
    50,
    //int32_t z;
    1,
    //int32_t cornerSize;
    9,

    //std::string text;
    "",
    //std::string fontName;
    kVeraFontBold,
    //uint32_t fontSize;
    32,
    //SDL_Color textColor;
    kLightAzureColor,
    //SDL_Color disabledTextColor;
    kLightGreyColor,

    //std::string clickSound;
    "uiClick",
  };

  const BoxSpriteParams kOptionsGreyPanel =
  {
    {
      //bool persistant;
      false,
      //std::string textureName;
      "greyPanel",
      //std::shared_ptr<Texture> texture;
      nullptr,
      //int32_t z;
      0,
    },
    //int32_t cornerSize;
    7,
    //int32_t width;
    510,
    //int32_t height;
    400,
  };

  const ButtonParams kOptionsSectionTitleButton =
  {
    //bool persistant;
    false,
    //std::string textureNormal;
    "greyPanel",
    //std::string texturePressed;
    "greyPanel",
    //std::string textureDisabled;
    "bluePanel",

    //int32_t width;
    170,
    //int32_t height;
    50,
    //int32_t z;
    2,
    //int32_t cornerSize;
    7,

    //std::string text;
    "",
    //std::string fontName;
    kVeraFontBold,
    //uint32_t fontSize;
    24,
    //SDL_Color textColor;
    k50GreyColor,
    //SDL_Color disabledTextColor;
    kLightAzureColor,

    //std::string clickSound;
    "uiClick",
  };

  const SliderParams kOptionsSlider =
  {
    //bool persistant;
    false,
    //std::string font;
    kVeraFontBold,
    //uint32_t fontSize;
    16,
    //SDL_Color fontColor;
    k50GreyColor,
    //std::string minTitle;
    "0",
    //std::string maxTitle;
    "100",
    //int32_t width;
    300,
    //int32_t minMaxYMargin;
    20,
    //std::string axisTexture;
    "greySliderLine",
    //std::string axisEndTexture;
    "greySliderEnd",
    //std::string pointerTexture;
    "greySliderPointer",
    //int32_t z;
    1,
    //float initialValue;
    0,
  };

  const ButtonParams kOptionsKeybindButton =
  {
    //bool persistant;
    false,
    //std::string textureNormal;
    "greyButton",
    //std::string texturePressed;
    "greyButtonPressed",
    //std::string textureDisabled;
    "blueBorderButton",

    //int32_t width;
    120,
    //int32_t height;
    50,
    //int32_t z;
    1,
    //int32_t cornerSize;
    9,

    //std::string text;
    "",
    //std::string fontName;
    kVeraFontBold,
    //uint32_t fontSize;
    12,
    //SDL_Color textColor;
    k50GreyColor,
    //SDL_Color disabledTextColor;
    k50GreyColor,

    //std::string clickSound;
    "uiClick",
  };

  const CheckboxParams kBlueCheckbox =
  {
    //bool persistant;
    false,
    //std::string checkedTexture;
    "blueCheckbox",
    //std::string emptyTexture;
    "greyBox",
    //int32_t z;
    2,
    //bool checked;
    false
  };

  const DropdownParams kOptionsDropdown =
  {
    //bool persistant;
    false,
    //std::string boxTexture;
    "dropdownField",
    //int32_t cornerSize;
    9,
    //std::string expandArrowTexture;
    "smallArrowDown",
    //std::string contractArrowTexture;
    "smallArrowUp",
    //std::string scrollBarTexture;
    "greySliderLineVertical",
    //std::string scrollBarPointTexture;
    "greySliderEnd",
    //int32_t width;
    400,
    //int32_t maxEntries;
    5,
    //int32_t entryHeight;
    50,
    //int32_t arrowsMargin;
    20,
    //int32_t scrollBarMargin;
    5,
    //int32_t z;
    5,
    //std::string textFont;
    kVeraFont,
    //uint32_t textSize;
    16,
    //SDL_Color textColor;
    k50GreyColor
  };
}
