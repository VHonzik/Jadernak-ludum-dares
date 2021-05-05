using System;
using System.Collections.Generic;


namespace BehaviorTree
{
    public abstract class Decorator : BehaviorNode
    {
        protected BehaviorNode DecoratedBehavior;

        public Decorator(Blackboard _blackboard, BehaviorNode _decoratedBehavior)
            : base(_blackboard)
        {
            DecoratedBehavior = _decoratedBehavior;
        }

        public override void Reset()
        {
            DecoratedBehavior.Reset();
        }
    }
}
