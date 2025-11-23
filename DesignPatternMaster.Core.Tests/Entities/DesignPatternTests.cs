using DesignPatternMaster.Core.Entities;
using FluentAssertions;
using Xunit;

namespace DesignPatternMaster.Core.Tests.Entities
{
    public class DesignPatternTests
    {
        [Fact]
        public void DesignPattern_ShouldInitializeWithRequiredProperties()
        {
            // Arrange & Act
            var pattern = new DesignPattern
            {
                Id = "singleton",
                Name = "Singleton Pattern",
                Summary = "Ensures a class has only one instance",
                Category = "Creational",
                Difficulty = "Beginner",
                ModernRelevance = "Still widely used in modern applications"
            };

            // Assert
            pattern.Id.Should().Be("singleton");
            pattern.Name.Should().Be("Singleton Pattern");
            pattern.Summary.Should().Be("Ensures a class has only one instance");
            pattern.Category.Should().Be("Creational");
            pattern.Difficulty.Should().Be("Beginner");
            pattern.ModernRelevance.Should().Be("Still widely used in modern applications");
            pattern.IsAntiPattern.Should().BeFalse();
            pattern.Sections.Should().NotBeNull().And.BeEmpty();
            pattern.Tags.Should().NotBeNull().And.BeEmpty();
        }

        [Fact]
        public void DesignPattern_ShouldAllowSettingIsAntiPattern()
        {
            // Arrange
            var pattern = new DesignPattern
            {
                Id = "singleton",
                Name = "Singleton Pattern",
                Summary = "Test",
                Category = "Creational",
                Difficulty = "Beginner",
                ModernRelevance = "Test",
                IsAntiPattern = true
            };

            // Assert
            pattern.IsAntiPattern.Should().BeTrue();
        }

        [Fact]
        public void DesignPattern_ShouldAllowAddingSections()
        {
            // Arrange
            var pattern = new DesignPattern
            {
                Id = "test",
                Name = "Test",
                Summary = "Test",
                Category = "Test",
                Difficulty = "Test",
                ModernRelevance = "Test"
            };

            var section = new Section
            {
                Title = "Introduction",
                Content = "This is an introduction"
            };

            // Act
            pattern.Sections.Add(section);

            // Assert
            pattern.Sections.Should().HaveCount(1);
            pattern.Sections[0].Title.Should().Be("Introduction");
        }

        [Fact]
        public void DesignPattern_ShouldAllowAddingTags()
        {
            // Arrange
            var pattern = new DesignPattern
            {
                Id = "test",
                Name = "Test",
                Summary = "Test",
                Category = "Test",
                Difficulty = "Test",
                ModernRelevance = "Test"
            };

            // Act
            pattern.Tags.Add("design");
            pattern.Tags.Add("pattern");

            // Assert
            pattern.Tags.Should().HaveCount(2);
            pattern.Tags.Should().Contain(new[] { "design", "pattern" });
        }

        [Fact]
        public void DesignPattern_ShouldAllowSettingIconPath()
        {
            // Arrange
            var pattern = new DesignPattern
            {
                Id = "test",
                Name = "Test",
                Summary = "Test",
                Category = "Test",
                Difficulty = "Test",
                ModernRelevance = "Test",
                IconPath = "/icons/singleton.png"
            };

            // Assert
            pattern.IconPath.Should().Be("/icons/singleton.png");
        }
    }
}
