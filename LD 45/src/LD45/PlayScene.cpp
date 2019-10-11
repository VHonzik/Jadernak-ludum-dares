#include "PlayScene.h"

#include "AI.h"
#include "AlignedGroup.h"
#include "Button.h"
#include "Dice.h"
#include "DiceBuy.h"
#include "EngineStyles.h"
#include "Game.h"
#include "Input.h"
#include "LD45Styles.h"
#include "EngineTime.h"
#include "Tooltip.h"
#include "Upgrade.h"

#include <array>
#include <string>

using namespace LD45;

namespace {
  const auto kRollButtonOffset = 50;

  const auto kUpgradesGridRows = 3;
  const auto kUpgradesGridColumns = 4;

  const auto kAIStatePause = 1.0f;
  const auto kAIBuyPause = 1.0f;

  const size_t kEmptyPilesForEnd = 4;
}

namespace LD45
{
  void PlayScene::Start()
  {
    TooltipParams tooltipParams;
    tooltipParams.persistant = false;
    tooltipParams.fontName = kVeraFont;
    tooltipParams.fontSize = 16;
    tooltipParams.textColor = kDarkGreyColor;
    tooltipParams.cornerSize = 7;
    tooltipParams.textureName = "greyPanel";
    tooltipParams.width = 400;
    tooltipParams.padding = 10;
    tooltipParams.z = 7;

    for (const auto& tooltipText : kDiceSkillTooltips)
    {
      tooltipParams.text = tooltipText.second;
      _tooltips[tooltipText.first] = GGame.Create<Tooltip>(tooltipParams);
    }

    auto buttonParams = kMainMenuBlueButton;
    buttonParams.text = "ROLL";
    buttonParams.width = 200;
    buttonParams.height = 64;
    buttonParams.clickSound = "diceRoll";

    _rollButton = GGame.Create<Button>(buttonParams);
    _rollButton->SetCenterPosition(GGame.GetWidth() - buttonParams.width / 2 - 20, GGame.GetHalfHeight());
    _rollButton->Show(false);

    buttonParams.clickSound = "uiClick";
    buttonParams.text = "END TURN";
    _endTurnButton = GGame.Create<Button>(buttonParams);
    _endTurnButton->SetCenterPosition(_rollButton->GetCenterX(), _rollButton->GetCenterY());
    _endTurnButton->Show(false);

    buttonParams.text = "BUY";
    buttonParams.clickSound = "cashRegister";
    _buyButton = GGame.Create<Button>(buttonParams);
    _buyButton->SetCenterPosition(_rollButton->GetCenterX(), _rollButton->GetCenterY());
    _buyButton->Show(true);

    FTCParams ftcParams;
    ftcParams.persistant = false;
    ftcParams.font = nullptr;
    ftcParams.fontName = kVeraFont;
    ftcParams.fontSize = 32;
    ftcParams.format = "Gold: # #";
    ftcParams.defaultColor = kLightGreyColor;
    ftcParams.z = 0;

    _playerGoldText = GGame.Create<FTC>(ftcParams);
    _playerGoldText->SetPositon(20, GGame.GetHeight() - 52);
    _playerGoldText->SetIntValue(0, 0);
    _playerGoldText->SetValueColor(0, kGoldColor);
    _playerGoldText->SetStringValue(1, "");
    _playerGoldText->SetValueColor(1, kGreenColor);

    _aiGoldText = GGame.Create<FTC>(ftcParams);
    _aiGoldText->SetPositon(GGame.GetWidth() - 200, 20);
    _aiGoldText->SetIntValue(0, 0);
    _aiGoldText->SetValueColor(0, kGoldColor);
    _aiGoldText->SetStringValue(1, "");
    _aiGoldText->SetValueColor(1, kGreenColor);

    ftcParams.format = "VP: # #";

    _playerVPText = GGame.Create<FTC>(ftcParams);
    _playerVPText->SetPositon(20, GGame.GetHeight() - 104);
    _playerVPText->SetIntValue(0, 0);
    _playerVPText->SetValueColor(0, kGreenColor);
    _playerVPText->SetStringValue(1, "");
    _playerVPText->SetValueColor(1, kGreenColor);

    _aiVPText = GGame.Create<FTC>(ftcParams);
    _aiVPText->SetPositon(GGame.GetWidth() - 167, 72);
    _aiVPText->SetIntValue(0, 0);
    _aiVPText->SetValueColor(0, kGreenColor);
    _aiVPText->SetStringValue(1, "");
    _aiVPText->SetValueColor(1, kGreenColor);

    _upgradesGrid = Grid(kUpgradesGridRows, kUpgradesGridColumns, 3 * GGame.GetWidth() / 5, 3 * GGame.GetHeight() / 5);
    _upgradesGrid.SetCenterPosition(GGame.GetHalfWidth(), GGame.GetHalfHeight());

    UpgradeParams upgradeParams;
    upgradeParams.persistant = false;
    upgradeParams.skill = kDiceSkill_OneGold;
    upgradeParams.z = 0;
    upgradeParams.count = 10;

    DiceBuyParams diceBuyParams;
    diceBuyParams.persistant = false;
    diceBuyParams.skills = { };
    diceBuyParams.cost = 0;
    diceBuyParams.count = 4;
    diceBuyParams.highlightCenterX = GGame.GetWidth() * 7 / 8;
    diceBuyParams.highlightCenterY = GGame.GetHeight() * 6 / 7;
    diceBuyParams.z = 0;

    for (size_t i = 0; i < kUpgradesGridRows; i++)
    {
      for (size_t j = 0; j < kUpgradesGridColumns; j++)
      {
        if (i == 0 && j == kUpgradesGridColumns - 1)
        {
          upgradeParams.count = 6;
          upgradeParams.cost = 1;
          upgradeParams.skill = kDiceSkill_OneGold;
        }
        else if (i == 1 && j == kUpgradesGridColumns - 1)
        {
          upgradeParams.count = 10;
          upgradeParams.cost = 3;
          upgradeParams.skill = kDiceSkill_TwoGold;
        }
        else if (i == 2 && j == kUpgradesGridColumns - 1)
        {
          upgradeParams.count = 8;
          upgradeParams.cost = 6;
          upgradeParams.skill = kDiceSkill_ThreeGold;
        }
        else if (i == 0 && j == kUpgradesGridColumns - 2)
        {
          upgradeParams.count = 6;
          upgradeParams.cost = 2;
          upgradeParams.skill = kDiceSkill_OneVP;
        }
        else if (i == 1 && j == kUpgradesGridColumns - 2)
        {
          upgradeParams.count = 10;
          upgradeParams.cost = 5;
          upgradeParams.skill = kDiceSkill_ThreeVP;
        }
        else if (i == 2 && j == kUpgradesGridColumns - 2)
        {
          upgradeParams.count = 8;
          upgradeParams.cost = 8;
          upgradeParams.skill = kDiceSkill_SixVP;
        }
        else if (i == 0 && j == kUpgradesGridColumns - 3)
        {
          upgradeParams.count = 3;
          upgradeParams.cost = 3;
          upgradeParams.skill = kDiceSkill_Reroll;
        }
        else if (i == 1 && j == kUpgradesGridColumns - 3)
        {
          upgradeParams.count = 3;
          upgradeParams.cost = 4;
          upgradeParams.skill = kDiceSkill_Trash;
        }
        else if (i == 2 && j == kUpgradesGridColumns - 3)
        {
          upgradeParams.count = 3;
          upgradeParams.cost = 5;
          upgradeParams.skill = kDiceSkill_UpGold;
        }
        else if (i == 0 && j == kUpgradesGridColumns - 4)
        {
          diceBuyParams.skills = { kDiceSkill_OneGold, kDiceSkill_OneGold, kDiceSkill_OneGold, kDiceSkill_OneGold, kDiceSkill_OneGold, kDiceSkill_OneGold };
        }
        else if (i == 1 && j == kUpgradesGridColumns - 4)
        {
          diceBuyParams.skills = { kDiceSkill_Nothing, kDiceSkill_Nothing, kDiceSkill_Nothing, kDiceSkill_Nothing, kDiceSkill_Nothing, kDiceSkill_Nothing };
        }
        else if (i == 2 && j == kUpgradesGridColumns - 4)
        {
          diceBuyParams.skills = { kDiceSkill_OneGold, kDiceSkill_OneGold, kDiceSkill_OneGold, kDiceSkill_OneVP, kDiceSkill_OneVP, kDiceSkill_OneVP };
        }

        if (j == kUpgradesGridColumns - 4)
        {
          _diceBuy.push_back(GGame.Create<DiceBuy>(diceBuyParams));
          _diceBuy[_diceBuy.size() - 1]->SetTooltips(&_tooltips);
          _upgradesGrid.AddSingle(_diceBuy[_diceBuy.size() - 1], j + i * kUpgradesGridColumns);
        }
        else
        {
          _upgrades.push_back(GGame.Create<Upgrade>(upgradeParams));
          _upgrades[_upgrades.size() - 1]->SetTooltips(&_tooltips);
          _upgradesGrid.AddSingle(_upgrades[_upgrades.size() - 1], j + i * kUpgradesGridColumns);
        }
      }
    }

    _upgradesGrid.Align();

    const auto diceHeight = _diceBuy[0]->GetHeight();

    _playerDiceGroup = AlignedGroup(kGroupDirection_Horizontal, kHorizontalAlignment_Center, kVerticalAlignment_Top, 20,
      GGame.GetHalfWidth(), GGame.GetHeight() - static_cast<decltype(diceHeight)>(1.5 * diceHeight));

    _aiDiceGroup = AlignedGroup(kGroupDirection_Horizontal, kHorizontalAlignment_Center, kVerticalAlignment_Top, 20,
      GGame.GetHalfWidth(), static_cast<decltype(diceHeight)>(0.5 * diceHeight));

    SpriteParams spriteParams;
    spriteParams.persistant = false;
    spriteParams.textureName = "endScreenBg";
    spriteParams.texture = nullptr;
    spriteParams.z = 100;

    _resultBackground = GGame.Create<Sprite>(spriteParams);
    _resultBackground->SetPosition(0, 0);
    _resultBackground->Show(false);

    spriteParams.z = 101;
    spriteParams.textureName = "endScreenDefeat";
    _resultDefeat = GGame.Create<Sprite>(spriteParams);
    _resultDefeat->SetPosition(0, 0);
    _resultDefeat->Show(false);

    spriteParams.textureName = "endScreenVictory";
    _resultVictory = GGame.Create<Sprite>(spriteParams);
    _resultVictory->SetPosition(0, 0);
    _resultVictory->Show(false);

    buttonParams = kMainMenuBlueButton;
    buttonParams.text = "END GAME";
    buttonParams.width = 230;
    buttonParams.height = 85;
    buttonParams.z = 110;
    _backToMenuButton = GGame.Create<Button>(buttonParams);
    _backToMenuButton->SetCenterPosition(GGame.GetHalfWidth(), 3 * GGame.GetHeight() / 4);
    _backToMenuButton->Show(false);

    _playerGold = 0;
    _playerPotentialGold = 0;
    _playerVP = 0;
    _playerPotentialVP = 0;

    _aiGold = 0;
    _aiPotentialGold = 0;
    _aiVP = 0;
    _aiPotentialVP = 0;

    _state = kGameState_PlayerRollingPhase;

    _ai = std::make_unique<AI>();
    _aiTimer = 0.0f;
    _buyingUpgrade = nullptr;
    _buyingTargetDice = nullptr;
  }

