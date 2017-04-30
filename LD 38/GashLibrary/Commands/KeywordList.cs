using Gash;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace GashLibrary.Commands
{
    class KeywordList : IEnumerable<IKeyword>
    {
        private List<IKeyword> KnownKeywords = new List<IKeyword>();
        private Dictionary<string, IKeyword> KnownKeywordsMap = new Dictionary<string, IKeyword>();

        public void RegisterKeyword(IKeyword keyword)
        {
            KnownKeywords.Add(keyword);
            KnownKeywordsMap.Add(keyword.Name, keyword);
        }

        internal bool FindMan(string keyword)
        {
            IKeyword result = null;
            if (KnownKeywordsMap.TryGetValue(keyword, out result))
            {
                result.PrintManPage();
                return true;
            }
            return false;
        }

        public IEnumerator<IKeyword> GetEnumerator()
        {
            return KnownKeywords.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return KnownKeywords.GetEnumerator();
        }
    }
}
