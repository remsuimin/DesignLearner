using DesignPatternMaster.Core.Entities;
using FluentAssertions;
using Xunit;

namespace DesignPatternMaster.Core.Tests.Entities
{
    public class CodeSampleTests
    {
        [Fact]
        public void CodeSample_ShouldInitializeWithRequiredProperties()
        {
            // Arrange & Act
            var codeSample = new CodeSample(
                language: "csharp",
                code: "public class Example { }",
                description: "Example class");

            // Assert
            codeSample.Language.Should().Be("csharp");
            codeSample.Code.Should().Be("public class Example { }");
            codeSample.Description.Should().Be("Example class");
        }

        [Theory]
        [InlineData("csharp", "C# code")]
        [InlineData("javascript", "JS code")]
        [InlineData("python", "Python code")]
        public void CodeSample_ShouldSupportMultipleLanguages(string language, string code)
        {
            // Arrange & Act
            var codeSample = new CodeSample(
                language: language,
                code: code,
                description: "Test");

            // Assert
            codeSample.Language.Should().Be(language);
            codeSample.Code.Should().Be(code);
        }
    }
}