  void PlayScene::UpdateStatsAfterRoll()
  {
    const auto& dices = _state == kGameState_PlayerRollingPhase ? _playerDice : _aiDice;
    auto& potentialGold = _state == kGameState_PlayerRollingPhase ? _playerPotentialGold : _aiPotentialGold;
    auto& gold = _state == kGameState_PlayerRollingPhase ? _playerGold : _aiGold;
    auto& potentialVP = _state == kGameState_PlayerRollingPhase ? _playerPotentialVP : _aiPotentialVP;
    auto& vp = _state == kGameState_PlayerRollingPhase ? _playerVP : _aiVP;
    auto& goldText = _state == kGameState_PlayerRollingPhase ? _playerGoldText : _aiGoldText;
    auto& vpText = _state == kGameState_PlayerRollingPhase ? _playerVPText : _aiVPText;

    potentialGold = 0;
    potentialVP = 0;

    for (const auto dice : dices)
    {
      switch (dice->GetCurrentSkill())
      {
      case kDiceSkill_OneGold:
        potentialGold++;
        break;
      case kDiceSkill_TwoGold:
        potentialGold += 2;
        break;
      case kDiceSkill_ThreeGold:
        potentialGold +=3;
        break;
      case kDiceSkill_OneVP:
        potentialVP++;
        break;
      case kDiceSkill_ThreeVP:
        potentialVP += 3;
        break;
      case kDiceSkill_SixVP:
        potentialVP += 6;
        break;
      default:
        break;
      }
    }
    goldText->SetIntValue(0, 0);
    goldText->SetStringValue(1, (potentialGold >= 0 ? "+" : "-") + std::to_string(potentialGold));

    vpText->SetIntValue(0, vp);
    vpText->SetStringValue(1, (potentialVP >= 0 ? "+" : "-") + std::to_string(potentialVP));

    for (auto upgrade : _upgrades)
    {
      upgrade->HideAfford();
    }

    for (auto diceBuy : _diceBuy)
    {
      diceBuy->HideAfford();
    }
  }

