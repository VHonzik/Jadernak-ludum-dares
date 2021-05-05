using System;
using System.Collections.Generic;


namespace BehaviorTree
{
    public class BehaviorAction : BehaviorNode
    {

        private Func<BehaviorReturnCode> Action;

        public BehaviorAction(Blackboard _blackboard, Func<BehaviorReturnCode> _action)
            : base(_blackboard)
        {
            Action = _action;
        }

        public override BehaviorReturnCode Execute()
        {
            try
            {
                switch (Action.Invoke())
                {
                    case BehaviorReturnCode.Success:
                        ReturnCode = BehaviorReturnCode.Success;
                        return ReturnCode;
                    case BehaviorReturnCode.Failure:
                        ReturnCode = BehaviorReturnCode.Failure;
                        return ReturnCode;
                    case BehaviorReturnCode.Running:
                        ReturnCode = BehaviorReturnCode.Running;
                        return ReturnCode;
                    case BehaviorReturnCode.Error:
                        ReturnCode = BehaviorReturnCode.Error;
                        return ReturnCode;
                    default:
                        ReturnCode = BehaviorReturnCode.Failure;
                        return ReturnCode;
                }
            }
            catch (Exception e)
            {
                Blackboard.ErrorText = e.Message;
                ReturnCode = BehaviorReturnCode.Error;
                return ReturnCode;
            }
        }

        public override void Reset()
        {
            
        }

        public override BehaviorNode Copy(Blackboard freshBlackboard)
        {
            return new BehaviorAction(freshBlackboard, new Func<BehaviorReturnCode>(Action));
            
        }
    }

}
