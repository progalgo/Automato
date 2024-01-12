using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;

using Automato.Constants;
using Automato.Contracts.Services;
using Automato.Core.Contracts.Services;
using Automato.Core.Services;
using Automato.Models;
using Automato.Services;
using Automato.ViewModels;
using Automato.Views;

using Microsoft.Extensions.Configuration;

using Prism.Ioc;
using Prism.Mvvm;
using Prism.Unity;

namespace Automato;

// For more information about application lifecycle events see https://docs.microsoft.com/dotnet/framework/wpf/app-development/application-management-overview
// For docs about using Prism in WPF see https://prismlibrary.com/docs/wpf/introduction.html

// WPF UI elements use language en-US by default.
// If you need to support other cultures make sure you add converters and review dates and numbers in your UI to ensure everything adapts correctly.
// Tracking issue for improving this is https://github.com/dotnet/wpf/issues/1946
public partial class App : PrismApplication
{
    private string[] _startUpArgs;

    public App()
    {
    }

    protected override Window CreateShell()
        => Container.Resolve<ShellWindow>();

    protected override async void OnInitialized()
    {
        var persistAndRestoreService = Container.Resolve<IPersistAndRestoreService>();
        persistAndRestoreService.RestoreData();

        var themeSelectorService = Container.Resolve<IThemeSelectorService>();
        themeSelectorService.InitializeTheme();

        base.OnInitialized();
        await Task.CompletedTask;
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        _startUpArgs = e.Args;
        base.OnStartup(e);
    }

    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
        // Core Services
        containerRegistry.Register<IFileService, FileService>();

        // App Services
        containerRegistry.Register<IApplicationInfoService, ApplicationInfoService>();
        containerRegistry.Register<ISystemService, SystemService>();
        containerRegistry.Register<IPersistAndRestoreService, PersistAndRestoreService>();
        containerRegistry.Register<IThemeSelectorService, ThemeSelectorService>();
        containerRegistry.RegisterSingleton<IRightPaneService, RightPaneService>();

        // Views
        containerRegistry.RegisterForNavigation<SettingsPage, SettingsViewModel>(PageKeys.Settings);
        containerRegistry.RegisterForNavigation<MainPage, MainViewModel>(PageKeys.Main);
        containerRegistry.RegisterForNavigation<ShellWindow, ShellViewModel>();

        // Configuration
        var configuration = BuildConfiguration();
        var appConfig = configuration
            .GetSection(nameof(AppConfig))
            .Get<AppConfig>();

        // Register configurations to IoC
        containerRegistry.RegisterInstance<IConfiguration>(configuration);
        containerRegistry.RegisterInstance<AppConfig>(appConfig);
    }

    private IConfiguration BuildConfiguration()
    {
        var appLocation = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        return new ConfigurationBuilder()
            .SetBasePath(appLocation)
            .AddJsonFile("appsettings.json")
            .AddCommandLine(_startUpArgs)
            .Build();
    }

    private void OnExit(object sender, ExitEventArgs e)
    {
        var persistAndRestoreService = Container.Resolve<IPersistAndRestoreService>();
        persistAndRestoreService.PersistData();
    }

    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        // TODO: Please log and handle the exception as appropriate to your scenario
        // For more info see https://docs.microsoft.com/dotnet/api/system.windows.application.dispatcherunhandledexception?view=netcore-3.0
    }
}