  void PlayScene::UpdateStatsAfterEnteringBuy()
  {
    auto& potentialGold = _state == kGameState_PlayerBuyingPhase ? _playerPotentialGold : _aiPotentialGold;
    auto& gold = _state == kGameState_PlayerBuyingPhase ? _playerGold : _aiGold;
    auto& potentialVP = _state == kGameState_PlayerBuyingPhase ? _playerPotentialVP : _aiPotentialVP;
    auto& vp = _state == kGameState_PlayerBuyingPhase ? _playerVP : _aiVP;
    auto& goldText = _state == kGameState_PlayerBuyingPhase ? _playerGoldText : _aiGoldText;
    auto& vpText = _state == kGameState_PlayerBuyingPhase ? _playerVPText : _aiVPText;

    gold = potentialGold;
    potentialGold = 0;
    goldText->SetIntValue(0, gold);
    goldText->SetStringValue(1, "");

    vp += potentialVP;
    potentialVP = 0;
    vpText->SetIntValue(0, vp);
    vpText->SetStringValue(1, "");

    if (_state == kGameState_PlayerBuyingPhase)
    {
      for (auto upgrade : _upgrades)
      {
        upgrade->UpdateCanAfford(_playerGold);
      }

      for (auto diceBuy : _diceBuy)
      {
        diceBuy->UpdateCanAfford(_playerGold);
      }
    }
    else
    {
      for (auto upgrade : _upgrades)
      {
        upgrade->HideAfford();
      }

      for (auto diceBuy : _diceBuy)
      {
        diceBuy->HideAfford();
      }
    }
  }

