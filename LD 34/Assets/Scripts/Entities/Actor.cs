using UnityEngine;
using System.Collections;
using BehaviorTree;
using System.Collections.Generic;


namespace Entities
{
 
    public abstract class Actor : MonoBehaviour
    {
        protected List<Behavior> AssignedBehaviors = new List<Behavior>();
        protected int frames;
        protected Task currentTask;

        public GameObject Target; 

        public void AppendBehaviors(params string[] behaviors)
        {
            Blackboard blackboard = new Blackboard();
            blackboard.BehaviorOwner = this;
            
            foreach (string s in behaviors)
            {
                AssignedBehaviors.Add(AIManager.Instance.GetBehavior(s, blackboard));
            }

        }
        
        protected virtual void Update()
        {
            frames++;
            if (frames % 30 == 0)
            {
                foreach (Behavior behavior in AssignedBehaviors)
                {
                    if (behavior.Execute() == BehaviorReturnCode.Error)
                    {
                        behavior.Reset();
                        if (!string.IsNullOrEmpty(behavior.Blackboard.ErrorText))
                        {
                            Debug.Log(behavior.Blackboard.ErrorText);
                            behavior.Blackboard.ErrorText = null;
                        }

                    }

                }
                frames = 0;
            }
        }
        
    }
}