using Esri.ArcGISRuntime.Tasks.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Chapter2
{
    public class SearchCommand : ICommand
    {
        #region ICommand Members

        private ViewModel vm;
        public SearchCommand(ViewModel vm)
        {
            this.vm = vm;

        }
        public bool CanExecute(object parameter)
        {
            return true;
        }
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        public async void Execute(object parameter)
        {
            var url = "http://sampleserver6.arcgisonline.com/arcgis/rest/services/USA/MapServer";
            var findTask = new FindTask(new Uri(url));

            var findParameters = new FindParameters();
            findParameters.LayerIDs.Add(0); // Cities
            findParameters.LayerIDs.Add(3); // Counties
            findParameters.LayerIDs.Add(2); // States

            findParameters.SearchFields.Add("name");
            findParameters.SearchFields.Add("areaname");
            findParameters.SearchFields.Add("state_name");

            findParameters.ReturnGeometry = true;
            //findParameters.SpatialReference = MyMapView.SpatialReference;

            findParameters.SearchText = (string)parameter;
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
            var msg = string.Format("Found {0} cities, {1} counties, and {2} states containing '" + (string)parameter +
                               "' in a Name attribute", foundCities, foundCounties,
                     foundStates);
            MessageBox.Show(msg);

            vm.SearchResults = findResult.Results;
            // Bind the results to a DataGrid control on the page
           // MyDataGrid.ItemsSource = findResult.Results;

        }
        #endregion
    }
}
