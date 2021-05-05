using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CardGame
{

    class WaitForCallback<TReturn>
    {
        public delegate TReturn FunctionWithCallBack(IEnumerator callback);

        private FunctionWithCallBack Function { get; set; }
        private bool _finished;

        public WaitForCallback(FunctionWithCallBack function)
        {
            Function = function;
            _finished = false;
        }

        private IEnumerator Done()
        {
            _finished = true;
            yield break;
        }
            
        public IEnumerator Do()
        {
            Function(Done());
           
            while(_finished == false)
            {
                yield return null;
            }
        }
    }
}
