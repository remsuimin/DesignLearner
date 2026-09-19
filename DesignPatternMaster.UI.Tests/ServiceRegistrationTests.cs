using DesignPatternMaster.Core.Interfaces;
using DesignPatternMaster.UI.Services;
using DesignPatternMaster.UI.ViewModels;
using DesignPatternMaster.UseCases.Queries;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DesignPatternMaster.UI.Tests;

public class ServiceRegistrationTests
{
    [Fact]
    public void BuildServices_ShouldResolveNonVisualServices()
    {
        using var provider = App.BuildServices().BuildServiceProvider();

        provider.GetRequiredService<IPatternRepository>().Should().NotBeNull();
        provider.GetRequiredService<IGetPatternListQuery>().Should().NotBeNull();
        provider.GetRequiredService<IGetPatternDetailQuery>().Should().NotBeNull();
        provider.GetRequiredService<INavigationService>().Should().NotBeNull();
        provider.GetRequiredService<IDialogService>().Should().NotBeNull();
        provider.GetRequiredService<IUrlLauncher>().Should().NotBeNull();
        provider.GetRequiredService<MainWindowViewModel>().Should().NotBeNull();
        provider.GetRequiredService<SettingsViewModel>().Should().NotBeNull();

        provider.GetRequiredService<DashboardViewModel>().Should().NotBeNull();
        provider.GetRequiredService<PatternDetailViewModel>().Should().NotBeNull();
    }

    [Fact]
    public void BuildServices_ShouldRespectLifetimes()
    {
        using var provider = App.BuildServices().BuildServiceProvider();

        provider.GetRequiredService<DashboardViewModel>()
            .Should().NotBeSameAs(provider.GetRequiredService<DashboardViewModel>());
        provider.GetRequiredService<PatternDetailViewModel>()
            .Should().NotBeSameAs(provider.GetRequiredService<PatternDetailViewModel>());
        provider.GetRequiredService<MainWindowViewModel>()
            .Should().BeSameAs(provider.GetRequiredService<MainWindowViewModel>());
        provider.GetRequiredService<SettingsViewModel>()
            .Should().BeSameAs(provider.GetRequiredService<SettingsViewModel>());
    }
}