  void PlayScene::UpdateStatsAfterEndingTurn()
  {
    auto& potentialGold = _state == kGameState_AIWaitingForRoll ? _playerPotentialGold : _aiPotentialGold;
    auto& gold = _state == kGameState_AIWaitingForRoll ? _playerGold : _aiGold;
    auto& potentialVP = _state == kGameState_AIWaitingForRoll ? _playerPotentialVP : _aiPotentialVP;
    auto& goldText = _state == kGameState_AIWaitingForRoll ? _playerGoldText : _aiGoldText;
    auto& vpText = _state == kGameState_AIWaitingForRoll ? _playerVPText : _aiVPText;

    potentialGold = 0;
    gold = 0;

    potentialVP = 0;

    goldText->SetIntValue(0, 0);
    goldText->SetStringValue(1, "");

    vpText->SetStringValue(1, "");

    for (auto upgrade : _upgrades)
    {
      upgrade->HideAfford();
    }
  }

  void PlayScene::UpdateStatsAfterBuying(const int32_t cost)
  {
    _playerGold -= cost;
    for (auto upgrade : _upgrades)
    {
      upgrade->UpdateCanAfford(_playerGold);
    }

    for (auto diceBuy : _diceBuy)
    {
      diceBuy->UpdateCanAfford(_playerGold);
    }

    _playerGoldText->SetIntValue(0, _playerGold);
  }

