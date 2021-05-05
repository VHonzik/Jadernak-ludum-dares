using Entities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
    public class Blackboard
    {
        public string ErrorText {get; set; }

        public Actor BehaviorOwner { get; set; }

        public Blackboard()
        {

        }
    }
}
