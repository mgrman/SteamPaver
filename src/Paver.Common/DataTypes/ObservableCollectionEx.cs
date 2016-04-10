using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paver.Common
{
    public class ObservableCollectionEx<T> : ObservableCollection<T>
    {
        public void AddRange(IEnumerable<T> items)
        {
            var itemsList = items.ToList();

            try
            {
                _suspendCollectionChanged++;
                foreach (var item in itemsList)
                {
                    Add(item);
                }
            }
            finally
            {
                _suspendCollectionChanged--;
            }

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, itemsList));
        }

        public void ReplaceAndResetCollection(IEnumerable<T> items)
        {
            try
            {
                _suspendCollectionChanged++;
                Clear();
                AddRange(items);

            }
            finally
            {
                _suspendCollectionChanged--;
            }

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void DiffCollection(IEnumerable<T> items)
        {
            
            foreach (var item in this.Except(items).ToArray())
            {
                this.Remove(item);
            }

            foreach (var item in items.Except(this).ToArray())
            {
                this.Add(item);
            }
        }


        private int _suspendCollectionChanged = 0;

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (_suspendCollectionChanged > 0)
                return;

            base.OnCollectionChanged(e);
        }


    }
}