  void PlayScene::UpdateStatsAfterAIBuying(const int32_t cost)
  {
    _aiGold -= cost;
    _aiGoldText->SetIntValue(0, _aiGold);
  }

  void PlayScene::CancelBuy()
  {
    _state = kGameState_PlayerBuyingPhase;
    for (auto upgrade : _upgrades)
    {
      if (upgrade->IsBuying())
      {
        upgrade->CancelBuy();
      }
      upgrade->UpdateCanAfford(_playerGold);
    }

    for (auto diceBuy : _diceBuy)
    {
      diceBuy->UpdateCanAfford(_playerGold);
    }

    for (auto playerDice : _playerDice)
    {
      playerDice->RemoveState(kDiceState_PossibleBuyTarget);
      playerDice->RemoveState(kDiceState_PickingBuySide);
      playerDice->RemoveState(kDiceState_OtherDiceActive);
    }
  }

  void PlayScene::ConfirmBuy()
  {
    _state = kGameState_PlayerBuyingPhase;
    auto upgrade = std::find_if(std::begin(_upgrades), std::end(_upgrades), [](const auto upgrade) { return upgrade->IsBuying(); });
    if (upgrade == std::end(_upgrades))
    {
      CancelBuy();
    }

    auto dice = std::find_if(std::begin(_playerDice), std::end(_playerDice), [](const auto dice) { return dice->SidePicked().first; });
    if (dice == std::end(_playerDice))
    {
      CancelBuy();
    }

    const auto sidePicked = (*dice)->SidePicked();

    (*upgrade)->ConfirmBuy();

    (*dice)->ReplaceSide(sidePicked.second, (*upgrade)->GetSkill());

    UpdateStatsAfterBuying((*upgrade)->Cost());

    for (auto playerDice : _playerDice)
    {
      playerDice->RemoveState(kDiceState_PossibleBuyTarget);
      playerDice->RemoveState(kDiceState_PickingBuySide);
      playerDice->RemoveState(kDiceState_OtherDiceActive);
    }

    auto emptyPiles = std::count_if(std::cbegin(_upgrades), std::cend(_upgrades), [](const Upgrade* upgrade) { return upgrade->IsEmpty(); });
    if (emptyPiles >= kEmptyPilesForEnd)
    {
      End();
    }
  }

  void PlayScene::CancelTarget()
  {
    for (auto playerDice : _playerDice)
    {
      playerDice->RemoveState(kDiceState_PossibleActiveSkillTarget);
      playerDice->RemoveState(kDiceState_ActiveSkillTarget);
      playerDice->RemoveState(kDiceState_OtherDiceActive);
      playerDice->RemoveState(kDiceState_UsingActiveSkill);
    }
  }

  bool PlayScene::UseSkill(Dice* skillDice, Dice* targetDice, size_t targetDiceSide)
  {
    switch (skillDice->GetCurrentSkill())
    {
    case kDiceSkill_Trash:
    {
      targetDice->ReplaceSide(targetDiceSide, kDiceSkill_Nothing);
      skillDice->AddState(kDiceState_ActiveSkillUsed);
      break;
    }
    case kDiceSkill_UpGold:
    {
      const auto pickedSkill = targetDice->GetSkill(targetDiceSide);
      if (pickedSkill != kDiceSkill_OneGold && pickedSkill != kDiceSkill_TwoGold)
      {
        skillDice->AddState(kDiceState_ActiveSkillUsed);
      }
      else
      {
        targetDice->ReplaceSide(targetDiceSide, pickedSkill == kDiceSkill_OneGold ? kDiceSkill_TwoGold : kDiceSkill_ThreeGold);
        skillDice->AddState(kDiceState_ActiveSkillUsed);
      }
    }
    break;
    default:
      return false;
    }

    return true;
  }

