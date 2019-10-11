#pragma once

#include "IScene.h"

#include "AlignedGroup.h"
#include "LD45Styles.h"
#include "Grid.h"

#include <vector>

namespace LD45
{
  class AI;
  class Button;
  class Dice;
  class DiceBuy;
  class FTC;
  class Tooltip;
  class Upgrade;

  enum GameState
  {
    kGameState_PlayerRollingPhase,
    kGameState_PlayerUsingSkill,
    kGameState_PlayerBuyingPhase,
    kGameState_PlayerActivelyBuying,
    kGameState_AIWaitingForRoll,
    kGameState_AIRollingPhase,
    kGameState_AIUsingSkill,
    kGameState_AIBuyingPhase,
    kGameState_AIActivelyBuying,
    kGameState_AIDone,
  };

  class PlayScene : public IScene
  {
  public:
    void Start() override;
    void Update() override;

    std::shared_ptr<IScene> DoReset() override { return std::make_shared<PlayScene>(); }

  private:
    void UpdateStatsAfterRoll();
    void UpdateStatsAfterEnteringBuy();
    void UpdateStatsAfterEndingTurn();
    void UpdateStatsAfterAIBuying(const int32_t cost);
    void UpdateStatsAfterBuying(const int32_t cost);
    void UpdatePlayer();

    void CancelBuy();
    void ConfirmBuy();

    void CancelTarget();
    void ConfirmTarget();
    bool UseSkill(Dice* skillDice, Dice* targetDice, size_t targetDiceSide);

    bool UpdateAIRoll();
    bool UpdateAIBuy();
    void UpdateAI();

    void BuyDice(const DiceBuy* boughtDice, bool player);

    void End();

    AlignedGroup _playerDiceGroup;
    std::vector<Dice*> _playerDice;

    AlignedGroup _aiDiceGroup;
    std::vector<Dice*> _aiDice;

    Button* _rollButton;
    Button* _buyButton;
    Button* _endTurnButton;

    FTC* _playerGoldText;
    FTC* _playerVPText;

    int32_t _playerGold;
    int32_t _playerPotentialGold;
    int32_t _playerVP;
    int32_t _playerPotentialVP;

    FTC* _aiGoldText;
    FTC* _aiVPText;

    int32_t _aiGold;
    int32_t _aiPotentialGold;
    int32_t _aiVP;
    int32_t _aiPotentialVP;

    Grid _upgradesGrid;
    std::vector<Upgrade*> _upgrades;
    std::vector<DiceBuy*> _diceBuy;

    std::unordered_map<DiceSkill, Tooltip*> _tooltips;

    std::unique_ptr<AI> _ai;
    float _aiTimer;
    Dice* _buyingTargetDice;
    size_t _buyingDiceSide;
    Upgrade* _buyingUpgrade;
    DiceBuy* _buyingDice;

    Dice* _skillDice;
    Dice* _skillTargetDice;
    size_t _skillTargetDiceSide;

    Sprite* _resultBackground;
    Sprite* _resultVictory;
    Sprite* _resultDefeat;
    Button* _backToMenuButton;

    GameState _state;
  };
}