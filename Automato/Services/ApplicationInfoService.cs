using System.Diagnostics;
using System.Reflection;

using Automato.Contracts.Services;

using OSVersionHelper;

using Windows.ApplicationModel;

namespace Automato.Services;

public class ApplicationInfoService : IApplicationInfoService
{
    public ApplicationInfoService()
    {
    }

    public Version GetVersion()
    {
        if (WindowsVersionHelper.HasPackageIdentity)
        {
            // Packaged application
            // Set the app version in Automato.Packaging > Package.appxmanifest > Packaging > PackageVersion
            var packageVersion = Package.Current.Id.Version;
            return new Version(packageVersion.Major, packageVersion.Minor, packageVersion.Build, packageVersion.Revision);
        }

        // Set the app version in Automato > Properties > Package > PackageVersion
        string assemblyLocation = Assembly.GetExecutingAssembly().Location;
        var version = FileVersionInfo.GetVersionInfo(assemblyLocation).FileVersion;
        return new Version(version);
    }
}
