using System;
using System.Collections.Generic;
using System.Text;

namespace GashLibrary.Commands
{
    public interface IKeyword
    {
        string Name { get; }
        string ColoredName { get; }
        void PrintManPage();
    }
}
