using DesignPatternMaster.Core.Entities;
using DesignPatternMaster.Core.Enums;
using DesignPatternMaster.Infrastructure.Repositories;
using FluentAssertions;
using System.Text.Json;
using Xunit;

namespace DesignPatternMaster.Infrastructure.Tests.Repositories
{
    public class JsonPatternRepositoryTests : IDisposable
    {
        private readonly string _testFilePath;
        private readonly string _testDirectory;

        public JsonPatternRepositoryTests()
        {
            _testDirectory = Path.Combine(Path.GetTempPath(), "DesignPatternMaster.Tests", Guid.NewGuid().ToString());
            Directory.CreateDirectory(_testDirectory);
            _testFilePath = Path.Combine(_testDirectory, "test_patterns.json");
        }

        public void Dispose()
        {
            if (Directory.Exists(_testDirectory))
            {
                Directory.Delete(_testDirectory, true);
            }
        }

        [Fact]
        public async Task GetAllPatternsAsync_ShouldReturnAllPatterns_WhenFileExists()
        {
            // Arrange
            var testPatterns = new List<DesignPattern>
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

            var json = JsonSerializer.Serialize(testPatterns);
            await File.WriteAllTextAsync(_testFilePath, json);

            var repository = new JsonPatternRepository(_testFilePath);

            // Act
            var result = await repository.GetAllPatternsAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Select(p => p.Id).Should().Contain(new[] { "singleton", "factory" });
        }

        [Fact]
        public async Task GetAllPatternsAsync_ShouldThrowFileNotFoundException_WhenFileDoesNotExist()
        {
            // Arrange
            var nonExistentPath = Path.Combine(_testDirectory, "nonexistent.json");
            var repository = new JsonPatternRepository(nonExistentPath);

            // Act
            var act = async () => await repository.GetAllPatternsAsync();

            // Assert
            await act.Should().ThrowAsync<FileNotFoundException>();
        }

        [Fact]
        public async Task GetPatternByIdAsync_ShouldReturnPattern_WhenIdExists()
        {
            // Arrange
            var testPatterns = new List<DesignPattern>
            {
                new DesignPattern(
                    id: "singleton",
                    name: "Singleton",
                    summary: "Test",
                    category: PatternCategory.Creational,
                    difficulty: DifficultyLevel.Beginner,
                    modernRelevance: "Test")
            };

            var json = JsonSerializer.Serialize(testPatterns);
            await File.WriteAllTextAsync(_testFilePath, json);

            var repository = new JsonPatternRepository(_testFilePath);

            // Act
            var result = await repository.GetPatternByIdAsync("singleton");

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be("singleton");
            result.Name.Should().Be("Singleton");
        }

