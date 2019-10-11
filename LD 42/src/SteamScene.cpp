#include "SteamScene.h"

#include "Sprite.h"
#include "IconSprite.h"
#include "Game.h"
#include "Time.h"
#include "Input.h"
#include "Camera.h"

#include <algorithm>


namespace
{
  const float g = 2 * 9.81f;
  const float pi = 3.14159265358979323846f;
  const float Rad2Deg = 180.0f / pi;
  const float Deg2Rad = pi / 180.0f;

  const int32_t kLeftborder = -512 + 33;
  const int32_t kRightborder = 512 - 33;
  const int32_t kBottomborder = 384 + 66;

  const float kMinAngle = 45 * Deg2Rad;
  const float kMaxAngle = 85 * Deg2Rad;

  const int32_t kMaxHeight = 700;
  const int32_t kMinHeight = 400;

  float MaximumDistance(float initSpeed, float angle)
  {
    return ((initSpeed * initSpeed) / g) * std::sin(2 * angle);
  }

  float MaximumHeight(float initSpeed, float angle)
  {
    return ((initSpeed * initSpeed) * (std::sin(angle) * std::sin(angle)) / 2 * g);
  }

  float MinimumSpeed(float angle)
  {
    return std::sqrt((kMinHeight * 2.0f * g) / (std::sin(angle) * std::sin(angle)));
  }

  float MaximumSpeed(float angle)
  {
    return std::sqrt((kMaxHeight * 2.0f * g) / (std::sin(angle) * std::sin(angle)));
  }
}

namespace LD42
{
  SteamScene::SteamScene(CherEngine::Game& game)
    : IScene(game)
  {
    for (float angle = kMinAngle; angle <= kMaxAngle; angle += (0.5f * Deg2Rad))
    {
      const auto minSpeed = MinimumSpeed(angle);
      const auto maxSpeed = MaximumSpeed(angle);

      for (float speed = minSpeed; speed <= maxSpeed; speed += 1.0f)
      {
        _solutions.push_back({ 0, angle, speed, MaximumDistance(speed, angle) });
      }
    }
  }

  void SteamScene::DoLoad()
  {
    _desktop = CreateSprite("emptydesktop");

    _gaben = CreateSprite("gaben");
    _gaben->SetCenterPosition(-512 + 93, -384 + 572);

    _steamsale = CreateSprite("steamsale");
    _steamsale->SetCenterPosition(-512 + 929, -384 + 572);

    for (int32_t i = 0; i < 5; i++)
    {
      _gameIcons.emplace_back(std::make_unique<IconSprite>(*this, "xcom", 32.3f));
      _gameIcons.emplace_back(std::make_unique<IconSprite>(*this, "subnautica", 13.5f));
      _gameIcons.emplace_back(std::make_unique<IconSprite>(*this, "bayonetta", 13.3f));
      _gameIcons.emplace_back(std::make_unique<IconSprite>(*this, "civ", 8.6f));
      _gameIcons.emplace_back(std::make_unique<IconSprite>(*this, "divinity", 10.2f));
      _gameIcons.emplace_back(std::make_unique<IconSprite>(*this, "orwell", 1.0f));
      _gameIcons.emplace_back(std::make_unique<IconSprite>(*this, "oxygen", 1.0f));
    }

    _spritesLaunched.resize(_gameIcons.size(), { false, 0, 0, 0, 0, 0, 0 });

    _cursorShadows.resize(kCursorShadows, nullptr);
    for (int32_t i = 0; i < kCursorShadows; i++)
    {
      _cursorShadows[i] = CreateSprite("cursorsprite");
    }

    _spacebarBG = CreateSprite("spacebar_bg");
    _spacebarBG->SetCenterPosition(0, 350);

    _spacebarBlue = CreateSprite("spacebar_blue");
    _spacebarBlue->AttachTo(_spacebarBG);

    _spacebarRed = CreateSprite("spacebar_red");
    _spacebarRed->AttachTo(_spacebarBG);

  }

