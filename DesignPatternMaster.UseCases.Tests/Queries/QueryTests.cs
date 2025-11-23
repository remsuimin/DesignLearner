using DesignPatternMaster.Core.Entities;
using DesignPatternMaster.Core.Interfaces;
using DesignPatternMaster.UseCases.Queries;
using FluentAssertions;
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
            _query = new GetPatternListQuery(_mockRepository.Object);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnAllPatterns()
        {
            // Arrange
            var expectedPatterns = new List<DesignPattern>
            {
                new DesignPattern
                {
                    Id = "singleton",
                    Name = "Singleton",
                    Summary = "Test",
                    Category = "Creational",
                    Difficulty = "Beginner",
                    ModernRelevance = "Test"
                },
                new DesignPattern
                {
                    Id = "factory",
                    Name = "Factory",
                    Summary = "Test",
                    Category = "Creational",
                    Difficulty = "Intermediate",
                    ModernRelevance = "Test"
                }
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
        }
    }

    public class GetPatternDetailQueryTests
    {
        private readonly Mock<IPatternRepository> _mockRepository;
        private readonly GetPatternDetailQuery _query;

        public GetPatternDetailQueryTests()
        {
            _mockRepository = new Mock<IPatternRepository>();
            _query = new GetPatternDetailQuery(_mockRepository.Object);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnPattern_WhenIdExists()
        {
            // Arrange
            var expectedPattern = new DesignPattern
            {
                Id = "singleton",
                Name = "Singleton",
                Summary = "Test",
                Category = "Creational",
                Difficulty = "Beginner",
                ModernRelevance = "Test"
            };

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
        public async Task ExecuteAsync_ShouldReturnNull_WhenIdDoesNotExist()
        {
            // Arrange
            _mockRepository
                .Setup(r => r.GetPatternByIdAsync("nonexistent"))
                .ReturnsAsync((DesignPattern?)null);

            // Act
            var result = await _query.ExecuteAsync("nonexistent");

            // Assert
            result.Should().BeNull();
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
                .ReturnsAsync((DesignPattern?)null);

            // Act
            await _query.ExecuteAsync(patternId);

            // Assert
            _mockRepository.Verify(r => r.GetPatternByIdAsync(patternId), Times.Once);
        }
    }
}
