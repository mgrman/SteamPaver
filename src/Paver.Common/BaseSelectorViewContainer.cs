using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Paver.Common
{
    public abstract class BaseSelectorViewContainer<TView>
        where TView:Control
    {
        public string Header { get; }
        public TView Content { get; }

        public BaseSelectorViewContainer(string header, TView content)
        {
            Header = header;
            Content = content;
        }
    }
}
