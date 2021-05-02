using FluentAssertions;
using Xunit;

namespace ClassicUO.UnitTests.Game.SerialHelper
{
    public class IsValid
    {
        [Theory]
        [InlineData(1)]
        [InlineData(0x1FFFFFFF)]
        [InlineData(0x7FFFFFFF)]
        public void IsValid_Serial_Should_Be_Legal(uint serial)
        {
            ClassicUO.Game.SerialHelper.IsValid(serial)
                .Should()
                .BeTrue();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(4294967295)] // guglielmo: first was 0x80000000, but was bugged for AddTooltip, we added the max uint value to fix the problem
        public void IsValid_Serial_Should_Not_Be_Legal(uint serial)
        {
            ClassicUO.Game.SerialHelper.IsValid(serial)
                .Should()
                .BeFalse();
        }
    }
}