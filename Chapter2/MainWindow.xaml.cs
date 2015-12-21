using Esri.ArcGISRuntime.Controls;
using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Layers;
using Esri.ArcGISRuntime.Tasks;
using Esri.ArcGISRuntime.Tasks.Query;

using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace Chapter2
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MyMapView_LayerLoaded(object sender, LayerLoadedEventArgs e)
        {
            if (e.LoadError == null)
                return;

            Debug.WriteLine(string.Format("Error while loading layer : {0} - {1}", e.Layer.ID, e.LoadError.Message));
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
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
            findParameters.SpatialReference = MyMapView.SpatialReference;

            findParameters.SearchText = SearchTextBox.Text;
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
            var msg = string.Format("Found {0} cities, {1} counties, and {2} states containing '" + SearchTextBox.Text +
                               "' in a Name attribute", foundCities, foundCounties,
                     foundStates);
            MessageBox.Show(msg);


            // Bind the results to a DataGrid control on the page
            MyDataGrid.ItemsSource = findResult.Results;
        }
    }
}
