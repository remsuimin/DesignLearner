using DesignPatternMaster.Core.Entities;
using DesignPatternMaster.Core.Enums;
using DesignPatternMaster.Core.Interfaces;
using DesignPatternMaster.UseCases.Queries;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace DesignPatternMaster.UseCases.Tests.Queries
{
    public class GetPatternListQueryTests
    {
        private readonly Mock<IPatternRepository> _mockRepository;
        private readonly GetPatternListQuery _query;

        public GetPatternListQueryTests()
        {
            _mockRepository = new Mock<IPatternRepository>();
            _query = new GetPatternListQuery(_mockRepository.Object, Mock.Of<ILogger<GetPatternListQuery>>());
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnAllPatterns()
        {
            // Arrange
            var expectedPatterns = new List<DesignPattern>
            {
                new DesignPattern(
                    id: "singleton",
                    name: "Singleton",
                    summary: "Test",
                    category: PatternCategory.Creational,
                    difficulty: DifficultyLevel.Beginner,
                    modernRelevance: "Test"),
                new DesignPattern(
                    id: "factory",
                    name: "Factory",
                    summary: "Test",
                    category: PatternCategory.Creational,
                    difficulty: DifficultyLevel.Intermediate,
                    modernRelevance: "Test")
            };

            _mockRepository
                .Setup(r => r.GetAllPatternsAsync())
                .ReturnsAsync(expectedPatterns);

            // Act
            var result = await _query.ExecuteAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Should().BeEquivalentTo(expectedPatterns);
            _mockRepository.Verify(r => r.GetAllPatternsAsync(), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnEmptyList_WhenNoPatternsExist()
        {
            // Arrange
            _mockRepository
                .Setup(r => r.GetAllPatternsAsync())
                .ReturnsAsync(new List<DesignPattern>());

            // Act
            var result = await _query.ExecuteAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            _mockRepository.Verify(r => r.GetAllPatternsAsync(), Times.Once);
        }

        [Fact]
        public void Constructor_ShouldThrow_WhenRepositoryIsNull()
        {
            var act = () => new GetPatternListQuery(null!, Mock.Of<ILogger<GetPatternListQuery>>());
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Constructor_ShouldThrow_WhenLoggerIsNull()
        {
            var act = () => new GetPatternListQuery(Mock.Of<IPatternRepository>(), null!);
            act.Should().Throw<ArgumentNullException>();
        }
    }
}
