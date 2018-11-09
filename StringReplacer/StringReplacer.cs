namespace StringReplacer
{
    using System;
    using System.Text.RegularExpressions;

    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Order;
    using System.Linq;

    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [RankColumn]
    [MemoryDiagnoser]
    public class StringReplacer
    {
        private readonly Regex pattern = new Regex("[;,\t\r ]|[\n]{2}", RegexOptions.Compiled);

        private readonly char[] separators = { ' ', ';', ',', '\r', '\t', '\n' };

        private readonly string[] separatorsStrings = { " ", ";", ",", "\r", "\t", "\n" };

        [Params(@"this;is,\ra\t\n\n\ntest")]
        public string Value { get; set; }

        [Benchmark(Baseline = true, Description = "zgirod - MultipleReplaceMethod")]
        public string MultipleReplaceMethod()
        {
            return this.Value.Replace(';', '\n').Replace(',', '\n').Replace('\r', '\n').Replace('\t', '\n')
                .Replace(' ', '\n').Replace("\n\n", "\n");
        }

        [Benchmark(Description = "johnluetke - Regex")]
        public string Regex()
        {
            return this.pattern.Replace(this.Value, "\n");
        }

        [Benchmark(Description = "Paul Walls - ReplaceUsingSplit")]
        public string ReplaceUsingSplit()
        {
            var temp = this.Value.Split(this.separators, StringSplitOptions.RemoveEmptyEntries);
            return string.Join("\n", temp);
        }

        [Benchmark(Description = "dodgy_coder - ReplaceUsingAggregate")]
        public string ReplaceUsingAggregate()
        {
            return this.separators.Aggregate(this.Value, (c1, c2) => c1.Replace(c2, '\n'));
        }

        [Benchmark(Description = "John Whiter - ReplaceUsingStringBuilderAndCharArray")]
        public string ReplaceUsingStringBuilderAndCharArray()
        {
            return StringUtils.ReplaceAny(this.Value, '\n', this.separators);
        }

        [Benchmark(Description = "Fab - ReplaceUsingStringBuilderAndHashSet")]
        public string ReplaceUsingStringBuilderAndHashSet()
        {
            return StringUtils.MultiReplace(this.Value, this.separators, '\n');
        }

        [Benchmark(Description = "Daniel Székely - ReplaceInForeach")]
        public string ReplaceInForeach()
        {
            var str = this.Value;
            foreach (var singleChar in this.separators)
            {
                str = str.Replace(singleChar, '_');
            }

            return str;
        }

        [Benchmark(Description = "sɐunıɔןɐqɐp - ReplaceUsingStringBuilderAndIndexOf")]
        public string ReplaceUsingStringBuilderAndIndexOf()
        {
            return this.Value.ReplaceAll(this.separatorsStrings, "\n");
        }
    }
}
