using GashLibrary.Commands;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Game
{
    class Actions : IEnumerable<Action>
    {
        public List<Action> ActionTypes = new List<Action>();

        public Actions()
        {
            for(int i=0; i < Action.Names.Length; i++)
            {
                ActionTypes.Add(new Action((Action.ActionType)i));
            }
        }

        public IEnumerator<Action> GetEnumerator()
        {
            return ActionTypes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ActionTypes.GetEnumerator();
        }
    }
}
