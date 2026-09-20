using System.Windows.Controls;
using DesignPatternMaster.UI.Views.Pages;
using DesignPatternMaster.UseCases.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DesignPatternMaster.UI.Services;

public sealed partial class NavigationService : INavigationService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<NavigationService> _logger;
    private readonly IDialogService _dialog;
    private Frame? _frame;

    public NavigationService(IServiceProvider services, ILogger<NavigationService> logger, IDialogService dialog)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(dialog);
        _services = services;
        _logger = logger;
        _dialog = dialog;
    }

    public void Initialize(Frame frame)
    {
        ArgumentNullException.ThrowIfNull(frame);
        _frame = frame;
    }

    private Frame Frame => _frame ?? throw new InvalidOperationException("Navigation frame is not initialized.");

    public async Task NavigateToDashboardAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var page = _services.GetRequiredService<DashboardPage>();
            Frame.Navigate(page);
            TrimJournal();
            await page.ViewModel.LoadDataAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            LogFailedToNavigateToDashboard(_logger, ex);
            _dialog.ShowError($"ダッシュボードの表示に失敗しました: {ex.Message}", "エラー");
        }
    }

    public async Task NavigateToSettingsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var page = _services.GetRequiredService<SettingsPage>();
            Frame.Navigate(page);
            TrimJournal();
        }
        catch (Exception ex)
        {
            LogFailedToNavigateToSettings(_logger, ex);
            _dialog.ShowError($"設定画面の表示に失敗しました: {ex.Message}", "エラー");
        }
    }

    public async Task NavigateToPatternDetailAsync(string patternId, CancellationToken cancellationToken = default)
    {
        try
        {
            var page = _services.GetRequiredService<PatternDetailPage>();
            Frame.Navigate(page);
            TrimJournal();
            await page.ViewModel.LoadPatternAsync(patternId, cancellationToken);
        }
        catch (PatternNotFoundException ex)
        {
            LogPatternNotFound(_logger, ex, patternId);
            _dialog.ShowError($"パターンが見つかりません: {patternId}", "エラー");
            await NavigateToDashboardAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            LogFailedToNavigateToPatternDetail(_logger, ex, patternId);
            _dialog.ShowError($"詳細画面の表示に失敗しました: {ex.Message}", "エラー");
        }
    }

    private void TrimJournal()
    {
        const int MaxJournalEntries = 10;
        var frame = Frame;
        while (frame.CanGoBack && frame.BackStack.Cast<object>().Count() > MaxJournalEntries)
        {
            if (frame.RemoveBackEntry() is null)
                break;
        }
    }

    [LoggerMessage(EventId = 1, Level = LogLevel.Error, Message = "Failed to navigate to dashboard.")]
    private static partial void LogFailedToNavigateToDashboard(ILogger logger, Exception ex);

    [LoggerMessage(EventId = 2, Level = LogLevel.Error, Message = "Failed to navigate to settings.")]
    private static partial void LogFailedToNavigateToSettings(ILogger logger, Exception ex);

    [LoggerMessage(EventId = 3, Level = LogLevel.Warning, Message = "Pattern not found: {PatternId}")]
    private static partial void LogPatternNotFound(ILogger logger, Exception ex, string patternId);

    [LoggerMessage(EventId = 4, Level = LogLevel.Error, Message = "Failed to navigate to pattern detail: {PatternId}")]
    private static partial void LogFailedToNavigateToPatternDetail(ILogger logger, Exception ex, string patternId);
}