  void SteamScene::DoRestart()
  {
    _speedMultiplier = kDefaultSpeedMultiplier;
    _bonusSpawn = kDefaultBonusSpawn;
    _overdrive = false;
    _eventDuration = kDefaultEventDuration;
    _launchDelay = kDefaultLaunchDelay;

    _desktop->SetVisible(true);

    for (auto& launched : _spritesLaunched)
    {
      launched.launched = false;
      launched.split = false;
    }

    for (auto& icon : _gameIcons)
    {
      icon->Restore();
      icon->SetVisible(false);
    }

    _fullSpace = 0.5f * kCapacity;

    _spacebarBG->SetVisible(true);
    _spacebarBlue->SetVisible(true);
    _spacebarBlue->SetWidthPercentage(_fullSpace / kCapacity);
    _spacebarRed->SetWidthPercentage(_fullSpace / kCapacity);
    _spacebarRed->SetVisible(false);

    int32_t x = CherEngine::GInput.MouseX, y = CherEngine::GInput.MouseY;
    CherEngine::GCamera.TransformMouse(x, y);
    for (int32_t i = 0; i < kCursorShadows; i++)
    {
      _cursorShadows[i]->SetVisible(true);
      _cursorShadows[i]->SetCenterPosition(x + 16, y + 16);
    }

    _timer = 0.0f;
    _eventTimer = 0.0f;
    _nextEventTimer = 5.0f;

    _gaben->SetVisible(false);
    _gabenEventOn = false;

    _steamsale->SetVisible(false);
    _steamsaleEventOn = false;

    _nextEventIsGaben = _game.RandomNumber(0, 1) == 0;
  }

