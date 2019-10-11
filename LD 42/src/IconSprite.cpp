#include "IconSprite.h"

#include "IScene.h"
#include "Sprite.h"

namespace LD42
{
  IconSprite::IconSprite(CherEngine::IScene& scene, const char* name, float weight)
    : _wasSplit(false)
    , Weight(weight)
  {
    for (int32_t i = 0; i < kElementsCount; i++)
    {
      char buffer[128];
      std::snprintf(buffer, 128, "%s%d", name, i);
      auto sprite = scene.CreateSprite(buffer);
      _sprites.push_back(sprite);

      if (i > 0)
      {
        sprite->AttachTo(_sprites[0]);
      }
    }

    _masterSprite = _sprites[0];
  }

  CherEngine::Sprite* IconSprite::GetMasterSprite() const
  {
    return _masterSprite;
  }

  CherEngine::Sprite* IconSprite::GetSecondMasterSprite() const
  {
    return _secondMasterSprite;
  }

  void IconSprite::Restore()
  {
    _masterSprite = _sprites[0];
    _secondMasterSprite = nullptr;
    for (int32_t i = 1; i < kElementsCount; i++)
    {
      _sprites[i]->AttachTo(_sprites[0]);
      _sprites[i]->SetCenterPosition(0,0);
    }
  }

  void IconSprite::Split(SplitDirection direction)
  {
    _wasSplit = true;

    switch (direction)
    {
    case LD42::kSplitDirection_Horizontal:
      _secondMasterSprite = _sprites[3];
      _secondMasterSprite->AttachTo(nullptr);
      _secondMasterSprite->SetCenterPosition(_masterSprite->CenterX, _masterSprite->CenterY);
      _sprites[4]->AttachTo(_secondMasterSprite);
      _sprites[5]->AttachTo(_secondMasterSprite);
      _sprites[6]->AttachTo(_secondMasterSprite);
      break;
    case LD42::kSplitDirection_Vertical:
      _secondMasterSprite = _sprites[1];
      _secondMasterSprite->AttachTo(nullptr);
      _secondMasterSprite->SetCenterPosition(_masterSprite->CenterX, _masterSprite->CenterY);
      _sprites[2]->AttachTo(_secondMasterSprite);
      _sprites[3]->AttachTo(_secondMasterSprite);
      _sprites[4]->AttachTo(_secondMasterSprite);
      break;
    case LD42::kSplitDirection_DiagonalLR:
      _secondMasterSprite = _sprites[4];
      _secondMasterSprite->AttachTo(nullptr);
      _secondMasterSprite->SetCenterPosition(_masterSprite->CenterX, _masterSprite->CenterY);
      _sprites[5]->AttachTo(_secondMasterSprite);
      _sprites[6]->AttachTo(_secondMasterSprite);
      _sprites[7]->AttachTo(_secondMasterSprite);
      break;
    case LD42::kSplitDirection_DiagonalRL:
      _secondMasterSprite = _sprites[2];
      _secondMasterSprite->AttachTo(nullptr);
      _secondMasterSprite->SetCenterPosition(_masterSprite->CenterX, _masterSprite->CenterY);
      _sprites[3]->AttachTo(_secondMasterSprite);
      _sprites[4]->AttachTo(_secondMasterSprite);
      _sprites[5]->AttachTo(_secondMasterSprite);
      break;
    default:
      break;
    }
  }

  void IconSprite::SetVisible(bool visible)
  {
    for (auto sprite : _sprites)
    {
      sprite->SetVisible(visible);
    }
  }
}

