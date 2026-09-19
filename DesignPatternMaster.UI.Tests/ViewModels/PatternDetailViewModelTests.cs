using DesignPatternMaster.Core.Entities;
using DesignPatternMaster.Core.Enums;
using DesignPatternMaster.UI.ViewModels;
using DesignPatternMaster.UseCases.Queries;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace DesignPatternMaster.UI.Tests.ViewModels
{
    public class PatternDetailViewModelTests
    {
        [Fact]
        public async Task LoadPatternAsync_ShouldSetSelectedPattern()
        {
            var expected = new DesignPattern(
                id: "singleton",
                name: "Singleton",
                summary: "Test",
                category: PatternCategory.Creational,
                difficulty: DifficultyLevel.Beginner,
                modernRelevance: "Test");
            var mockQuery = new Mock<IGetPatternDetailQuery>();
            mockQuery.Setup(q => q.ExecuteAsync("singleton", It.IsAny<CancellationToken>())).ReturnsAsync(expected);
            var vm = new PatternDetailViewModel(mockQuery.Object, Mock.Of<ILogger<PatternDetailViewModel>>());

            await vm.LoadPatternAsync("singleton");

            vm.SelectedPattern.Should().NotBeNull();
            vm.SelectedPattern!.Id.Should().Be("singleton");
            vm.SelectedPattern.Should().Be(expected);
            mockQuery.Verify(q => q.ExecuteAsync("singleton", It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