  void SteamScene::Update()
  {
    _timer += CherEngine::GTime.deltaTime;

    auto countLaunched = 0;
    for (int32_t i = 0; i < _spritesLaunched.size(); i++)
    {
      if (_spritesLaunched[i].launched)
      {
        countLaunched += 1;
      }
    }

    if (_timer > kDuration && countLaunched == 0)
    {
      _game.PlaySound("logoff");
      _game.StartScene("wonScene");
      return;
    }

    if (_timer >= kDuration - kOvedriveDuration && !_overdrive)
    {
      _overdrive = true;
      Overdrive();
    }
    else if (_timer + _eventDuration < kDuration - kOvedriveDuration)
    {
      _nextEventTimer -= CherEngine::GTime.deltaTime;
      if (_nextEventTimer <= 0.0f)
      {
        _nextEventTimer += _game.RandomNumber(kMinNextEvent, kMaxNextEvent);
        RandomEvent();
      }
    }

    if (_gabenEventOn || _steamsaleEventOn)
    {
      _eventTimer += CherEngine::GTime.deltaTime;

      if (_eventTimer > kEventAppearSpeed)
      {
        if (_steamsaleEventOn)
        {
          _bonusSpawn = kSteamSaleBonusSpawn;
        }

        if (_gabenEventOn)
        {
          _speedMultiplier = kGabenSpeedMultiplier;
        }
      }

      if (_eventTimer < _eventDuration - kEventBlinking)
      {
        if (_steamsaleEventOn)
        {
          _steamsale->SetAlpha(std::min(1.0f, _eventTimer / kEventAppearSpeed));
        }

        if (_gabenEventOn)
        {
          _gaben->SetAlpha(std::min(1.0f, _eventTimer / kEventAppearSpeed));
        }
      }
      else if (_eventTimer >= _eventDuration - kEventBlinking)
      {
        const auto alpha = (1.0f - std::fmod(_eventTimer - (_eventDuration - kEventBlinking), kEventBlinkingSpeed)) * 0.5f;

        if (_steamsaleEventOn)
        {
          _steamsale->SetAlpha(0.5f + alpha);
        }

        if (_gabenEventOn)
        {
          _gaben->SetAlpha(0.5f + alpha);
        }
      }

      if (_eventTimer >= _eventDuration)
      {
        if (_steamsaleEventOn)
        {
          _bonusSpawn = kDefaultBonusSpawn;
          _steamsaleEventOn = false;
          _steamsale->SetVisible(false);
        }

        if (_gabenEventOn)
        {
          _speedMultiplier = kDefaultSpeedMultiplier;
          _gabenEventOn = false;
          _gaben->SetVisible(false);
        }

        _eventTimer = 0.0f;
      }
    }

    _cursorTimer -= CherEngine::GTime.deltaTime;
    if (_cursorTimer <= 0.0f)
    {
      for (int32_t i = kCursorShadows - 1; i > 0; i--)
      {
        int32_t x = _cursorShadows[i - 1]->CenterX, y = _cursorShadows[i - 1]->CenterY;
        _cursorShadows[i]->SetCenterPosition(x, y);
      }

      int32_t x = CherEngine::GInput.MouseX, y = CherEngine::GInput.MouseY;
      CherEngine::GCamera.TransformMouse(x, y);
      _cursorShadows[0]->SetCenterPosition(x + 16, y + 16);

      _cursorTimer = 0.005f;
    }

    for (int32_t i = 0; i < _spritesLaunched.size(); i++)
    {
      auto& icon = _gameIcons[i];
      auto& spriteLaunched = _spritesLaunched[i];
      if (spriteLaunched.launched && !spriteLaunched.split)
      {
        spriteLaunched.t += _speedMultiplier * CherEngine::GTime.deltaTime;
        auto x = spriteLaunched.initialX + spriteLaunched.direction * (spriteLaunched.velocity * spriteLaunched.t * std::cos(spriteLaunched.angle));
        auto y = spriteLaunched.initialY - (spriteLaunched.velocity * spriteLaunched.t * std::sin(spriteLaunched.angle))
          + (0.5f * g * spriteLaunched.t * spriteLaunched.t);
        icon->GetMasterSprite()->SetCenterPosition(static_cast<int32_t>(x), static_cast<int32_t>(y));

        if (y > kBottomborder)
        {
          spriteLaunched.launched = false;
          icon->SetVisible(false);

          _fullSpace = std::min(_fullSpace + icon->Weight, kCapacity);
          const auto pc = _fullSpace / kCapacity;
          if (pc > 0.75f && !_spacebarRed->GetVisible())
          {
            _spacebarRed->SetVisible(true);
            _spacebarBlue->SetVisible(false);
          }


          if (_fullSpace >= kCapacity)
          {
            _game.PlaySound("logoff");
            _game.StartScene("lostScene");
            return;
          }
          else
          {
            _game.PlaySound("hardwarefail");
          }

          _spacebarBlue->SetWidthPercentage(pc);
          _spacebarRed->SetWidthPercentage(pc);

        }

        if (!spriteLaunched.split)
        {
          for (int32_t i = kCursorShadows - 1; i >= 0; i--)
          {
            // Find first shadow inside
            auto cursorShadow = _cursorShadows[i];
            if (icon->GetMasterSprite()->IsInside(cursorShadow->CenterX - 16, cursorShadow->CenterY - 16))
            {
              CherEngine::Sprite* firstLeftOutsideShadow = nullptr;
              CherEngine::Sprite* firstRightOutsideShadow = nullptr;
              for (int32_t j = i - 1; j >= 0; j--)
              {
                auto potentialShadow = _cursorShadows[j];
                if (!icon->GetMasterSprite()->IsInside(potentialShadow->CenterX - 16, potentialShadow->CenterY - 16))
                {
                  firstLeftOutsideShadow = potentialShadow;
                  break;
                }
              }

              for (int32_t j = i + 1; j < kCursorShadows; j++)
              {
                auto potentialShadow = _cursorShadows[j];
                if (!icon->GetMasterSprite()->IsInside(potentialShadow->CenterX - 16, potentialShadow->CenterY - 16))
                {
                  firstRightOutsideShadow = potentialShadow;
                  break;
                }
              }

              if (firstLeftOutsideShadow != nullptr && firstRightOutsideShadow != nullptr)
              {
                const int xDiff = std::abs(firstLeftOutsideShadow->CenterX - firstRightOutsideShadow->CenterX);
                const int yDiff = std::abs(firstLeftOutsideShadow->CenterY - firstRightOutsideShadow->CenterY);

                LD42::SplitDirection direction;

                if (xDiff == 0)
                {
                  direction = kSplitDirection_Vertical;
                }
                else if (yDiff == 0)
                {
                  direction = kSplitDirection_Horizontal;
                }
                else
                {
                  const auto angle = std::atan2(yDiff, xDiff);
                  const auto lX = firstLeftOutsideShadow->CenterX;
                  const auto rX = firstRightOutsideShadow->CenterX;
                  const auto lY = firstLeftOutsideShadow->CenterY;
                  const auto rY = firstRightOutsideShadow->CenterY;

                  if (angle < Deg2Rad * 23)
                  {
                    direction = kSplitDirection_Horizontal;
                  }
                  else if (angle < Deg2Rad * (45 + 23) && ((lX < rX  &&  lY < rY) || (lX > rX  &&  lY > rY)))
                  {
                    direction = kSplitDirection_DiagonalLR;
                  }
                  else if (angle < Deg2Rad * (45 + 23) && ((lX > rX  &&  lY < rY) || (lX < rX  &&  lY > rY)))
                  {
                    direction = kSplitDirection_DiagonalRL;
                  }
                  else
                  {
                    direction = kSplitDirection_Vertical;
                  }
                }

                spriteLaunched.split = true;
                spriteLaunched.initialMasterX = icon->GetMasterSprite()->CenterX;
                spriteLaunched.initialMasterY = icon->GetMasterSprite()->CenterY;
                spriteLaunched.t = 0.0f;
                switch (direction)
                {
                case LD42::kSplitDirection_Horizontal:
                  spriteLaunched.velocityX = 0;
                  spriteLaunched.velocityY = 20;
                  break;
                case LD42::kSplitDirection_Vertical:
                  spriteLaunched.velocityX = -30;
                  spriteLaunched.velocityY = 0;
                  break;
                case LD42::kSplitDirection_DiagonalLR:
                  spriteLaunched.velocityX = 15;
                  spriteLaunched.velocityY = 10;
                  break;
                case LD42::kSplitDirection_DiagonalRL:
                  spriteLaunched.velocityX = -15;
                  spriteLaunched.velocityY = 10;
                  break;
                default:
                  break;
                }

                _game.PlaySound("recycle");

                icon->Split(direction);
              }
              break;
            }
          }
        }
      }
      else if (spriteLaunched.launched && spriteLaunched.split)
      {
        spriteLaunched.t += _speedMultiplier * CherEngine::GTime.deltaTime;
        auto x = spriteLaunched.initialMasterX + (spriteLaunched.velocityX * spriteLaunched.t);
        auto y = spriteLaunched.initialMasterY - (spriteLaunched.velocityY * spriteLaunched.t) + (0.5f * g * spriteLaunched.t * spriteLaunched.t);
        icon->GetMasterSprite()->SetCenterPosition(static_cast<int32_t>(x), static_cast<int32_t>(y));

        auto xSecond = spriteLaunched.initialMasterX + (-spriteLaunched.velocityX * spriteLaunched.t);
        auto ySecond = spriteLaunched.initialMasterY - (-spriteLaunched.velocityY * spriteLaunched.t) + (0.5f * g * spriteLaunched.t * spriteLaunched.t);
        icon->GetSecondMasterSprite()->SetCenterPosition(static_cast<int32_t>(xSecond), static_cast<int32_t>(ySecond));

        if (y > kBottomborder && ySecond > kBottomborder)
        {
          spriteLaunched.launched = false;
          spriteLaunched.split = false;
          icon->SetVisible(false);
          icon->Restore();
        }
      }
    }

    _launchTimer -= CherEngine::GTime.deltaTime;
    if (_launchTimer <= 0.0f && _timer < kDuration)
    {
      auto launchCount = _bonusSpawn + _game.RandomPoisson();
      for (size_t i = 0; i < launchCount; i++)
      {
        LaunchRandomnIcon();
      }

      // Always launch at least one
      if (launchCount == 0)
      {
        LaunchRandomnIcon();
      }

      _launchTimer = _launchDelay;
    }
  }

