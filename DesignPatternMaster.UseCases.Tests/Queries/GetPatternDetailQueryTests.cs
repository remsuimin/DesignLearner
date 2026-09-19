using DesignPatternMaster.Core.Entities;
using DesignPatternMaster.Core.Enums;
using DesignPatternMaster.Core.Interfaces;
using DesignPatternMaster.UseCases.Exceptions;
using DesignPatternMaster.UseCases.Queries;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace DesignPatternMaster.UseCases.Tests.Queries
{
    public class GetPatternDetailQueryTests
    {
        private readonly Mock<IPatternRepository> _mockRepository;
        private readonly GetPatternDetailQuery _query;

        public GetPatternDetailQueryTests()
        {
            _mockRepository = new Mock<IPatternRepository>();
            _query = new GetPatternDetailQuery(_mockRepository.Object, Mock.Of<ILogger<GetPatternDetailQuery>>());
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnPattern_WhenIdExists()
        {
            // Arrange
            var expectedPattern = new DesignPattern(
                id: "singleton",
                name: "Singleton",
                summary: "Test",
                category: PatternCategory.Creational,
                difficulty: DifficultyLevel.Beginner,
                modernRelevance: "Test");

            _mockRepository
                .Setup(r => r.GetPatternByIdAsync("singleton"))
                .ReturnsAsync(expectedPattern);

            // Act
            var result = await _query.ExecuteAsync("singleton");

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedPattern);
            _mockRepository.Verify(r => r.GetPatternByIdAsync("singleton"), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldThrowPatternNotFoundException_WhenIdDoesNotExist()
        {
            // Arrange
            _mockRepository
                .Setup(r => r.GetPatternByIdAsync("nonexistent"))
                .ReturnsAsync((DesignPattern?)null);

            // Act
            var act = async () => await _query.ExecuteAsync("nonexistent");

            // Assert
            await act.Should().ThrowAsync<PatternNotFoundException>();
            _mockRepository.Verify(r => r.GetPatternByIdAsync("nonexistent"), Times.Once);
        }

        [Theory]
        [InlineData("singleton")]
        [InlineData("factory")]
        [InlineData("observer")]
        public async Task ExecuteAsync_ShouldCallRepositoryWithCorrectId(string patternId)
        {
            // Arrange
            _mockRepository
                .Setup(r => r.GetPatternByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new DesignPattern(
                    id: "singleton",
                    name: "Singleton",
                    summary: "Test",
                    category: PatternCategory.Creational,
                    difficulty: DifficultyLevel.Beginner,
                    modernRelevance: "Test"));

            // Act
            await _query.ExecuteAsync(patternId);

            // Assert
            _mockRepository.Verify(r => r.GetPatternByIdAsync(patternId), Times.Once);
        }

        [Fact]
        public void Constructor_ShouldThrow_WhenRepositoryIsNull()
        {
            var act = () => new GetPatternDetailQuery(null!, Mock.Of<ILogger<GetPatternDetailQuery>>());
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Constructor_ShouldThrow_WhenLoggerIsNull()
        {
            var act = () => new GetPatternDetailQuery(Mock.Of<IPatternRepository>(), null!);
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task ExecuteAsync_ShouldThrow_WhenIdIsEmpty()
        {
            var act = async () => await _query.ExecuteAsync("");
            await act.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task ExecuteAsync_ShouldThrow_WhenIdIsWhitespace()
        {
            var act = async () => await _query.ExecuteAsync("   ");
            await act.Should().ThrowAsync<ArgumentException>();
        }
    }
}
