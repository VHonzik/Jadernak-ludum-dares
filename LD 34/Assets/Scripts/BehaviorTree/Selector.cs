using System;
using System.Collections.Generic;


namespace BehaviorTree
{
    public class Selector : Composite
    {
        public Selector(Blackboard _blackboard, params BehaviorNode[] _behaviorNodes)
            : base(_blackboard, _behaviorNodes)
        {

        }

        public override BehaviorReturnCode Execute()
        {
            try
            {
                for (; LastBehavior < BehaviorNodes.Length; LastBehavior++)
                {
                    switch (BehaviorNodes[LastBehavior].Execute())
                    {
                        case BehaviorReturnCode.Failure:
                            continue;
                        case BehaviorReturnCode.Success:
                            LastBehavior = 0;
                            ReturnCode = BehaviorReturnCode.Success;
                            return ReturnCode;
                        case BehaviorReturnCode.Running:
                            ReturnCode = BehaviorReturnCode.Running;
                            return ReturnCode;
                        case BehaviorReturnCode.Error:
                            ReturnCode = BehaviorReturnCode.Error;
                            return ReturnCode;
                        default:
                            continue;
                    }
                }
                LastBehavior = 0;
                ReturnCode = BehaviorReturnCode.Failure;
                return ReturnCode;
            }
            catch (Exception e)
            {
                Blackboard.ErrorText = e.Message;
                ReturnCode = BehaviorReturnCode.Error;
                return ReturnCode;
            }            
        }

        public override BehaviorNode Copy(Blackboard freshBlackboard)
        {
            BehaviorNode[] freshBehaviorNodes = new BehaviorNode[BehaviorNodes.Length];
            for (int i = 0; i < BehaviorNodes.Length; i++)
            {
                freshBehaviorNodes[i] = BehaviorNodes[i].Copy(freshBlackboard);
            }
            return new Selector(freshBlackboard, freshBehaviorNodes);
        }
    }
}
