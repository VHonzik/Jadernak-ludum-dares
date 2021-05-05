using System;
using System.Collections.Generic;

namespace BehaviorTree
{
    public abstract class BehaviorNode
    {
        protected BehaviorReturnCode ReturnCode;

        public Blackboard Blackboard { get; set; }

        public BehaviorNode(Blackboard _blackboard)
        {
            Blackboard = _blackboard;
        }

        public abstract BehaviorReturnCode Execute();

        public abstract void Reset();

        public abstract BehaviorNode Copy(Blackboard _blackboard);

    }
}
