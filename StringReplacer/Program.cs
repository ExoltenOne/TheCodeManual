using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringReplacer
{
    using BenchmarkDotNet.Running;

    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<StringReplacer>();

            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).RunAll();
        }
    }
}
