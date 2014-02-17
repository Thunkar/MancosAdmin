using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MancosAdmin.Model
{
    public class ObservableSet<T> : ObservableCollection<T>
    {
        protected override void InsertItem(int index, T item)
        {
            if (Contains(item))
            {
                return;
            }
            base.InsertItem(index, item);
        }

    }
}