        [Fact]
        public async Task GetPatternByIdAsync_ShouldReturnNull_WhenIdDoesNotExist()
        {
            // Arrange
            var testPatterns = new List<DesignPattern>
            {
                new DesignPattern(
                    id: "singleton",
                    name: "Singleton",
                    summary: "Test",
                    category: PatternCategory.Creational,
                    difficulty: DifficultyLevel.Beginner,
                    modernRelevance: "Test")
            };

            var json = JsonSerializer.Serialize(testPatterns);
            await File.WriteAllTextAsync(_testFilePath, json);

            var repository = new JsonPatternRepository(_testFilePath);

            // Act
            var result = await repository.GetPatternByIdAsync("nonexistent");

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetAllPatternsAsync_ShouldCacheResults()
        {
            // Arrange
            var testPatterns = new List<DesignPattern>
            {
                new DesignPattern(
                    id: "singleton",
                    name: "Singleton",
                    summary: "Test",
                    category: PatternCategory.Creational,
                    difficulty: DifficultyLevel.Beginner,
                    modernRelevance: "Test")
            };

            var json = JsonSerializer.Serialize(testPatterns);
            await File.WriteAllTextAsync(_testFilePath, json);

            var repository = new JsonPatternRepository(_testFilePath);

            // Act
            var result1 = await repository.GetAllPatternsAsync();
            
            // Delete the file to verify caching
            File.Delete(_testFilePath);
            
            var result2 = await repository.GetAllPatternsAsync();

            // Assert
            result1.Should().HaveCount(1);
            result2.Should().HaveCount(1);
            result2.Should().BeEquivalentTo(result1);
        }

        [Fact]
        public async Task GetAllPatternsAsync_ShouldDeserializeComplexPatterns()
        {
            // Arrange
            var testPattern = new DesignPattern(
                id: "singleton",
                name: "Singleton",
                summary: "Test",
                category: PatternCategory.Creational,
                difficulty: DifficultyLevel.Beginner,
                modernRelevance: "Test",
                sections: new List<Section>
                {
                    new Section(
                        title: "Introduction",
                        content: "Test content",
                        codeSample: new CodeSample(
                            language: "csharp",
                            code: "var x = 10;",
                            description: "Test"))
                },
                tags: new List<string> { "design", "pattern" });

            var json = JsonSerializer.Serialize(new List<DesignPattern> { testPattern });
            await File.WriteAllTextAsync(_testFilePath, json);

            var repository = new JsonPatternRepository(_testFilePath);

            // Act
            var result = await repository.GetAllPatternsAsync();

            // Assert
            var pattern = result.First();
            pattern.Sections.Should().HaveCount(1);
            pattern.Sections[0].CodeSample.Should().NotBeNull();
            pattern.Tags.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetAllPatternsAsync_ShouldThrowJsonException_WhenJsonIsInvalid()
        {
            // Arrange
            await File.WriteAllTextAsync(_testFilePath, "{ invalid json }");
            var repository = new JsonPatternRepository(_testFilePath);

            // Act & Assert
            var act = async () => await repository.GetAllPatternsAsync();
            await act.Should().ThrowAsync<JsonException>();
        }

        [Fact]
        public async Task GetPatternByIdAsync_ShouldBeCaseInsensitive()
        {
            // Arrange
            var testPatterns = new List<DesignPattern>
            {
                new DesignPattern(
                    id: "singleton",
                    name: "Singleton",
                    summary: "Test",
                    category: PatternCategory.Creational,
                    difficulty: DifficultyLevel.Beginner,
                    modernRelevance: "Test")
            };

            var json = JsonSerializer.Serialize(testPatterns);
            await File.WriteAllTextAsync(_testFilePath, json);

            var repository = new JsonPatternRepository(_testFilePath);

            // Act
            var result = await repository.GetPatternByIdAsync("SINGLETON");

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be("singleton");
        }

        [Fact]
        public async Task GetAllPatternsAsync_ShouldHandleEmptyJsonArray()
        {
            // Arrange
            await File.WriteAllTextAsync(_testFilePath, "[]");
            var repository = new JsonPatternRepository(_testFilePath);

            // Act
            var result = await repository.GetAllPatternsAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetPatternByIdAsync_ShouldReturnFirstMatch_WhenMultipleExist()
        {
            // Arrange
            var testPatterns = new List<DesignPattern>
            {
                new DesignPattern(
                    id: "test",
                    name: "First",
                    summary: "Test",
                    category: PatternCategory.Creational,
                    difficulty: DifficultyLevel.Beginner,
                    modernRelevance: "Test"),
                new DesignPattern(
                    id: "test",
                    name: "Second",
                    summary: "Test",
                    category: PatternCategory.Creational,
                    difficulty: DifficultyLevel.Beginner,
                    modernRelevance: "Test")
            };

            var json = JsonSerializer.Serialize(testPatterns);
            await File.WriteAllTextAsync(_testFilePath, json);

            var repository = new JsonPatternRepository(_testFilePath);

            // Act
            var result = await repository.GetPatternByIdAsync("test");

            // Assert
            result.Should().NotBeNull();
            result!.Name.Should().Be("First");
        }

        [Fact]
        public async Task GetAllPatternsAsync_ShouldHandlePatternWithNullableFields()
        {
            // Arrange
            var testPattern = new DesignPattern(
                id: "test",
                name: "Test",
                summary: "Test",
                category: PatternCategory.Creational,
                difficulty: DifficultyLevel.Beginner,
                modernRelevance: "Test",
                iconPath: null);

            var json = JsonSerializer.Serialize(new List<DesignPattern> { testPattern });
            await File.WriteAllTextAsync(_testFilePath, json);

            var repository = new JsonPatternRepository(_testFilePath);

            // Act
            var result = await repository.GetAllPatternsAsync();

            // Assert
            var pattern = result.First();
            pattern.IconPath.Should().BeNull();
        }

        [Fact]
        public async Task GetAllPatternsAsync_ShouldLoadRealPatternsJson()
        {
            // Arrange: default path resolution + None-Update-transferred real data.
            var repository = new JsonPatternRepository();

            // Act
            var result = await repository.GetAllPatternsAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
            result.Select(p => p.Id).Should().Contain("strategy");
            result.Should().OnlyContain(p => !string.IsNullOrWhiteSpace(p.Id));
        }

        [Fact]
        public async Task GetAllPatternsAsync_ShouldThrowJsonException_WhenJsonIsEmptyString()
        {
            await File.WriteAllTextAsync(_testFilePath, "");
            var repository = new JsonPatternRepository(_testFilePath);

            var act = async () => await repository.GetAllPatternsAsync();
            await act.Should().ThrowAsync<JsonException>();
        }

        [Fact]
        public async Task GetAllPatternsAsync_ShouldThrowJsonException_WhenJsonIsWhitespace()
        {
            await File.WriteAllTextAsync(_testFilePath, "   ");
            var repository = new JsonPatternRepository(_testFilePath);

            var act = async () => await repository.GetAllPatternsAsync();
            await act.Should().ThrowAsync<JsonException>();
        }

        [Fact]
        public async Task GetAllPatternsAsync_ShouldReturnEmptyList_WhenJsonIsNullLiteral()
        {
            await File.WriteAllTextAsync(_testFilePath, "null");
            var repository = new JsonPatternRepository(_testFilePath);

            var result = await repository.GetAllPatternsAsync();
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllPatternsAsync_ShouldThrowJsonException_WhenJsonIsEmptyObject()
        {
            await File.WriteAllTextAsync(_testFilePath, "{}");
            var repository = new JsonPatternRepository(_testFilePath);

            var act = async () => await repository.GetAllPatternsAsync();
            await act.Should().ThrowAsync<JsonException>();
        }

        [Fact]
        public async Task GetAllPatternsAsync_ShouldHandleConcurrentCalls()
        {
            var testPattern = new DesignPattern(
                id: "singleton",
                name: "Singleton",
                summary: "Test",
                category: PatternCategory.Creational,
                difficulty: DifficultyLevel.Beginner,
                modernRelevance: "Test");
            var json = JsonSerializer.Serialize(new List<DesignPattern> { testPattern });
            await File.WriteAllTextAsync(_testFilePath, json);
            var repository = new JsonPatternRepository(_testFilePath);

            var tasks = Enumerable.Range(0, 8).Select(_ => repository.GetAllPatternsAsync());
            var results = await Task.WhenAll(tasks);

            results.Should().HaveCount(8);
            foreach (var r in results)
            {
                r.Should().HaveCount(1);
                r.First().Id.Should().Be("singleton");
            }
        }

        [Fact]
        public async Task GetPatternByIdAsync_ShouldBeCaseInsensitive_WithUpperCase()
        {
            var testPatterns = new List<DesignPattern>
            {
                new DesignPattern(
                    id: "singleton",
                    name: "Singleton",
                    summary: "Test",
                    category: PatternCategory.Creational,
                    difficulty: DifficultyLevel.Beginner,
                    modernRelevance: "Test")
            };
            var json = JsonSerializer.Serialize(testPatterns);
            await File.WriteAllTextAsync(_testFilePath, json);
            var repository = new JsonPatternRepository(_testFilePath);

            var result = await repository.GetPatternByIdAsync("SINGLETON");

            result.Should().NotBeNull();
            result!.Id.Should().Be("singleton");
        }
    }
}