  void PlayScene::ConfirmTarget()
  {
    auto skillDice = std::find_if(std::begin(_playerDice), std::end(_playerDice), [](const auto dice) { return dice->CheckState(kDiceState_UsingActiveSkill); });
    if (skillDice == std::end(_playerDice))
    {
      CancelTarget();
      return;
    }

    auto targetDice = std::find_if(std::begin(_playerDice), std::end(_playerDice), [](const auto dice) { return dice->SidePicked().first; });
    if (targetDice == std::end(_playerDice))
    {
      CancelTarget();
      return;
    }

    if (!UseSkill((*skillDice), (*targetDice), (*targetDice)->SidePicked().second))
    {
      CancelTarget();
    }

    for (const auto playerDice : _playerDice)
    {
      if (playerDice->RolledActiveSkill() && !playerDice->CheckState(kDiceState_ActiveSkillUsed))
      {
        playerDice->AddState(kDiceState_PossibleActiveSkill);
      }
      else
      {
        playerDice->RemoveState(kDiceState_PossibleActiveSkill);
      }

      playerDice->RemoveState(kDiceState_PossibleActiveSkillTarget);
      playerDice->RemoveState(kDiceState_ActiveSkillTarget);
      playerDice->RemoveState(kDiceState_OtherDiceActive);
      playerDice->RemoveState(kDiceState_UsingActiveSkill);
    }
  }

  void PlayScene::End()
  {
    _resultBackground->Show(true);
    _resultVictory->Show(_playerVP >= _aiVP);
    _resultDefeat->Show(!_resultVictory->IsShown());
    _backToMenuButton->Show(true);
  }

  void PlayScene::Update()
  {
    UpdatePlayer();
    UpdateAI();

    if (_backToMenuButton->Pressed())
    {
      Reset();
      GGame.PlayScene(kMainMenuScene);
    }
  }

