#include "PlayScene.h"

#include "Sprite.h"
#include "Game.h"
#include "Input.h"
#include "Time.h"
#include "Utils.h"
#include "FTC.h"
#include "EngineStyles.h"
#include "Settings.h"
#include "EngineSettings.h"
#include "Text.h"
#include "BuildingIcon.h"
#include "ResourceBar.h"
#include "BuildingPanel.h"

#include <string>
#include <cassert>

namespace
{
  const std::vector<std::string> kMap =
  {
    "bgPlain", "bgPlain", "bgPlain", "bgPlain", "bgPlain", "bgPlain", "bgPlain", "bgPlain", "bgPlain", "bgPlain", "bgPlain", "bgPlain",
    "bgPlain", "bgRock1", "bgPlain", "bgPlain", "bgPlain", "bgPlain", "bgPlain", "bgPlain", "bgRock2", "bgPlain", "bgPlain", "bgPlain",
    "bgPlain", "bgPlain", "bgPlain", "bgRock2", "trees", "bgPlain", "bgPlain", "bgPlain", "bgPlain", "bgPlain", "bgRock3", "bgPlain",
    "bgPlain", "bgPlain", "bgPlain", "trees", "bgPlain", "bgPlain", "bgPlain", "bgPlain", "bgPlain", "bgPlain", "bgPlain", "bgPlain",
    "bgPlain", "bgPlain", "bgPlain", "bgPlain", "bgPlain", "bgPlain", "bgPlain", "bgPlain", "bgPlain", "bgPlain", "bgPlain", "bgPlain",
    "bgPlain", "bgPlain", "bgPlain", "bgPlain", "bgPlain", "bgPlain", "bgPlain", "blueCrystals", "bgPlain", "bgPlain", "bgPlain", "bgPlain",
    "bgPlain", "bgPlain", "bgRock3", "bgPlain", "bgPlain", "bgPlain", "bgPlain", "bgPlain", "bgPlain", "bgPlain", "bgPlain", "bgPlain",
    "bgPlain", "bgPlain", "bgPlain", "bgPlain", "bgPlain", "bgPlain", "bgPlain", "bgPlain", "bgPlain", "bgPlain", "bgPlain", "bgPlain",
  };

  const auto kSpriteWidth = 120;
  const auto kSpriteHeight = 140;

  const auto kSpriteOddXOffset = 60;
  const auto kSpriteOddYOffset = 100;

  const std::pair<int32_t, int32_t> kLandingOffset = { 1,-12 };
  const float kLandingDuration = 2.0f;
  const float kLandingBaseDelay = 1.25f;
  const float kLandingAppearDuration = 0.5f;
  const float kLandingFastDuration = 0.75f;

  const float kUiSlideDuration = 1.0f;
}

namespace LD43
{
  void PlayScene::Start()
  {
    const auto fitsH = static_cast<int32_t>(std::ceil(static_cast<float>(GGame.GetWidth()) / kSpriteWidth)) + 1;
    const auto fitsV = static_cast<int32_t>(std::ceil(static_cast<float>(GGame.GetHeight()) / kSpriteOddYOffset)) + 1;
    _columns = fitsH;
    _rows = fitsV;

    SpriteParams params;
    params.persistant = false;
    params.texture = nullptr;
    params.z = 5;

    for (int32_t i = 0; i < kMap.size(); i++)
    {
      params.textureName = kMap[i];

      auto sprite = GGame.Create<Sprite>(params);

      assert(kSpriteWidth == sprite->GetWidth());
      assert(kSpriteHeight == sprite->GetHeight());

      auto pos = VectorIndexToXY(i);
      sprite->SetCenterPosition(pos.first, pos.second);

      _backgroundSprites.push_back(sprite);
    }



    params.z = 6;
    params.textureName = "landingPlatform";
    _landingPlatform = GGame.Create<Sprite>(params);
    auto pos = VectorIndexToXY(RowColumnToVectorIndex(3, 5));
    _landingPlatform->SetCenterPosition(pos.first, pos.second);
    _landingPlatform->SetAlpha(0.0f);

    params.z = 7;
    params.textureName = "spaceCraft";
    _spacecraft = GGame.Create<Sprite>(params);
    pos = VectorIndexToXY(RowColumnToVectorIndex(3, 5));
    pos.first += kLandingOffset.first;
    pos.second += kLandingOffset.second - 150;
    _spacecraft->SetCenterPosition(pos.first, pos.second);

    ResourceBarParams resourceBarParams;
    resourceBarParams.persistant = false;
    resourceBarParams.initialWood = 150;
    resourceBarParams.initialCrystal = 20;
    resourceBarParams.z = 100;
    _resourceBar = GGame.Create<ResourceBar>(resourceBarParams);

    BuildingPanelParams buildingPanelParams;
    buildingPanelParams.persistant = false;
    buildingPanelParams.z = 100;

    _buildingPanel = GGame.Create<BuildingPanel>(buildingPanelParams);
    _buildingPanel->SetPosition(GGame.GetWidth() - _buildingPanel->GetWidth(),
      GGame.GetHeight());

    _landingTimer = 0.0f;
    _landingFinished = false;
    _landingDestination = VectorIndexToXY(RowColumnToVectorIndex(3, 5));
    _landingDestination.first += kLandingOffset.first;
    _landingDestination.second += kLandingOffset.second;

    _uiSlideTimer = 0.0f;
    _uiSlideFinished = false;
  }

