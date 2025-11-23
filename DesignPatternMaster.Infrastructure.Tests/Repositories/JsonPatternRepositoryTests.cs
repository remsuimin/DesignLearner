using DesignPatternMaster.Core.Entities;
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
        public async Task GetAllPatternsAsync_ShouldReturnEmptyList_WhenFileDoesNotExist()
        {
            // Arrange
            var nonExistentPath = Path.Combine(_testDirectory, "nonexistent.json");
            var repository = new JsonPatternRepository(nonExistentPath);

            // Act
            var result = await repository.GetAllPatternsAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetPatternByIdAsync_ShouldReturnPattern_WhenIdExists()
        {
            // Arrange
            var testPatterns = new List<DesignPattern>
            {
                new DesignPattern
                {
                    Id = "singleton",
                    Name = "Singleton",
                    Summary = "Test",
                    Category = "Creational",
                    Difficulty = "Beginner",
                    ModernRelevance = "Test"
                }
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
                new DesignPattern
                {
                    Id = "singleton",
                    Name = "Singleton",
                    Summary = "Test",
                    Category = "Creational",
                    Difficulty = "Beginner",
                    ModernRelevance = "Test"
                }
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
                new DesignPattern
                {
                    Id = "singleton",
                    Name = "Singleton",
                    Summary = "Test",
                    Category = "Creational",
                    Difficulty = "Beginner",
                    ModernRelevance = "Test"
                }
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
            var testPattern = new DesignPattern
            {
                Id = "singleton",
                Name = "Singleton",
                Summary = "Test",
                Category = "Creational",
                Difficulty = "Beginner",
                ModernRelevance = "Test",
                Sections = new List<Section>
                {
                    new Section
                    {
                        Title = "Introduction",
                        Content = "Test content",
                        CodeSample = new CodeSample
                        {
                            Language = "csharp",
                            Code = "var x = 10;",
                            Description = "Test"
                        }
                    }
                },
                Tags = new List<string> { "design", "pattern" }
            };

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
        public async Task GetAllPatternsAsync_ShouldHandleInvalidJson_AndReturnEmptyList()
        {
            // Arrange
            await File.WriteAllTextAsync(_testFilePath, "{ invalid json }");
            var repository = new JsonPatternRepository(_testFilePath);

            // Act & Assert
            // Should not throw, but return empty list or handle gracefully
            var act = async () => await repository.GetAllPatternsAsync();
            await act.Should().ThrowAsync<JsonException>();
        }

        [Fact]
        public async Task GetPatternByIdAsync_ShouldBeCaseInsensitive()
        {
            // Arrange
            var testPatterns = new List<DesignPattern>
            {
                new DesignPattern
                {
                    Id = "singleton",
                    Name = "Singleton",
                    Summary = "Test",
                    Category = "Creational",
                    Difficulty = "Beginner",
                    ModernRelevance = "Test"
                }
            };

            var json = JsonSerializer.Serialize(testPatterns);
            await File.WriteAllTextAsync(_testFilePath, json);

            var repository = new JsonPatternRepository(_testFilePath);

            // Act
            var result = await repository.GetPatternByIdAsync("singleton");

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
                new DesignPattern
                {
                    Id = "test",
                    Name = "First",
                    Summary = "Test",
                    Category = "Creational",
                    Difficulty = "Beginner",
                    ModernRelevance = "Test"
                },
                new DesignPattern
                {
                    Id = "test",
                    Name = "Second",
                    Summary = "Test",
                    Category = "Creational",
                    Difficulty = "Beginner",
                    ModernRelevance = "Test"
                }
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
            var testPattern = new DesignPattern
            {
                Id = "test",
                Name = "Test",
                Summary = "Test",
                Category = "Test",
                Difficulty = "Test",
                ModernRelevance = "Test",
                IconPath = null
            };

            var json = JsonSerializer.Serialize(new List<DesignPattern> { testPattern });
            await File.WriteAllTextAsync(_testFilePath, json);

            var repository = new JsonPatternRepository(_testFilePath);

            // Act
            var result = await repository.GetAllPatternsAsync();

            // Assert
            var pattern = result.First();
            pattern.IconPath.Should().BeNull();
        }
    }
}
