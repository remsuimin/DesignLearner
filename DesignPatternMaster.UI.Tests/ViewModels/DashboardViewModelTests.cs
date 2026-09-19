using DesignPatternMaster.Core.Entities;
using DesignPatternMaster.Core.Enums;
using DesignPatternMaster.UI.Services;
using DesignPatternMaster.UI.ViewModels;
using DesignPatternMaster.UseCases.Queries;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace DesignPatternMaster.UI.Tests.ViewModels
{
    public class DashboardViewModelTests
    {
        [Fact]
        public async Task LoadDataAsync_ShouldPopulateEmpty_WhenNoPatterns()
        {
            var mockQuery = new Mock<IGetPatternListQuery>();
            mockQuery.Setup(q => q.ExecuteAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<DesignPattern>());
            var vm = new DashboardViewModel(mockQuery.Object, Mock.Of<INavigationService>(), Mock.Of<ILogger<DashboardViewModel>>());

            await vm.LoadDataAsync();

            vm.Patterns.Should().BeEmpty();
            mockQuery.Verify(q => q.ExecuteAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task LoadDataAsync_ShouldPopulateTwoPatterns()
        {
            var patterns = new List<DesignPattern>
            {
                new DesignPattern(id: "singleton", name: "Singleton", summary: "Test", category: PatternCategory.Creational, difficulty: DifficultyLevel.Beginner, modernRelevance: "Test"),
                new DesignPattern(id: "factory", name: "Factory", summary: "Test", category: PatternCategory.Creational, difficulty: DifficultyLevel.Intermediate, modernRelevance: "Test")
            };
            var mockQuery = new Mock<IGetPatternListQuery>();
            mockQuery.Setup(q => q.ExecuteAsync(It.IsAny<CancellationToken>())).ReturnsAsync(patterns);
            var vm = new DashboardViewModel(mockQuery.Object, Mock.Of<INavigationService>(), Mock.Of<ILogger<DashboardViewModel>>());

            await vm.LoadDataAsync();

            vm.Patterns.Should().HaveCount(2);
            vm.Patterns.Select(p => p.Id).Should().Contain(new[] { "singleton", "factory" });
        }

        [Fact]
        public void NavigateToDetailCommand_WithNull_ShouldNotNavigate()
        {
            var mockNav = new Mock<INavigationService>();
            var vm = new DashboardViewModel(Mock.Of<IGetPatternListQuery>(), mockNav.Object, Mock.Of<ILogger<DashboardViewModel>>());

            vm.NavigateToDetailCommand.Execute(null);

            mockNav.Verify(n => n.NavigateToPatternDetailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public void NavigateToDetailCommand_WithValidPattern_ShouldNavigate()
        {
            var mockNav = new Mock<INavigationService>();
            mockNav.Setup(n => n.NavigateToPatternDetailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            var vm = new DashboardViewModel(Mock.Of<IGetPatternListQuery>(), mockNav.Object, Mock.Of<ILogger<DashboardViewModel>>());
            var pattern = new DesignPattern(id: "singleton", name: "Singleton", summary: "Test", category: PatternCategory.Creational, difficulty: DifficultyLevel.Beginner, modernRelevance: "Test");

            vm.NavigateToDetailCommand.Execute(pattern);

            mockNav.Verify(n => n.NavigateToPatternDetailAsync("singleton", It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
