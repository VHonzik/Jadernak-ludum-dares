using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BehaviorTree;
using Entities;

public class AIManager  {

    private  static AIManager _instance;

    public static AIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new AIManager();
            }
            return _instance;
        }
        
    }

    private AIManager() 
    {
        InitializeBehaviors();
    }
 
    public Behavior GetBehavior(string name, Blackboard blackboard)
    {

        
        
        /*
        if (name == "NPCBasic")
        {
            BehaviorAction isSelected = new BehaviorAction(blackboard, ((NPC)(blackboard.BehaviorOwner)).isSelected);
            BehaviorAction CalculatePathToTarget = new BehaviorAction(blackboard, ((NPC)(blackboard.BehaviorOwner)).CalculatePathToTarget);
            BehaviorAction MoveToTarget = new BehaviorAction(blackboard, ((NPC)(blackboard.BehaviorOwner)).MoveToTarget);
            Sequence sequenceWandering = new Sequence(blackboard, isSelected, CalculatePathToTarget, MoveToTarget);
            Selector selectorNPC = new Selector(blackboard, sequenceWandering);
            Behavior behaviorNPC = new Behavior(blackboard, selectorNPC);
            return behaviorNPC;
        }
        else if (name == "CreepBasic")
        {
            BehaviorAction isInAggroRange = new BehaviorAction(blackboard, ((Creep)(blackboard.BehaviorOwner)).IsInAggroRange);
            BehaviorAction AttackUnit = new BehaviorAction(blackboard, ((Creep)(blackboard.BehaviorOwner)).AttackUnit);
            Sequence selectorCreep = new Sequence(blackboard, isInAggroRange, AttackUnit);
            Behavior behaviorCreep = new Behavior(blackboard, selectorCreep);
            return behaviorCreep;
        }*/
        return null;
    }

    private void InitializeBehaviors()
    {
        
    }

}
