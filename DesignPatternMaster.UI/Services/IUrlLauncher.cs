namespace DesignPatternMaster.UI.Services;

public interface IUrlLauncher
{
    string RepositoryUrl { get; }

    bool TryOpenUrl(string url);
}
