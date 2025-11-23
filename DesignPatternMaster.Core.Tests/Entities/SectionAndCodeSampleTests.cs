using DesignPatternMaster.Core.Entities;
using FluentAssertions;
using Xunit;

namespace DesignPatternMaster.Core.Tests.Entities
{
    public class SectionTests
    {
        [Fact]
        public void Section_ShouldInitializeWithRequiredProperties()
        {
            // Arrange & Act
            var section = new Section
            {
                Title = "Introduction",
                Content = "This is the introduction content"
            };

            // Assert
            section.Title.Should().Be("Introduction");
            section.Content.Should().Be("This is the introduction content");
            section.CodeSample.Should().BeNull();
            section.ImagePath.Should().BeNull();
        }

        [Fact]
        public void Section_ShouldAllowSettingCodeSample()
        {
            // Arrange
            var section = new Section
            {
                Title = "Example",
                Content = "Example content",
                CodeSample = new CodeSample
                {
                    Language = "csharp",
                    Code = "var x = 10;",
                    Description = "Variable declaration"
                }
            };

            // Assert
            section.CodeSample.Should().NotBeNull();
            section.CodeSample!.Language.Should().Be("csharp");
        }

        [Fact]
        public void Section_ShouldAllowSettingImagePath()
        {
            // Arrange
            var section = new Section
            {
                Title = "Diagram",
                Content = "See diagram below",
                ImagePath = "/images/diagram.png"
            };

            // Assert
            section.ImagePath.Should().Be("/images/diagram.png");
        }
    }

    public class CodeSampleTests
    {
        [Fact]
        public void CodeSample_ShouldInitializeWithRequiredProperties()
        {
            // Arrange & Act
            var codeSample = new CodeSample
            {
                Language = "csharp",
                Code = "public class Example { }",
                Description = "Example class"
            };

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
            var codeSample = new CodeSample
            {
                Language = language,
                Code = code,
                Description = "Test"
            };

            // Assert
            codeSample.Language.Should().Be(language);
            codeSample.Code.Should().Be(code);
        }
    }
}
