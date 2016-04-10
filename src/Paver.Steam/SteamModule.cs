using Paver.Common;
using Prism.Events;
using Prism.Modularity;
using Prism.Regions;
using System;

namespace Paver.Steam
{
    public class SteamModule : IModule
    {
        private readonly IRegionManager _regionManager;

        public SteamModule(RegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public void Initialize()
        {
            _regionManager.AddToRegion(AvailableRegions.SelectorsRegion, new SteamSelectorViewContainer());
        }
    }
}