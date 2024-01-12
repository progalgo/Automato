using System.IO;
using System.Reflection;

using Automato.Contracts.Services;
using Automato.Core.Contracts.Services;
using Automato.Core.Services;
using Automato.Models;
using Automato.Services;
using Automato.ViewModels;

using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Prism.Regions;

using Unity;

namespace Automato.Tests.MSTest;

[TestClass]
public class PagesTests
{
    private readonly IUnityContainer _container;

    public PagesTests()
    {
        _container = new UnityContainer();
        _container.RegisterType<IRegionManager, RegionManager>();

        // Core Services
        _container.RegisterType<IFileService, FileService>();

        // App Services
        _container.RegisterType<IThemeSelectorService, ThemeSelectorService>();
        _container.RegisterType<ISystemService, SystemService>();
        _container.RegisterType<IPersistAndRestoreService, PersistAndRestoreService>();
        _container.RegisterType<IApplicationInfoService, ApplicationInfoService>();

        // Configuration
        var appLocation = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location);
        var configuration = new ConfigurationBuilder()
            .SetBasePath(appLocation)
            .AddJsonFile("appsettings.json")
            .Build();
        var appConfig = configuration
            .GetSection(nameof(AppConfig))
            .Get<AppConfig>();

        // Register configurations to IoC
        _container.RegisterInstance(configuration);
        _container.RegisterInstance(appConfig);
    }

    // TODO: Add tests for functionality you add to MainViewModel.
    [TestMethod]
    public void TestMainViewModelCreation()
    {
        var vm = _container.Resolve<MainViewModel>();
        Assert.IsNotNull(vm);
    }

    // TODO: Add tests for functionality you add to SettingsViewModel.
    [TestMethod]
    public void TestSettingsViewModelCreation()
    {
        var vm = _container.Resolve<SettingsViewModel>();
        Assert.IsNotNull(vm);
    }
}
