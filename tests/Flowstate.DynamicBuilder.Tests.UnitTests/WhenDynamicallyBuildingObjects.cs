using Xunit;
using Xunit.Abstractions;

namespace Flowstate.DynamicBuilder.Tests.UnitTests
{
    public class WhenDynamicallyBuildingObjects
    {
        private readonly ITestOutputHelper _output;

        public WhenDynamicallyBuildingObjects(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void BuiltObjectHasExpectedPropertyValues()
        {
            const string Property01Value = "abc";
            const int Property02Value = 123;
            const bool Property03Value = true;

            var testDtoBuilder = DynamicBuilderFactory.Create<ITestDtoBuilder, TestDto>(_output);

            testDtoBuilder
                .WithProperty01(Property01Value)
                .WithProperty02(Property02Value)
                .WithProperty03(Property03Value);

            var testDto = testDtoBuilder.Build();

            Assert.NotNull(testDto);
            Assert.Equal(Property01Value, testDto.Property01);
            Assert.Equal(Property02Value, testDto.Property02);
            Assert.Equal(Property03Value, testDto.Property03);
        }

        public class TestDto
        {
            public string? Property01 { get; set; }
            public int Property02 { get; set; }
            public bool Property03 { get; set; }
        }

        public interface ITestDtoBuilder
        {
            ITestDtoBuilder WithProperty01(string p);
            ITestDtoBuilder WithProperty02(int p);
            ITestDtoBuilder WithProperty03(bool p);

            TestDto Build();
        }
    }
}