using DesignPatternMaster.Core.Entities;
using DesignPatternMaster.Core.Enums;
using FluentAssertions;
using Xunit;

namespace DesignPatternMaster.Core.Tests.Entities
{
    public class DesignPatternTests
    {
        private static readonly string[] s_expectedTags = { "design", "pattern" };

        [Fact]
        public void DesignPattern_ShouldInitializeWithRequiredProperties()
        {
            // Arrange & Act
            var pattern = new DesignPattern(
                id: "singleton",
                name: "Singleton Pattern",
                summary: "Ensures a class has only one instance",
                category: PatternCategory.Creational,
                difficulty: DifficultyLevel.Beginner,
                modernRelevance: "Still widely used in modern applications");

            // Assert
            pattern.Id.Should().Be("singleton");
            pattern.Name.Should().Be("Singleton Pattern");
            pattern.Summary.Should().Be("Ensures a class has only one instance");
            pattern.Category.Should().Be(PatternCategory.Creational);
            pattern.Difficulty.Should().Be(DifficultyLevel.Beginner);
            pattern.ModernRelevance.Should().Be("Still widely used in modern applications");
            pattern.IsAntiPattern.Should().BeFalse();
            pattern.IsModern.Should().BeFalse();
            pattern.IconPath.Should().BeNull();
            pattern.Sections.Should().NotBeNull().And.BeEmpty();
            pattern.Tags.Should().NotBeNull().And.BeEmpty();
        }

        [Fact]
        public void DesignPattern_ShouldAllowSettingIsAntiPattern()
        {
            // Arrange
            var pattern = new DesignPattern(
                id: "singleton",
                name: "Singleton Pattern",
                summary: "Test",
                category: PatternCategory.Creational,
                difficulty: DifficultyLevel.Beginner,
                modernRelevance: "Test",
                isAntiPattern: true);

            // Assert
            pattern.IsAntiPattern.Should().BeTrue();
        }

        [Fact]
        public void DesignPattern_ShouldAllowAddingSections()
        {
            // Arrange
            var pattern = new DesignPattern(
                id: "test",
                name: "Test",
                summary: "Test",
                category: PatternCategory.Creational,
                difficulty: DifficultyLevel.Beginner,
                modernRelevance: "Test");

            var section = new Section(
                title: "Introduction",
                content: "This is an introduction");

            // Act
            pattern.AddSection(section);

            // Assert
            pattern.Sections.Should().HaveCount(1);
            pattern.Sections[0].Title.Should().Be("Introduction");
        }

        [Fact]
        public void DesignPattern_ShouldAllowAddingTags()
        {
            // Arrange
            var pattern = new DesignPattern(
                id: "test",
                name: "Test",
                summary: "Test",
                category: PatternCategory.Creational,
                difficulty: DifficultyLevel.Beginner,
                modernRelevance: "Test");

            // Act
            pattern.AddTag("design");
            pattern.AddTag("pattern");

            // Assert
            pattern.Tags.Should().HaveCount(2);
            pattern.Tags.Should().Contain(s_expectedTags);
        }

        [Fact]
        public void DesignPattern_ShouldAllowSettingIconPath()
        {
            // Arrange
            var pattern = new DesignPattern(
                id: "test",
                name: "Test",
                summary: "Test",
                category: PatternCategory.Creational,
                difficulty: DifficultyLevel.Beginner,
                modernRelevance: "Test",
                iconPath: "/icons/singleton.png");

            // Assert
            pattern.IconPath.Should().Be("/icons/singleton.png");
        }
    }
}