  std::pair<int32_t, int32_t> PlayScene::VectorIndexToRowColumn(int32_t index) const
  {
    const auto row = index / _columns;
    const auto column = index % _columns;

    return std::make_pair(row, column);
  }

  int32_t PlayScene::RowColumnToVectorIndex(int32_t row, int32_t column) const
  {
    return row * _columns + column;
  }

  std::pair<int32_t, int32_t> PlayScene::VectorIndexToXY(int32_t index) const
  {
    const auto row = VectorIndexToRowColumn(index).first;
    const auto column = VectorIndexToRowColumn(index).second;

    auto x = column * kSpriteWidth;
    auto y = row * kSpriteOddYOffset;

    if (row % 2 != 0)
    {
      x += kSpriteOddXOffset;
    }

    return std::make_pair(x, y);
  }

  int32_t PlayScene::CenterXYToVectorIndex(int32_t x, int32_t y) const
  {
    const auto row = y / kSpriteOddYOffset;
    const auto column = x / kSpriteWidth;

    return RowColumnToVectorIndex(row, column);
  }

  std::pair<int32_t, int32_t> PlayScene::XYToCenterXY(int32_t x, int32_t y) const
  {
    const auto row = (y + (kSpriteOddYOffset / 2)) / kSpriteOddYOffset;
    auto centerY = row * kSpriteOddYOffset;

    auto column = 0;
    auto centerX = 0;
    if (row % 2 != 0)
    {
      if (x >= 0)
      {
        column =  x / kSpriteWidth;
        centerX = kSpriteOddXOffset + column * kSpriteWidth;
      }
      else
      {
        column = x / kSpriteWidth;
        centerX = -kSpriteOddXOffset - column * kSpriteWidth;
      }
    }
    else
    {
      column = (x + kSpriteOddXOffset) / kSpriteWidth;
      centerX = column * kSpriteWidth;
    }

    return std::make_pair(centerX, centerY);
  }

  bool PlayScene::IsIndexFullyVisible(int32_t index) const
  {
    const auto row = VectorIndexToRowColumn(index).first;
    const auto column = VectorIndexToRowColumn(index).second;

    if (row <= 0 || row >= _rows-2) return false;

    if (row % 2 == 0)
    {
      if (column <= 0 || column > _columns - 2) return false;
    }
    else
    {
      if (column < 0 || column >= _columns - 2) return false;
    }

    if (column >= _columns - 4 && row >= _rows - 3) return false;

    return true;
  }


  void PlayScene::LandingIntroUpdate()
  {

    if (!_landingFinished)
    {
      _landingTimer += GTime.deltaTime;

      _spacecraft->SetAlpha(Clamp(_landingTimer, 0.0f, kLandingAppearDuration)
        / kLandingAppearDuration);

      int32_t y = 0;
      if (_landingTimer <= kLandingFastDuration)
      {
        y = MoveTowards(_spacecraft->GetCenterY(), _landingDestination.second, 2);
      }
      else
      {
        y = MoveTowards(_spacecraft->GetCenterY(), _landingDestination.second, 1);
      }


      _spacecraft->SetCenterPosition(_spacecraft->GetCenterX(), y);

      if (_landingTimer >= kLandingBaseDelay)
      {
        auto t = Clamp(_landingTimer, kLandingBaseDelay, kLandingDuration);
        t = (t - kLandingBaseDelay) / (kLandingDuration - kLandingBaseDelay);
        _landingPlatform->SetAlpha(t);
      }


      if (_landingTimer > kLandingDuration)
      {
        _landingFinished = true;
      }
    }
  }

  void PlayScene::UISlideIntroUpdate()
  {
    if (_landingFinished && !_uiSlideFinished)
    {
      _uiSlideTimer += GTime.deltaTime;

      _resourceBar->SetY(Interpolate(-_resourceBar->GetHeight(), 0,
        Clamp(_uiSlideTimer, 0.0f, kUiSlideDuration) / kUiSlideDuration));

      _buildingPanel->SetPosition(_buildingPanel->GetX(),
        GGame.GetHeight() - Interpolate(0, _buildingPanel->GetHeight(),
          Clamp(_uiSlideTimer, 0.0f, kUiSlideDuration) / kUiSlideDuration));

      if (_uiSlideTimer >= kUiSlideDuration)
      {
        _uiSlideFinished = true;
      }
    }
  }

  void PlayScene::Update()
  {
    const auto selectPos = XYToCenterXY(GInput.GetMouseX(), GInput.GetMouseY());
    const auto index = CenterXYToVectorIndex(selectPos.first, selectPos.second);
    const auto visible = IsIndexFullyVisible(index);
    const auto occupied = _backgroundSprites[index]->GetTextureName() != "bgPlain";
    _buildingPanel->UpdateCursorPos(selectPos, visible, occupied);

    LandingIntroUpdate();
    UISlideIntroUpdate();

  }
}