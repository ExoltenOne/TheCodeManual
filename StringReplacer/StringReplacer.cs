namespace StringReplacer
{
    using System;
    using System.Text.RegularExpressions;

    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Order;
    using System.Linq;
    using System.Text;

    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [RankColumn]
    [MemoryDiagnoser]
    [MarkdownExporterAttribute.GitHub]
    [ClrJob, CoreJob]
    public class StringReplacer
    {
        private readonly Regex pattern = new Regex("[;,\t\r ]|[\n]{2}", RegexOptions.Compiled);

        private readonly char[] separators = { ' ', ';', ',', '\r', '\t', '\n' };

        private readonly string[] separatorsStrings = { " ", ";", ",", "\r", "\t", "\n" };

        public string Value { get; } = "this;is,\ra\t\n\n\ntest";

        [Benchmark(Baseline = true, Description = "zgirod - MultipleReplaceMethod")]
        public string MultipleReplaceMethod()
        {
            var result = this.Value.Replace(';', '\n').Replace(',', '\n').Replace('\r', '\n').Replace('\t', '\n')
                .Replace(' ', '\n').Replace("\n\n", "\n");

            //while(result.Contains("\n\n"))
            //{
            //    result = result.Replace("\n\n", "\n");
            //}

            return result;
        }

        [Benchmark(Description = "kmadof - ReplaceByStringBuilder")]
        public string ReplaceByStringBuilder()
        {
            var builder = new StringBuilder(this.Value);
            builder.Replace(';', '\n').Replace(',', '\n').Replace('\r', '\n').Replace('\t', '\n')
                .Replace(' ', '\n').Replace("\n\n", "\n");

            return builder.ToString();
        }

        //[Benchmark(Description = "johnluetke - Regex")]
        public string Regex()
        {
            var str = this.pattern.Replace(this.Value, "\n");

            //while (str.Contains("\n\n"))
            //{
            //    str = str.Replace("\n\n", "\n");
            //}

            return str;
        }

        [Benchmark(Description = "Paul Walls - ReplaceUsingSplit")]
        public string ReplaceUsingSplit()
        {
            var temp = this.Value.Split(this.separators, StringSplitOptions.RemoveEmptyEntries);
            return string.Join("\n", temp);
        }

        //[Benchmark(Description = "dodgy_coder - ReplaceUsingAggregate")]
        public string ReplaceUsingAggregate()
        {
            var str = this.separators.Aggregate(this.Value, (c1, c2) => c1.Replace(c2, '\n'));

            //while (str.Contains("\n\n"))
            //{
            //    str = str.Replace("\n\n", "\n");
            //}

            return str;
        }

        //[Benchmark(Description = "John Whiter - ReplaceUsingStringBuilderAndCharArray")]
        public string ReplaceUsingStringBuilderAndCharArray()
        {
            var str = StringUtils.ReplaceAny(this.Value, '\n', this.separators);

            //while (str.Contains("\n\n"))
            //{
            //    str = str.Replace("\n\n", "\n");
            //}

            return str;
        }

        //[Benchmark(Description = "Fab - ReplaceUsingStringBuilderAndHashSet")]
        public string ReplaceUsingStringBuilderAndHashSet()
        {
            var str = StringUtils.MultiReplace(this.Value, this.separators, '\n');

            //while (str.Contains("\n\n"))
            //{
            //    str = str.Replace("\n\n", "\n");
            //}

            return str;
        }

        //[Benchmark(Description = "Daniel Székely - ReplaceInForeach")]
        public string ReplaceInForeach()
        {
            var str = this.Value;
            foreach (var singleChar in this.separators)
            {
                str = str.Replace(singleChar, '\n');
            }

            //while (str.Contains("\n\n"))
            //{
            //    str = str.Replace("\n\n", "\n");
            //}

            return str;
        }

        //[Benchmark(Description = "sɐunıɔןɐqɐp - ReplaceUsingStringBuilderAndIndexOf")]
        public string ReplaceUsingStringBuilderAndIndexOf()
        {
            var str = this.Value.ReplaceAll(this.separatorsStrings, "\n");

            //while (str.Contains("\n\n"))
            //{
            //    str = str.Replace("\n\n", "\n");
            //}

            return str;
        }

        //[Benchmark(Description = "kmadof - UnsafeReplace")]
        public unsafe string UnsafeReplace()
        {
            var value = string.Copy(this.Value);

            fixed (char* pfixed = value)
            {
                for (char* p = pfixed; *p != 0; p++)
                {
                    foreach(var oldChar in this.separators)
                    {
                        if(*p == oldChar)
                        {
                            *p = '\n';
                            break;
                        }
                    }
                }
            }

            //while (value.Contains("\n\n"))
            //{
            //    value = value.Replace("\n\n", "\n");
            //}

            return value;
        }
    }
}
