using DesignPatternMaster.UI.Views.Pages;
using DesignPatternMaster.UseCases.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Windows.Controls;

namespace DesignPatternMaster.UI.Services;

public sealed class NavigationService : INavigationService
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
            _logger.LogError(ex, "Failed to navigate to dashboard.");
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
            _logger.LogError(ex, "Failed to navigate to settings.");
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
            _logger.LogWarning(ex, "Pattern not found: {PatternId}", patternId);
            _dialog.ShowError($"パターンが見つかりません: {patternId}", "エラー");
            await NavigateToDashboardAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to navigate to pattern detail: {PatternId}", patternId);
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
}
