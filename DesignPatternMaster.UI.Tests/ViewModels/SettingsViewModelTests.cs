using DesignPatternMaster.UI.Services;
using DesignPatternMaster.UI.ViewModels;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace DesignPatternMaster.UI.Tests.ViewModels
{
    public class SettingsViewModelTests
    {
        [Fact]
        public void Constructor_ShouldSetVersionNotEmpty()
        {
            var vm = new SettingsViewModel(Mock.Of<IDialogService>(), Mock.Of<IUrlLauncher>(), Mock.Of<ILogger<SettingsViewModel>>());
            vm.Version.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void ResetDataCommand_ShouldShowInformation()
        {
            var mockDialog = new Mock<IDialogService>();
            var vm = new SettingsViewModel(mockDialog.Object, Mock.Of<IUrlLauncher>(), Mock.Of<ILogger<SettingsViewModel>>());

            vm.ResetDataCommand.Execute(null);

            mockDialog.Verify(d => d.ShowInformation(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            mockDialog.Verify(d => d.ShowError(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void OpenGitHubCommand_WhenSuccess_ShouldNotShowError()
        {
            var mockDialog = new Mock<IDialogService>();
            var mockLauncher = new Mock<IUrlLauncher>();
            mockLauncher.SetupGet(l => l.RepositoryUrl).Returns("https://example.com");
            mockLauncher.Setup(l => l.TryOpenUrl(It.IsAny<string>())).Returns(true);
            var vm = new SettingsViewModel(mockDialog.Object, mockLauncher.Object, Mock.Of<ILogger<SettingsViewModel>>());

            vm.OpenGitHubCommand.Execute(null);

            mockLauncher.Verify(l => l.TryOpenUrl("https://example.com"), Times.Once);
            mockDialog.Verify(d => d.ShowError(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void OpenGitHubCommand_WhenFailed_ShouldShowError()
        {
            var mockDialog = new Mock<IDialogService>();
            var mockLauncher = new Mock<IUrlLauncher>();
            mockLauncher.SetupGet(l => l.RepositoryUrl).Returns("https://example.com");
            mockLauncher.Setup(l => l.TryOpenUrl(It.IsAny<string>())).Returns(false);
            var vm = new SettingsViewModel(mockDialog.Object, mockLauncher.Object, Mock.Of<ILogger<SettingsViewModel>>());

            vm.OpenGitHubCommand.Execute(null);

            mockLauncher.Verify(l => l.TryOpenUrl("https://example.com"), Times.Once);
            mockDialog.Verify(d => d.ShowError(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }
    }
}
