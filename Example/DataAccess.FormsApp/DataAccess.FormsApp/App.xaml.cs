namespace DataAccess.FormsApp
{
    using System;
    using System.IO;
    using System.Reflection;

    using DataAccess.FormsApp.Components;
    using DataAccess.FormsApp.Handlers;
    using DataAccess.FormsApp.Modules;

    using Microsoft.Data.Sqlite;

    using Smart.Data;
    using Smart.Data.Mapper;
    using Smart.Forms.Resolver;
    using Smart.Navigation;
    using Smart.Resolver;

    using Xamarin.Essentials;

    public partial class App
    {
        private readonly Navigator navigator;

        public App(IComponentProvider provider)
        {
            InitializeComponent();

            // Config Resolver
            var resolver = CreateResolver(provider);
            ResolveProvider.Default.UseSmartResolver(resolver);

            // Config Navigator
            navigator = new NavigatorConfig()
                .UseFormsNavigationProvider()
                .UseResolver(resolver)
                .UseIdViewMapper(m => m.AutoRegister(Assembly.GetExecutingAssembly().ExportedTypes))
                .ToNavigator();
            navigator.Navigated += (sender, args) =>
            {
                // for debug
                System.Diagnostics.Debug.WriteLine(
                    $"Navigated: [{args.Context.FromId}]->[{args.Context.ToId}] : stacked=[{navigator.StackedCount}]");
            };

            // Config DataMapper
            SqlMapperConfig.Default.ConfigureTypeHandlers(config =>
            {
                config[typeof(DateTimeOffset)] = new DateTimeOffsetTypeHandler();
            });

            // Show MainWindow
            MainPage = resolver.Get<MainPage>();
        }

        private SmartResolver CreateResolver(IComponentProvider provider)
        {
            var config = new ResolverConfig()
                .UseAutoBinding()
                .UseArrayBinding()
                .UseAssignableBinding()
                .UsePropertyInjector();

            var databasePath = Path.Combine(FileSystem.AppDataDirectory, "Test.db");

            config.Bind<INavigator>().ToMethod(kernel => navigator).InSingletonScope();

            provider.RegisterComponents(config);
            config.Bind<Settings>().ToConstant(new Settings
            {
                DatabasePath = databasePath
            }).InSingletonScope();

            var connectionString = $"Data Source={databasePath}";
            config.Bind<IConnectionFactory>()
                .ToConstant(new CallbackConnectionFactory(() => new SqliteConnection(connectionString)));

            config.Bind<ApplicationState>().ToSelf().InSingletonScope();

            config.Bind<IDialogs>().To<Dialogs>().InSingletonScope();

            return config.ToResolver();
        }

        protected override void OnStart()
        {
            navigator.Forward(ViewId.Menu);
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