  void PlayScene::UpdatePlayer()
  {
    if (_rollButton->Released() && _state == kGameState_AIDone)
    {
      for (size_t i = 0; i < _playerDice.size(); i++)
      {
        _playerDice[i]->Roll(0.25f + i * (0.75f / _playerDice.size()));
      }

      _state = kGameState_PlayerRollingPhase;

      UpdateStatsAfterRoll();

      for (const auto playerDice : _playerDice)
      {
        if (playerDice->RolledActiveSkill())
        {
          playerDice->AddState(kDiceState_PossibleActiveSkill);
        }
      }

      _rollButton->Show(false);
      _buyButton->Show(true);
    }

    if (_buyButton->Released() && _state == kGameState_PlayerRollingPhase)
    {
      CancelTarget();

      for (const auto playerDice : _playerDice)
      {
        playerDice->RemoveState(kDiceState_ActiveSkillUsed);
        playerDice->RemoveState(kDiceState_PossibleActiveSkill);
      }

      for (auto diceBuy : _diceBuy)
      {
        diceBuy->SetDiceBought(false);
      }

      _state = kGameState_PlayerBuyingPhase;

      UpdateStatsAfterEnteringBuy();

      _buyButton->Show(false);
      _endTurnButton->Show(true);
    }

    if (_endTurnButton->Released() && (_state == kGameState_PlayerBuyingPhase || _state == kGameState_PlayerActivelyBuying))
    {
      if (_state == kGameState_PlayerActivelyBuying)
      {
        CancelBuy();
      }

      _state = kGameState_AIWaitingForRoll;

      UpdateStatsAfterEndingTurn();

      for (auto aiDice : _aiDice)
      {
        aiDice->Roll(1.0f);
      }

      UpdateStatsAfterRoll();

      _endTurnButton->Show(false);
    }

    if (_state == kGameState_PlayerActivelyBuying && GInput.KeyPressed(SDLK_ESCAPE))
    {
      CancelBuy();
    }

    if (_state == kGameState_PlayerRollingPhase)
    {
      for (auto dice : _playerDice)
      {
        if (dice->CheckState(kDiceState_UsingActiveSkill))
        {
          _state = kGameState_PlayerUsingSkill;
          for (auto otherDice : _playerDice)
          {
            if (otherDice == dice) continue;
            if (!otherDice->CheckState(kDiceState_PossibleActiveSkillTarget)) otherDice->AddState(kDiceState_PossibleActiveSkillTarget);
          }
        }
      }
    }

    if (_state == kGameState_PlayerUsingSkill)
    {
      if (GInput.KeyPressed(SDLK_ESCAPE))
      {
        CancelTarget();
        _state = kGameState_PlayerRollingPhase;
      }
      else
      {
        for (auto dice : _playerDice)
        {
          if (dice->CheckState(kDiceState_ActiveSkillTarget))
          {
            for (auto otherDice : _playerDice)
            {
              if (otherDice == dice) continue;
              if (otherDice->CheckState(kDiceState_UsingActiveSkill)) continue;
              if (!otherDice->CheckState(kDiceState_OtherDiceActive)) otherDice->AddState(kDiceState_OtherDiceActive);
            }

            auto side_picked = dice->SidePicked();
            if (side_picked.first)
            {
              ConfirmTarget();
              _state = kGameState_PlayerRollingPhase;

              UpdateStatsAfterRoll();
            }
          }
        }
      }
    }

    if (_state == kGameState_PlayerBuyingPhase)
    {
      bool anyEmptyDice = false;
      for (auto dice : _playerDice)
      {
        if (dice->HasEmptySide()) anyEmptyDice = true;
      }

      if (anyEmptyDice)
      {
        for (auto upgrade : _upgrades)
        {
          if (upgrade->Released() && upgrade->CanAfford(_playerGold))
          {
            upgrade->EnterBuy();
            _state = kGameState_PlayerActivelyBuying;

            for (auto upgrade : _upgrades)
            {
              upgrade->HideAfford();
            }

            for (auto diceBuy : _diceBuy)
            {
              diceBuy->HideAfford();
            }

            for (auto dice : _playerDice)
            {
              if (dice->HasEmptySide())
              {
                dice->AddState(kDiceState_PossibleBuyTarget);
              }
            }
            break;
          }
        }
      }

      for (auto diceBuy : _diceBuy)
      {
        if (diceBuy->Pressed() && !diceBuy->DiceBought())
        {
          diceBuy->ConfirmBuy();
          BuyDice(diceBuy, true);
          for (auto diceBuy : _diceBuy)
          {
            diceBuy->SetDiceBought(true);
          }
        }
      }
    }

    if (_state == kGameState_PlayerActivelyBuying)
    {
      for (auto dice : _playerDice)
      {
        if (dice->CheckState(kDiceState_PickingBuySide))
        {
          for (auto otherDice : _playerDice)
          {
            if (otherDice != dice && !otherDice->CheckState(kDiceState_OtherDiceActive))
            {
              otherDice->AddState(kDiceState_OtherDiceActive);
            }
          }
        }

        auto side_picked = dice->SidePicked();
        if (side_picked.first)
        {
          ConfirmBuy();
        }
      }
    }
  }

  bool PlayScene::UpdateAIRoll()
  {
    const auto result = _ai->RollAction(_aiDice);

    if (result.actionDice != nullptr && result.targetedDice != nullptr)
    {
      _aiTimer = kAIStatePause;

      _skillDice = result.actionDice;
      _skillTargetDice = result.targetedDice;
      _skillTargetDiceSide = result.targetedDiceSide;

      _state = kGameState_AIUsingSkill;
    }

    return result.done;
  }

