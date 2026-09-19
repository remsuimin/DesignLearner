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
            var section = new Section(
                title: "Introduction",
                content: "This is the introduction content");

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
            var section = new Section(
                title: "Example",
                content: "Example content",
                codeSample: new CodeSample(
                    language: "csharp",
                    code: "var x = 10;",
                    description: "Variable declaration"));

            // Assert
            section.CodeSample.Should().NotBeNull();
            section.CodeSample!.Language.Should().Be("csharp");
        }

        [Fact]
        public void Section_ShouldAllowSettingImagePath()
        {
            // Arrange
            var section = new Section(
                title: "Diagram",
                content: "See diagram below",
                imagePath: "/images/diagram.png");

            // Assert
            section.ImagePath.Should().Be("/images/diagram.png");
        }
    }
}
