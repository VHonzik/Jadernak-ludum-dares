#pragma once

#include "IScene.h"
#include <memory>
#include <vector>
#include "IconSprite.h"

namespace CherEngine
{
  class Sprite;
}

namespace LD42
{
  struct IconMotion
  {
    bool launched;
    bool split;

    // beforeSplit
    float angle;
    float velocity;
    float t;
    int32_t initialX;
    int32_t initialY;
    int32_t direction;

    //afterSplit
    float velocityX;
    float velocityY;
    int32_t initialMasterX;
    int32_t initialMasterY;
  };

  struct FiringSolution
  {
    int32_t direction;
    float angle;
    float speed;
    float distance;
  };

  class SteamScene : public CherEngine::IScene
  {
  public:
    SteamScene(CherEngine::Game& game);
    void DoLoad() override;
    void DoRestart() override;
    void Update() override;
  private:
    CherEngine::Sprite* _desktop;

    CherEngine::Sprite* _spacebarBG;
    CherEngine::Sprite* _spacebarBlue;
    CherEngine::Sprite* _spacebarRed;

    const float kEventAppearSpeed = 0.5f;
    const float kEventBlinking = 3.0f;
    const float kEventBlinkingSpeed = 1.0f;
    const float kDefaultEventDuration = 10.0f;
    float _eventDuration;
    float _eventTimer;
    const int32_t kMinNextEvent = 12;
    const int32_t kMaxNextEvent = 20;
    float _nextEventTimer = 0.0f;
    bool _overdrive;
    const float kOvedriveDuration = 30.0f;
    const float kOverideLaunchDelay = 1.0f;

    bool _nextEventIsGaben;

    CherEngine::Sprite* _gaben;
    bool _gabenEventOn;
    const float kGabenSpeedMultiplier = 5.0f;

    CherEngine::Sprite* _steamsale;
    bool _steamsaleEventOn;
    const int32_t kSteamSaleBonusSpawn = 3;

    const float kCapacity = 150.0f;
    float _fullSpace;

    const int32_t kCursorShadows = 20;

    std::vector<std::unique_ptr<IconSprite>> _gameIcons;
    std::vector<IconMotion> _spritesLaunched;
    std::vector<FiringSolution> _solutions;
    const float kDefaultLaunchDelay = 2.0f;
    float _launchDelay;
    float _launchTimer;

    const float kDuration = 120.0f;
    float _timer;

    std::vector<CherEngine::Sprite*> _cursorShadows;
    float _cursorTimer;

    const float kDefaultSpeedMultiplier = 2.0f;
    float _speedMultiplier;
    const int32_t kDefaultBonusSpawn = 0;
    int32_t _bonusSpawn;

    void LaunchRandomnIcon();
    void RandomEvent();
    void Overdrive();
    FiringSolution RandomSolution(int32_t x);
  };
}
