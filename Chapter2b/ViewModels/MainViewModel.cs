using System.Collections.Generic;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;

using Esri.ArcGISRuntime.Tasks.Query;
using Esri.ArcGISRuntime.Geometry;

using Chapter2b.Services;
using Chapter2b.Models;

namespace Chapter2b.ViewModels
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private Model myModel = null;
        private ObservableCollection<FindItem> listResults;
        public RelayCommand<int> SearchRelayCommand { get; private set; }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
            }
            else
            {
                // Code runs "for real"
                ConfigService config = new ConfigService();
                this.myModel = config.LoadJSON();

                this.SearchRelayCommand = new RelayCommand<int>(Search);
            }
        }

        public async void Search(int wkid)
        {
            
            var findTask = new FindTask(new System.Uri(this.USAUri));

            var findParameters = new FindParameters();
            findParameters.LayerIDs.Add(0); // Cities
            findParameters.LayerIDs.Add(3); // Counties
            findParameters.LayerIDs.Add(2); // States

            findParameters.SearchFields.Add("name");
            findParameters.SearchFields.Add("areaname");
            findParameters.SearchFields.Add("state_name");

            findParameters.ReturnGeometry = true;

            SpatialReference sr = new SpatialReference(wkid);
            findParameters.SpatialReference = sr;


            findParameters.SearchText = this.SearchText;
            findParameters.Contains = true;

            FindResult findResult = await
                     findTask.ExecuteAsync(findParameters);

            var foundCities = 0;
            var foundCounties = 0;
            var foundStates = 0;

            // Loop thru results; count the matches found in each layer
            foreach (FindItem findItem in findResult.Results)
            {
                switch (findItem.LayerID)
                {
                    case 0: // Cities
                        foundCities++;
                        break;
                    case 3: // Counties
                        foundCounties++;
                        break;
                    case 2: // States
                        foundStates++;
                        break;
                }
            }

            // Report the number of matches for each layer
            var msg = string.Format("Found {0} cities, {1} counties, and {2} states containing '" + this.SearchText +
                               "' in a Name attribute", foundCities, foundCounties,
                     foundStates);

            // Bind the results to a DataGrid control on the page
            IReadOnlyList<FindItem> temp = findResult.Results;

            ObservableCollection<FindItem> obsCollection = new ObservableCollection<FindItem>();
            foreach (FindItem item in temp)
            {
                obsCollection.Add(item);
            }

            this.GridDataResults = obsCollection;

            // show message
            Messenger.Default.Send<NotificationMessage>(new NotificationMessage(msg));
        }
        public ObservableCollection<FindItem> GridDataResults
        {
            get { return this.listResults; }
            set
            {
                this.listResults = value;
                RaisePropertyChanged("GridDataResults");
            }
        }
        public string SearchText
        {
            get { return this.myModel.SearchText; }
            set
            {
                this.myModel.SearchText = value;
                RaisePropertyChanged("SearchText");
            }
        }
        public string BasemapUri
        {
            get { return myModel.BasemapLayerUri; }
            set
            {
                this.myModel.BasemapLayerUri = value;
                RaisePropertyChanged("BasemapUri");
            }
        }
        public string USAUri
        {
            get { return myModel.USALayerUri; }
            set
            {
                this.myModel.USALayerUri = value;
                RaisePropertyChanged("USAUri");
            }
        }

    }
}