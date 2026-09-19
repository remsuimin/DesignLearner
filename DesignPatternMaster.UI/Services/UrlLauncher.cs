using System.Diagnostics;

namespace DesignPatternMaster.UI.Services;

public sealed class UrlLauncher : IUrlLauncher
{
    public const string DefaultRepositoryUrl = "https://github.com/remsuimin/DesignLearner";

    public string RepositoryUrl => DefaultRepositoryUrl;

    public bool TryOpenUrl(string url)
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
            return true;
        }
        catch
        {
            return false;
        }
    }
}
