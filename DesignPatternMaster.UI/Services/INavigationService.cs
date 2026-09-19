using System.Threading.Tasks;

namespace DesignPatternMaster.UI.Services;

public interface INavigationService
{
    void Initialize(System.Windows.Controls.Frame frame);

    Task NavigateToDashboardAsync(CancellationToken cancellationToken = default);

    Task NavigateToSettingsAsync(CancellationToken cancellationToken = default);

    Task NavigateToPatternDetailAsync(string patternId, CancellationToken cancellationToken = default);
}
