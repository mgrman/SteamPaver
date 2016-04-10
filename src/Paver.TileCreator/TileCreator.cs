using Paver.Common;
using Paver.Common.Models;
using Prism.Logging;
using Prism.Modularity;
using Prism.Regions;
using System;
using System.Collections.Generic;

namespace Paver.TileCreator
{
    internal class TileCreator : ITileCreator
    {
        public ITileCreator Creator { get; }


        public TileCreator(ILoggerFacade logger)
        {
            logger.Log($"Windows Version : {Environment.OSVersion.Version}",Category.Info,Priority.High);
            
            if (Environment.OSVersion.Version.Build >= 10586)
                Creator = new Win10TP2TileCreator(logger);
            else 
                Creator = null;
            
            if (Creator == null) 
                throw new InvalidOperationException("This version of Windows does not support creating Tiles!");
        }

        public void CreateTile(TileSettings tileSettings) => Creator.CreateTile(tileSettings);
        
        public IEnumerable<LinkTypes> GetSupportedLinkTypes(TileSettings tileSettings) => Creator.GetSupportedLinkTypes(tileSettings) ;
     
    }
}