namespace DesignPatternMaster.UI.Services;

public interface IDialogService
{
    void ShowInformation(string message, string title);

    void ShowError(string message, string title);
}