  bool PlayScene::UpdateAIBuy()
  {
    const auto result = _ai->BuyAction(_aiDice, _upgrades, _diceBuy, _aiGold, _aiVP);

    if (result.boughUpgrade != nullptr && result.targetedDice != nullptr)
    {
      _aiTimer = kAIBuyPause;

      _buyingTargetDice = result.targetedDice;
      _buyingDiceSide = result.targetedDiceSide;
      _buyingUpgrade = result.boughUpgrade;

      _buyingUpgrade->EnterBuy();
      _buyingTargetDice->AddState(kDiceState_PickingBuySide);
      _buyingTargetDice->AddState(kDiceState_PossibleBuyTarget);
      _state = kGameState_AIActivelyBuying;
    }

    if (result.boughtDice != nullptr)
    {
      _aiTimer = kAIBuyPause;

      _buyingDice = result.boughtDice;
      _buyingDice->EnterBuy();

      _state = kGameState_AIActivelyBuying;
    }

    return result.done;
  }

  void PlayScene::UpdateAI()
  {
    if (_aiTimer > 0.0f)
    {
      _aiTimer -= GTime.deltaTime;
      return;
    }

    if (_state == kGameState_AIWaitingForRoll)
    {
      bool allDone = true;
      for (const auto aiDice : _aiDice)
      {
        if (aiDice->CheckState(kDiceState_Rolling))
        {
          allDone = false;
        }
      }

      if (allDone)
      {
        _state = kGameState_AIRollingPhase;
      }
    }
    else if (_state == kGameState_AIRollingPhase)
    {
      if (UpdateAIRoll())
      {
        _state = kGameState_AIBuyingPhase;
        _aiTimer = kAIStatePause;
        UpdateStatsAfterEnteringBuy();

        for (auto diceBuy : _diceBuy)
        {
          diceBuy->SetDiceBought(false);
        }
      }
    }
    else if (_state == kGameState_AIBuyingPhase)
    {
      if (UpdateAIBuy())
      {
        _state = kGameState_AIDone;
        UpdateStatsAfterEndingTurn();
        _rollButton->Show(true);
      }
    }
    else if (_state == kGameState_AIActivelyBuying)
    {
      if (_buyingUpgrade != nullptr)
      {
        _buyingUpgrade->ConfirmBuy();
        _buyingTargetDice->RemoveState(kDiceState_PickingBuySide);
        _buyingTargetDice->RemoveState(kDiceState_PossibleBuyTarget);
        _buyingTargetDice->ReplaceSide(_buyingDiceSide, _buyingUpgrade->GetSkill());
        UpdateStatsAfterAIBuying(_buyingUpgrade->Cost());
        _buyingUpgrade = nullptr;
        _buyingTargetDice = nullptr;
      }

      if (_buyingDice != nullptr)
      {
        BuyDice(_buyingDice, false);
        _buyingDice->ConfirmBuy();
        _buyingDice = nullptr;

        for (auto diceBuy : _diceBuy)
        {
          diceBuy->SetDiceBought(true);
        }
      }

      _state = kGameState_AIBuyingPhase;
    }
    else if (_state == kGameState_AIUsingSkill)
    {
      UseSkill(_skillDice, _skillTargetDice, _skillTargetDiceSide);
      _state = kGameState_AIRollingPhase;
    }
  }

  void PlayScene::BuyDice(const DiceBuy* boughtDice, bool player)
  {
    DiceParams diceParams;
    diceParams.persistant = false;
    diceParams.z = 5;
    diceParams.highlightCenterX = GGame.GetWidth() * 7 / 8;
    diceParams.highlightCenterY = GGame.GetHeight() * 6 / 7;
    diceParams.initialSkills = boughtDice->GetSkills();

    const auto dice = GGame.Create<Dice>(diceParams);
    dice->Show(true);
    dice->SetTooltips(&_tooltips);
    if (player)
    {
      _playerDice.push_back(dice);
      _playerDiceGroup.AddVA(dice);
    }
    else
    {
      _aiDice.push_back(dice);
      _aiDiceGroup.AddVA(dice);
    }
  }
}