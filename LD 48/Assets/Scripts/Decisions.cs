using System;
using System.Collections.Generic;
using UnityEngine;

public enum Decision
{
  CoinsToOldMan,
  TurnedAwayEntrance,
  ApproachedKid,
  FoughtDrunkard,
  HelpedWoman,
  ChooseSmithThiefSide,
  GoToPub,
}

public class DecisionData
{
  public Decision Decision { get; set; }
  public bool Made { get; set; } = false;
  public int Choice { get; set; } = 0;
  public int[] MoralePenality { get; set; }
  public int[] MoraleRequirement { get; set; }
}

public class Decisions
{
  public int Morale { get; set; } = 20;
  public DecisionData CoinGivenToOldMan { get; private set; } = new DecisionData() { Decision = Decision.CoinsToOldMan, MoralePenality = new int[] { 5, 0 }, MoraleRequirement = new int[] { -5, -5 } };
  public DecisionData TurnedAwayAtEntrance { get; private set; } = new DecisionData() { Decision = Decision.TurnedAwayEntrance, MoralePenality = new int[2] { 0, 10 }, MoraleRequirement = new int[] { -5, -5 } };
  public DecisionData ApproachedKid { get; private set; } = new DecisionData() { Decision = Decision.ApproachedKid, MoralePenality = new int[2] { 5, 0 }, MoraleRequirement = new int[] { -5, -5 } };
  public DecisionData FoughtDrunkard { get; private set; } = new DecisionData() { Decision = Decision.FoughtDrunkard, MoralePenality = new int[] { 10 }, MoraleRequirement = new int[] { -5 } };
  public DecisionData HelpedWoman { get; private set; } = new DecisionData() { Decision = Decision.HelpedWoman, MoralePenality = new int[2] { 5, 0 }, MoraleRequirement = new int[] { 5, -5 } };
  public DecisionData ChooseSmithThiefSide { get; private set; } = new DecisionData() { Decision = Decision.ChooseSmithThiefSide, MoralePenality = new int[2] { 5, 10 }, MoraleRequirement = new int[] { 0, 0 } };
  public DecisionData GoToPub { get; private set; } = new DecisionData() { Decision = Decision.GoToPub, MoralePenality = new int[2] { 5, 0 }, MoraleRequirement = new int[] { 0, -5 } };

  private Dictionary<Decision, DecisionData> decisionsDict = new Dictionary<Decision, DecisionData>();

  public Decisions()
  {
    decisionsDict.Add(CoinGivenToOldMan.Decision, CoinGivenToOldMan);
    decisionsDict.Add(TurnedAwayAtEntrance.Decision, TurnedAwayAtEntrance);
    decisionsDict.Add(ApproachedKid.Decision, ApproachedKid);
    decisionsDict.Add(FoughtDrunkard.Decision, FoughtDrunkard);
    decisionsDict.Add(HelpedWoman.Decision, HelpedWoman);
    decisionsDict.Add(ChooseSmithThiefSide.Decision, ChooseSmithThiefSide);
    decisionsDict.Add(GoToPub.Decision, GoToPub);
  }

  public void MakeDecision(Decision decision, int choice)
  {
    Debug.Assert(decisionsDict.ContainsKey(decision));
    decisionsDict[decision].Made = true;
    Debug.Assert(choice >= 0 && choice < decisionsDict[decision].MoralePenality.Length);
    decisionsDict[decision].Choice = choice;
    Morale -= decisionsDict[decision].MoralePenality[choice];
  }

  public bool MadeDecision(Decision decision)
  {
    Debug.Assert(decisionsDict.ContainsKey(decision));
    return decisionsDict[decision].Made;
  }

  public bool MadeDecisionWithChoice(Decision decision, int choice)
  {
    Debug.Assert(decisionsDict.ContainsKey(decision));
    if (decisionsDict[decision].Made)
    {
      return decisionsDict[decision].Choice == choice;
    }

    return false;
  }

  public bool MadeDecisionWithDifferentChoice(Decision decision, int choice)
  {
    Debug.Assert(decisionsDict.ContainsKey(decision));
    if (decisionsDict[decision].Made)
    {
      return decisionsDict[decision].Choice != choice;
    }

    return false;
  }

  public DecisionData GetDecisionData(Decision decision)
  {
    Debug.Assert(decisionsDict.ContainsKey(decision));
    return decisionsDict[decision];
  }

  public void SufferMorale(int amount)
  {
    Morale -= amount;
  }

  public void Reset()
  {
    Morale = 20;
    foreach (var decisionPair in decisionsDict)
    {
      decisionPair.Value.Made = false;
      decisionPair.Value.Choice = 0;
    }
  }
}

