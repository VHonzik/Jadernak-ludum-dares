#pragma once

#include <vector>

namespace LD45
{
  class Dice;
  class Upgrade;
  class DiceBuy;

  struct RollResult
  {
    bool done;
    Dice* actionDice;
    Dice* targetedDice;
    size_t targetedDiceSide;
  };

  struct BuyResult
  {
    bool done;
    Upgrade* boughUpgrade;
    Dice* targetedDice;
    size_t targetedDiceSide;
    DiceBuy* boughtDice;
  };

  class AI
  {
  public:
    RollResult RollAction(const std::vector<Dice*>& dice);
    BuyResult BuyAction(const std::vector<Dice*>& dice,  const std::vector<Upgrade*>& upgrades, const std::vector<DiceBuy*>& buyDice, const int32_t gold, const int32_t vp);
  private:
    Upgrade* ChooseUpgrade(const std::vector<Dice*>& dice, const std::vector<Upgrade*>& affodableUpgrades, size_t emptySideCounts);
  };
}
