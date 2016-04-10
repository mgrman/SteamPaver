using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Paver.Common.Utils
{
    public static class EventUtils
    {
        
        public static void Fire(this object sender, EventHandler handler)
        {
            if (handler != null)
            {
                handler(sender, EventArgs.Empty);
            }
        }


        public static void Fire(this object sender, NotifyCollectionChangedEventHandler handler,NotifyCollectionChangedEventArgs args)
        {
            if (handler != null)
            {
                handler(sender,args);
            }
        }
        public static void Fire<Args>(this object sender, EventHandler<Args> handler, Args args)
            where Args : EventArgs
        {
            if (handler != null)
            {
                handler(sender, args);
            }
        }


        public static void Fire(this object sender, PropertyChangedEventHandler handler, [CallerMemberName] string propertyName = "")
        {
            if (handler != null)
            {
                handler(sender, new PropertyChangedEventArgs(propertyName));
            }
        }
        
    }
    
}
