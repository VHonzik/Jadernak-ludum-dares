using System;
using System.Collections.Generic;


namespace BehaviorTree
{
    public abstract class Composite : BehaviorNode
    {
        protected BehaviorNode[] BehaviorNodes;
        protected int LastBehavior;
        
        public Composite(Blackboard _blackboard, BehaviorNode[] _behaviorNodes)
            : base(_blackboard)
        {
            BehaviorNodes = _behaviorNodes;
            LastBehavior = 0;
        }

        public override void Reset()
        {
            foreach (BehaviorNode node in BehaviorNodes)
            {
                LastBehavior = 0;
                node.Reset();
            }
        }
    }
}
