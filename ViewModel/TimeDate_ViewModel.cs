using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TimeDate_Application.Model;

namespace TimeDate_Application.ViewModel
{
    class TimeDate_ViewModel : INotifyPropertyChanged
    {
        private TimeDate current = new TimeDate();

        public TimeDate Current
        {
            get { return current; }
            set { current = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string name = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
