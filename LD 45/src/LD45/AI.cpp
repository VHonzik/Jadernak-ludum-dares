#include "AI.h"

#include "Dice.h"
#include "DiceBuy.h"
#include "Upgrade.h"

#include <algorithm>

namespace LD45
{
  const BuyResult kFinishedBuying = { true, nullptr, nullptr, std::numeric_limits<size_t>::max(), nullptr };
  const RollResult kFinishedRolling = { true, nullptr, nullptr, std::numeric_limits<size_t>::max() };

  RollResult AI::RollAction(const std::vector<Dice*>& dice)
  {
    for (auto die : dice)
    {
      if (die->RolledActiveSkill() && !die->CheckState(kDiceState_ActiveSkillUsed))
      {
        RollResult result;
        result.done = false;
        result.actionDice = die;

        if (die->GetCurrentSkill() == kDiceSkill_UpGold)
        {
          for (auto otherDie : dice)
          {
            if (otherDie == die) continue;
            for (size_t i = 0; i < kDiceSides; i++)
            {
              if (otherDie->GetSkill(i) == kDiceSkill_OneGold || otherDie->GetSkill(i) == kDiceSkill_TwoGold)
              {
                result.targetedDice = otherDie;
                result.targetedDiceSide = i;
              }
            }
          }
        }
        else  if (die->GetCurrentSkill() == kDiceSkill_Trash)
        {
          for (auto otherDie : dice)
          {
            if (otherDie == die) continue;
            for (size_t i = 0; i < kDiceSides; i++)
            {
              if (otherDie->GetSkill(i) == kDiceSkill_OneGold)
              {
                result.targetedDice = otherDie;
                result.targetedDiceSide = i;
              }
            }
          }
        }
      }
    }

    return kFinishedRolling;
  }

  BuyResult AI::BuyAction(const std::vector<Dice*>& dice,
    const std::vector<Upgrade*>& upgrades,
    const std::vector<DiceBuy*>& buyDice,
    const int32_t gold,
    const int32_t vp)
  {
    // First try buying dice
    if (!buyDice[0]->DiceBought())
    {
      DiceBuy* wantedDice = nullptr;
      if (buyDice[2]->GetAmount() > 0 && dice.size() < 3)
      {
        wantedDice = buyDice[2];
      }
      else if (buyDice[1]->GetAmount() > 0 && dice.size() < 4)
      {
        wantedDice = buyDice[1];
      }
      else if (buyDice[2]->GetAmount() > 0)
      {
        wantedDice = buyDice[2];
      }
      else if (buyDice[0]->GetAmount() > 0)
      {
        wantedDice = buyDice[0];
      }
      else if (buyDice[1]->GetAmount() > 0)
      {
        wantedDice = buyDice[1];
      }

      if (wantedDice != nullptr)
      {
        return {
          false,
          nullptr,
          nullptr,
          std::numeric_limits<size_t>::max(),
          wantedDice
        };
      }
    }

    // Check we have any empty sides left
    std::vector<Dice*> dicesWithEmptySide;
    std::copy_if(std::cbegin(dice), std::cend(dice), std::back_inserter(dicesWithEmptySide), [](const Dice* dice)
    {
      return dice->HasEmptySide();
    });

    if (dicesWithEmptySide.size() == 0)
    {
      return kFinishedBuying;
    }

    size_t emptySidesCount = 0;
    for (const auto dice : dicesWithEmptySide)
    {
      emptySidesCount += dice->EmptySidesCount();
    }

    // Get all upgrades we can afford
    std::vector<Upgrade*> affordableUpgrades;
    std::copy_if(std::cbegin(upgrades), std::cend(upgrades), std::back_inserter(affordableUpgrades), [gold](const Upgrade* upgrade)
    {
      return upgrade->CanAfford(gold);
    });

    if (affordableUpgrades.size() == 0)
    {
      return kFinishedBuying;
    }

    // Pick one
    auto result = ChooseUpgrade(dice, affordableUpgrades, emptySidesCount);

    // Never buy nothing
    if (result != nullptr && result->GetSkill() != kDiceSkill_Nothing)
    {
      return { false, result, dicesWithEmptySide[0], dicesWithEmptySide[0]->EmptySideIndex() };
    }
    else
    {
      return kFinishedBuying;
    }
  }

  Upgrade* AI::ChooseUpgrade(const std::vector<Dice*>& dice, const std::vector<Upgrade*>& affodableUpgrades, size_t emptySideCounts)
  {
    Upgrade* result = nullptr;

    size_t upgoldcount = 0;
    size_t trashcount = 0;
    for (auto die : dice)
    {
      for (size_t i = 0; i < kDiceSides; i++)
      {
        if (die->GetSkill(i) == kDiceSkill_UpGold) upgoldcount++;
        if (die->GetSkill(i) == kDiceSkill_Trash) trashcount++;
      }
    }

    for (auto upgrade : affodableUpgrades)
    {
      if (emptySideCounts > 4 && upgrade->GetSkill() == kDiceSkill_TwoGold)
      {
        result = upgrade;
      }

      if (emptySideCounts > 2 && upgrade->GetSkill() == kDiceSkill_UpGold && upgoldcount < 2)
      {
        result = upgrade;
      }

      if (emptySideCounts > 2 && upgrade->GetSkill() == kDiceSkill_Trash && trashcount < 1)
      {
        result = upgrade;
      }

      if (emptySideCounts > 1 && upgrade->GetSkill() == kDiceSkill_ThreeGold)
      {
        result = upgrade;
      }

      if (emptySideCounts > 2 && upgrade->GetSkill() == kDiceSkill_ThreeVP)
      {
        result = upgrade;
      }

      if (upgrade->GetSkill() == kDiceSkill_SixVP)
      {
        result = upgrade;
      }
    }

    return result;
  }
}