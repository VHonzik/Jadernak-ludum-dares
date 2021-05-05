using System;
using System.Collections.Generic;


namespace BehaviorTree
{
    public enum BehaviorReturnCode
    {
        Failure,
        Success,
        Running,
        Error
    }

    public class Behavior : BehaviorNode 
    {

        private BehaviorNode Root;
        
        public Behavior(Blackboard _blackboard, BehaviorNode _root)
            : base(_blackboard)
        {
            Root = _root;
            
        }

        public override BehaviorReturnCode Execute()
        {
            try
            {
                switch (Root.Execute())
                {
                    case BehaviorReturnCode.Failure:
                        ReturnCode = BehaviorReturnCode.Failure;
                        return ReturnCode;
                    case BehaviorReturnCode.Success:
                        ReturnCode = BehaviorReturnCode.Success;
                        return ReturnCode;
                    case BehaviorReturnCode.Running:
                        ReturnCode = BehaviorReturnCode.Running;
                        return ReturnCode;
                    case BehaviorReturnCode.Error:
                        ReturnCode = BehaviorReturnCode.Error;
                        return ReturnCode;
                    default:
                        ReturnCode = BehaviorReturnCode.Running;
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
            Root.Reset();
        }
        
        public override BehaviorNode Copy(Blackboard freshBlackboard)
        {
            return new Behavior(freshBlackboard, Root.Copy(freshBlackboard));            
        }

    }
}
