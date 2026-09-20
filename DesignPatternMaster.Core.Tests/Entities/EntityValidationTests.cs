using DesignPatternMaster.Core.Entities;
using DesignPatternMaster.Core.Enums;
using FluentAssertions;
using Xunit;

namespace DesignPatternMaster.Core.Tests.Entities
{
    public class EntityValidationTests
    {
        // DesignPattern id
        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("   ")]
        public void DesignPattern_ShouldThrow_WhenIdIsEmptyOrWhitespace(string id)
        {
            var act = () => new DesignPattern(
                id: id,
                name: "Singleton",
                summary: "Test",
                category: PatternCategory.Creational,
                difficulty: DifficultyLevel.Beginner,
                modernRelevance: "Test");

            act.Should().Throw<ArgumentException>().WithParameterName(nameof(id));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("   ")]
        public void DesignPattern_ShouldThrow_WhenNameIsEmptyOrWhitespace(string name)
        {
            var act = () => new DesignPattern(
                id: "singleton",
                name: name,
                summary: "Test",
                category: PatternCategory.Creational,
                difficulty: DifficultyLevel.Beginner,
                modernRelevance: "Test");

            act.Should().Throw<ArgumentException>().WithParameterName(nameof(name));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("   ")]
        public void DesignPattern_ShouldThrow_WhenSummaryIsEmptyOrWhitespace(string summary)
        {
            var act = () => new DesignPattern(
                id: "singleton",
                name: "Singleton",
                summary: summary,
                category: PatternCategory.Creational,
                difficulty: DifficultyLevel.Beginner,
                modernRelevance: "Test");

            act.Should().Throw<ArgumentException>().WithParameterName(nameof(summary));
        }

        // Section
        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("   ")]
        public void Section_ShouldThrow_WhenTitleIsEmptyOrWhitespace(string title)
        {
            var act = () => new Section(title: title, content: "content");
            act.Should().Throw<ArgumentException>().WithParameterName(nameof(title));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("   ")]
        public void Section_ShouldThrow_WhenContentIsEmptyOrWhitespace(string content)
        {
            var act = () => new Section(title: "Title", content: content);
            act.Should().Throw<ArgumentException>().WithParameterName(nameof(content));
        }

        // CodeSample
        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("   ")]
        public void CodeSample_ShouldThrow_WhenLanguageIsEmptyOrWhitespace(string language)
        {
            var act = () => new CodeSample(language: language, code: "var x = 1;", description: "test");
            act.Should().Throw<ArgumentException>().WithParameterName(nameof(language));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("   ")]
        public void CodeSample_ShouldThrow_WhenCodeIsEmptyOrWhitespace(string code)
        {
            var act = () => new CodeSample(language: "csharp", code: code, description: "test");
            act.Should().Throw<ArgumentException>().WithParameterName(nameof(code));
        }

        [Fact]
        public void DesignPattern_AddSection_ShouldThrow_WhenNull()
        {
            var pattern = new DesignPattern(
                id: "test",
                name: "Test",
                summary: "Test",
                category: PatternCategory.Creational,
                difficulty: DifficultyLevel.Beginner,
                modernRelevance: "Test");

            var act = () => pattern.AddSection(null!);
            act.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("   ")]
        public void DesignPattern_AddTag_ShouldThrow_WhenEmptyOrWhitespace(string tag)
        {
            var pattern = new DesignPattern(
                id: "test",
                name: "Test",
                summary: "Test",
                category: PatternCategory.Creational,
                difficulty: DifficultyLevel.Beginner,
                modernRelevance: "Test");

            var act = () => pattern.AddTag(tag);
            act.Should().Throw<ArgumentException>().WithParameterName(nameof(tag));
        }

        [Fact]
        public void DesignPattern_AddTag_ShouldTrimWhitespace()
        {
            var pattern = new DesignPattern(
                id: "test",
                name: "Test",
                summary: "Test",
                category: PatternCategory.Creational,
                difficulty: DifficultyLevel.Beginner,
                modernRelevance: "Test");

            pattern.AddTag(" design ");

            pattern.Tags.Should().ContainSingle().Which.Should().Be("design");
        }

        [Fact]
        public void DesignPattern_AddTag_ShouldDeduplicate()
        {
            var pattern = new DesignPattern(
                id: "test",
                name: "Test",
                summary: "Test",
                category: PatternCategory.Creational,
                difficulty: DifficultyLevel.Beginner,
                modernRelevance: "Test");

            pattern.AddTag("design");
            pattern.AddTag("design");

            pattern.Tags.Should().HaveCount(1);
        }

        [Fact]
        public void DesignPattern_AddTag_ShouldDeduplicate_AfterTrim()
        {
            var pattern = new DesignPattern(
                id: "test",
                name: "Test",
                summary: "Test",
                category: PatternCategory.Creational,
                difficulty: DifficultyLevel.Beginner,
                modernRelevance: "Test");

            pattern.AddTag("design");
            pattern.AddTag(" design ");

            pattern.Tags.Should().HaveCount(1);
        }
    }
}
