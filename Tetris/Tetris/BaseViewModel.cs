using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public virtual void Notify(string propertyName)
        {
            PropertyChangedEventHandler Handler = this.PropertyChanged;
            if (Handler != null)
            {
                Handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