  FiringSolution SteamScene::RandomSolution(int32_t x)
  {
    const auto distanceToLeft = std::abs(kLeftborder - x);
    const auto distanceToRight = std::abs(kRightborder - x);

    std::vector<FiringSolution> possibleSolutions;
    for (const auto& solution : _solutions)
    {
      if (solution.distance <= distanceToLeft)
      {
        FiringSolution newSolution = solution;
        newSolution.direction = -1;
        possibleSolutions.push_back(newSolution);
      }
    }

    for (const auto& solution : _solutions)
    {
      if (solution.distance <= distanceToRight)
      {
        FiringSolution newSolution = solution;
        newSolution.direction = 1;
        possibleSolutions.push_back(newSolution);
      }
    }

    auto index = _game.RandomNumber(0, static_cast<int32_t>(possibleSolutions.size() - 1));
    return possibleSolutions[index];
  }

  void SteamScene::Overdrive()
  {
    _eventTimer = 0.0f;
    _gaben->SetVisible(true);
    _gabenEventOn = true;
    _steamsale->SetVisible(true);
    _steamsaleEventOn = true;
    _eventDuration = kOvedriveDuration;
    _launchDelay = kOverideLaunchDelay;
    _game.PlaySound("summersale");
  }

  void SteamScene::RandomEvent()
  {
    if (_nextEventIsGaben && !_gabenEventOn)
    {
      _gaben->SetVisible(true);
      _gabenEventOn = true;
      _game.PlaySound("aaa");
    }
    else if (_nextEventIsGaben && _gabenEventOn)
    {
      // Reset
      _eventTimer = kEventAppearSpeed;
    }
    else if (!_nextEventIsGaben && !_steamsaleEventOn)
    {
      _steamsale->SetVisible(true);
      _steamsaleEventOn = true;
      _game.PlaySound("saletime");
    }
    else if (!_nextEventIsGaben && _steamsaleEventOn)
    {
      _eventTimer = kEventAppearSpeed;
    }

    _nextEventIsGaben = !_nextEventIsGaben;
  }

  void SteamScene::LaunchRandomnIcon()
  {
    std::vector<int32_t> possibleToLaunch;
    for (int32_t i = 0; i < _spritesLaunched.size(); i++)
    {
      if (!_spritesLaunched[i].launched)
      {
        possibleToLaunch.push_back(i);
      }
    }

    if (possibleToLaunch.size() <= 0)
    {
      return;
    }

    auto index = _game.RandomNumber(0, static_cast<int32_t>(possibleToLaunch.size()-1));

    auto x = _game.RandomNumber(kLeftborder, kRightborder);
    auto solution = RandomSolution(x);

    auto& motion = _spritesLaunched[possibleToLaunch[index]];
    motion.launched = true;
    motion.velocity = solution.speed;
    motion.angle = solution.angle;
    motion.initialX = x;
    motion.initialY = 384;
    motion.direction = solution.direction;
    motion.t = 0.0f;

    auto& icon = _gameIcons[possibleToLaunch[index]];
    icon->SetVisible(true);
    icon->GetMasterSprite()->SetCenterPosition(motion.initialX, motion.initialY);
  }
}
