#include "ResourceBar.h"

#include "BoxSprite.h"
#include "Game.h"
#include "EngineStyles.h"
#include "Sprite.h"

namespace LD43
{
  ResourceBar* ResourceBar::_one_only = nullptr;

  ResourceBar::ResourceBar(const ResourceBarParams& params)
    : _wood(params.initialWood)
    , _crystal(params.initialCrystal)
  {
    BoxSpriteParams boxParams = kOptionsGreyPanel;
    boxParams.persistant = params.persistant;
    boxParams.width = 220;
    boxParams.height = 40;
    boxParams.z = params.z;
    _panel = GGame.Create<BoxSprite>(boxParams);
    _panel->SetPosition(GGame.GetWidth() - _panel->GetWidth(), -boxParams.height);

    SpriteParams spriteParams;
    spriteParams.persistant = params.persistant;
    spriteParams.texture = nullptr;
    spriteParams.z = params.z+1;
    spriteParams.textureName = "woodResource";
    _woodIcon = GGame.Create<Sprite>(spriteParams);
    _woodIcon->SetWidth(11);
    _woodIcon->SetHeight(21);
    _woodIcon->SetCenterPosition(GGame.GetWidth() - 200, _panel->GetCenterY());

    FTCParams ftcParams;
    ftcParams.persistant = false;
    ftcParams.font = nullptr;
    ftcParams.fontName = kVeraFontBold;
    ftcParams.fontSize = 16;
    ftcParams.format = "#";
    ftcParams.defaultColor = k50GreyColor;

    _woodText = GGame.Create<FTC>(ftcParams);
    _woodText->SetHorizontalAlign(kHorizontalAlignment_Left);
    _woodText->SetVerticalAlign(kVerticalAlignment_Center);
    _woodText->SetPositon(_woodIcon->GetX() + _woodIcon->GetWidth() + 10, _panel->GetCenterY());
    _woodText->SetIntValue(0, _wood);

    spriteParams.textureName = "crystalResource";
    _crystalIcon = GGame.Create<Sprite>(spriteParams);
    _crystalIcon->SetWidth(19);
    _crystalIcon->SetHeight(18);
    _crystalIcon->SetCenterPosition(GGame.GetWidth() - 80, _panel->GetCenterY());

    _crystalText = GGame.Create<FTC>(ftcParams);
    _crystalText->SetHorizontalAlign(kHorizontalAlignment_Left);
    _crystalText->SetVerticalAlign(kVerticalAlignment_Center);
    _crystalText->SetPositon(_crystalIcon->GetX() + _crystalIcon->GetWidth() + 10, _panel->GetCenterY());
    _crystalText->SetIntValue(0, _crystal);

    _one_only = this;
  }

  void ResourceBar::SetY(int32_t y)
  {
    _panel->SetPosition(_panel->GetX(), y);
    _woodIcon->SetCenterPosition(_woodIcon->GetCenterX(), _panel->GetCenterY());
    _woodText->SetPositon(_woodIcon->GetX() + _woodIcon->GetWidth() + 10, _panel->GetCenterY());
    _crystalIcon->SetCenterPosition(GGame.GetWidth() - 80, _panel->GetCenterY());
    _crystalText->SetPositon(_crystalIcon->GetX() + _crystalIcon->GetWidth() + 10, _panel->GetCenterY());
  }

  bool ResourceBar::CanAfford(int32_t wood, int32_t crystal)
  {
    return wood <= _wood && crystal <= _crystal;
  }

  int32_t ResourceBar::GetHeight() const
  {
    return _panel->GetHeight();
  }
}
