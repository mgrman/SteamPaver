using Paver.Common;
using Prism.Modularity;
using Prism.Logging;
using Prism.Regions;
using System;
using Microsoft.Practices.Unity;

namespace Paver.TileCreator
{
    public class TileCreatorModule : IModule
    {
        private readonly ILoggerFacade _logger;
        private readonly IUnityContainer _container;

        public TileCreatorModule(ILoggerFacade logger,IUnityContainer container)
        {
            _logger = logger;
            _container = container;
        }

        public void Initialize()
        {
            _container.RegisterInstance<ITileCreator>(new TileCreator(_logger));
        }
    }
}