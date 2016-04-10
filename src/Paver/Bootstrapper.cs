using Microsoft.Practices.Unity;
using Prism.Unity;
using Paver.Views;
using System.Windows;
using Prism.Modularity;
using System;
using Prism.Logging;

namespace Paver
{
    class Bootstrapper : UnityBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void InitializeShell()
        {
            Application.Current.MainWindow.Show();
        }

        protected override void ConfigureModuleCatalog()
        {
            var catalog = ModuleCatalog as ModuleCatalog;

            catalog.AddModule(typeof(Paver.TileCreator.TileCreatorModule));

            catalog.AddModule(typeof(Paver.Steam.SteamModule));
            
        }
    }
}
