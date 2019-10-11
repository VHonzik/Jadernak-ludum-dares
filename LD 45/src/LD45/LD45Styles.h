#pragma once

#include <unordered_map>

namespace LD45
{
  const char kPlayScene[] = "PlayScene";

  const enum DiceSkill
  {
    kDiceSkill_Nothing,
    kDiceSkill_OneGold,
    kDiceSkill_TwoGold,
    kDiceSkill_ThreeGold,
    kDiceSkill_OneVP,
    kDiceSkill_ThreeVP,
    kDiceSkill_SixVP,
    kDiceSkill_Reroll,
    kDiceSkill_Trash,
    kDiceSkill_UpGold,
  };

  const std::unordered_map<DiceSkill, std::string> kDiceSkillTextures =
  {
    {kDiceSkill_Nothing, "zzz"},
    {kDiceSkill_OneGold, "oneGold"},
    {kDiceSkill_TwoGold, "twoGold"},
    {kDiceSkill_ThreeGold, "threeGold"},
    {kDiceSkill_OneVP, "oneVP"},
    {kDiceSkill_ThreeVP, "threeVP"},
    {kDiceSkill_SixVP, "sixVP"},
    {kDiceSkill_Reroll, "reroll"},
    {kDiceSkill_Trash, "trash"},
    {kDiceSkill_UpGold, "upgold"},
  };

  const std::string kGoldGenericTooltip = " Gold can be spend to buy dice sides or one dice per turn in Buy phase and disappears at the end of turn.";
  const std::string kVPGenericTooltip = " Player with the most victory points when 4 side piles are bought is the winner.";

  const std::unordered_map<DiceSkill, std::string> kDiceSkillTooltips =
  {
    {kDiceSkill_Nothing, "Does nothing when rolled. Can be replaced by dice sides in Buy phase."},
    {kDiceSkill_OneGold, "Rewards 1 gold when rolled." + kGoldGenericTooltip},
    {kDiceSkill_TwoGold, "Rewards 2 gold when rolled." + kGoldGenericTooltip},
    {kDiceSkill_ThreeGold, "Rewards 3 gold when rolled." + kGoldGenericTooltip},
    {kDiceSkill_OneVP, "Rewards 1 victory point when rolled." + kVPGenericTooltip},
    {kDiceSkill_ThreeVP, "Rewards 3 victory point when rolled." + kVPGenericTooltip},
    {kDiceSkill_SixVP, "Rewards 6 victory point when rolled." + kVPGenericTooltip},
    {kDiceSkill_Reroll, "Dice will be re-rolled if it lands on this. Improves dice consistency. Dice filled with this is useless."},
    {kDiceSkill_Trash, "Can remove one side of another dice owned by the player when rolled."},
    {kDiceSkill_UpGold, "Can upgrade another dice gold side owned by the player when rolled. Upgrading non-gold sides has not effect but still uses the action."},
  };

  constexpr auto kDiceSides = 6U;
}
