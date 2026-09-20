using System.Reflection;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DesignPatternMaster.UI.Services;
using Microsoft.Extensions.Logging;

namespace DesignPatternMaster.UI.ViewModels;

public sealed partial class SettingsViewModel : ObservableObject
{
    private readonly IDialogService _dialog;
    private readonly IUrlLauncher _launcher;
    private readonly ILogger<SettingsViewModel> _logger;

    [ObservableProperty]
    private string _selectedTheme = "Dark";

    [ObservableProperty]
    private double _codeFontSize = 14;

    [ObservableProperty]
    private string _version = "1.0.0";

    public SettingsViewModel(IDialogService dialog, IUrlLauncher launcher, ILogger<SettingsViewModel> logger)
    {
        ArgumentNullException.ThrowIfNull(dialog);
        ArgumentNullException.ThrowIfNull(launcher);
        ArgumentNullException.ThrowIfNull(logger);
        _dialog = dialog;
        _launcher = launcher;
        _logger = logger;

        // Get version from assembly
        var assembly = Assembly.GetExecutingAssembly();
        var version = assembly.GetName().Version;
        Version = version != null ? $"{version.Major}.{version.Minor}.{version.Build}" : "1.0.0";
    }

    [RelayCommand]
    private void ResetData()
    {
        LogResetDataRequested(_logger);
        _dialog.ShowInformation("データのリセット機能は今後実装予定です。", "情報");
    }

    [RelayCommand]
    private void OpenGitHub()
    {
        if (_launcher.TryOpenUrl(_launcher.RepositoryUrl))
            return;

        LogFailedToOpenUrl(_logger, _launcher.RepositoryUrl);
        _dialog.ShowError("ブラウザを開けませんでした。", "エラー");
    }

    [LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = "Reset data requested.")]
    private static partial void LogResetDataRequested(ILogger logger);

    [LoggerMessage(EventId = 2, Level = LogLevel.Warning, Message = "Failed to open URL: {Url}")]
    private static partial void LogFailedToOpenUrl(ILogger logger, string url);
}
