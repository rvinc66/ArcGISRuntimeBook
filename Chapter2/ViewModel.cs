using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Chapter2
{
    public class ViewModel : INotifyPropertyChanged
    {
        public Model myModel { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public ViewModel()
        {
            myModel = new Model();
        }

        public string BasemapUri
        {
            get { return myModel.BasemapLayerUri;  }
            set 
            { 
                this.myModel.BasemapLayerUri = value;
                OnPropertyChanged("BasemapUri");
            }
        }
        public string USAUri
        {
            get { return myModel.USALayerUri; }
            set 
            {
                this.myModel.USALayerUri = value;
                OnPropertyChanged("USAUri");
            }
        }
        public string SearchText
        {
            get { return myModel.SearchText; }
            set 
            { 
                this.myModel.SearchText = value;
                OnPropertyChanged("SearchText");
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string member = "")
        {
            var eventHandler = PropertyChanged;
            if (eventHandler != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(member));
            }
        }
    }
}
