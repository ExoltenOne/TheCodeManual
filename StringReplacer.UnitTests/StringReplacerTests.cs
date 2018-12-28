using FluentAssertions;
using Xunit;

namespace StringReplacer.UnitTests
{
    public class StringReplacerTests
    {
        private const string original = "this;is,\ra\t\n\n\ntest";
        private const string expected = "this\nis\na\ntest";
        private readonly StringReplacer replacer = new StringReplacer();

        [Fact]
        public void ShouldMultipleReplaceMethodReturnsValidValue()
        {
            replacer.MultipleReplaceMethod().Should().Be(expected);
            replacer.Value.Should().Be(original);
        }

        [Fact]
        public void ShouldRegexReturnsValidValue()
        {
            replacer.Regex().Should().Be(expected);
            replacer.Value.Should().Be(original);
        }

        [Fact]
        public void ShouldReplaceUsingSplitReturnsValidValue()
        {
            replacer.ReplaceUsingSplit().Should().Be(expected);
            replacer.Value.Should().Be(original);
        }

        [Fact]
        public void ShouldReplaceUsingAggregateReturnsValidValue()
        {
            replacer.ReplaceUsingAggregate().Should().Be(expected);
            replacer.Value.Should().Be(original);
        }

        [Fact]
        public void ShouldReplaceUsingStringBuilderAndCharArrayReturnsValidValue()
        {
            replacer.ReplaceUsingStringBuilderAndCharArray().Should().Be(expected);
            replacer.Value.Should().Be(original);
        }

        [Fact]
        public void ShouldReplaceUsingStringBuilderAndHashSetReturnsValidValue()
        {
            replacer.ReplaceUsingStringBuilderAndHashSet().Should().Be(expected);
            replacer.Value.Should().Be(original);
        }

        [Fact]
        public void ShouldReplaceInForeachReturnsValidValue()
        {
            replacer.ReplaceInForeach().Should().Be(expected);
            replacer.Value.Should().Be(original);
        }

        [Fact]
        public void ShouldReplaceUsingStringBuilderAndIndexOfReturnsValidValue()
        {
            replacer.ReplaceUsingStringBuilderAndIndexOf().Should().Be(expected);
            replacer.Value.Should().Be(original);
        }

        [Fact]
        public void ShouldUnsafeReplaceReturnsValidValue()
        {
            replacer.UnsafeReplace().Should().Be(expected);
            replacer.Value.Should().Be(original);
        }
    }
}
