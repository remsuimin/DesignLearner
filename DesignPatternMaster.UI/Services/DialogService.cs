using System.Windows;

namespace DesignPatternMaster.UI.Services;

public sealed class DialogService : IDialogService
{
    public void ShowInformation(string message, string title)
    {
        MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
    }

    public void ShowError(string message, string title)
    {
        MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
    }
}
