using Esri.ArcGISRuntime.Controls;
using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Layers;
using Esri.ArcGISRuntime.LocalServices;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.Tasks.Geoprocessing;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Chapter10b.ViewModels
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class ClipViewModel : ViewModelBase
    {
        private MapView mapView;
        public RelayCommand ClipRelayCommand { get; private set; }

        // input 
        SimpleLineSymbol simpleInputLineSymbol = null;
        SimpleRenderer inputLineRenderer = null;

        public string status = "Ready!";

        // result
        SimpleFillSymbol simpleResultFillSymbol = null;
        SimpleLineSymbol simpleResultLineSymbol = null;
        SimpleRenderer simpleResultRenderer = null;

        double distance = 100.0;
        GraphicsLayer resultGraphicsLayer = null;
        GraphicsLayer inputGraphicsLayer = null;
        private Geoprocessor gpTask;
        private string clipGPKPath = @"C:\ArcGISRuntimeBook\Data\clip-features.gpk";
        private string gpUrl = string.Empty;
        private LocalGeoprocessingService localGPService = null;

        /// <summary>
        /// Initializes a new instance of the ViewshedViewModel class.
        /// </summary>
        public ClipViewModel()
        {
            this.ClipRelayCommand = new RelayCommand(Clip);

            Messenger.Default.Register<Esri.ArcGISRuntime.Controls.MapView>(this, (mapView) =>
            {
                this.mapView = mapView;

                this.simpleInputLineSymbol = new SimpleLineSymbol();
                this.simpleInputLineSymbol.Color = System.Windows.Media.Colors.Red;
                this.simpleInputLineSymbol.Width = 2;
                this.simpleInputLineSymbol.Style = SimpleLineStyle.Solid;

                this.simpleResultLineSymbol = new SimpleLineSymbol();
                this.simpleResultLineSymbol.Color = (Color)ColorConverter.ConvertFromString("#FF0000FF");
                
                this.simpleResultFillSymbol = new SimpleFillSymbol();
                this.simpleResultFillSymbol.Color = (Color)ColorConverter.ConvertFromString("#770000FF");
                this.simpleResultFillSymbol.Outline = this.simpleResultLineSymbol;

                this.simpleResultRenderer = new SimpleRenderer();
                this.simpleResultRenderer.Symbol = this.simpleResultFillSymbol;

                this.inputLineRenderer = new SimpleRenderer();
                this.inputLineRenderer.Symbol = this.simpleInputLineSymbol;


               
                this.localGPService = new LocalGeoprocessingService(this.clipGPKPath, GeoprocessingServiceType.SubmitJob);
                this.localGPService.StartAsync();
                

                this.resultGraphicsLayer = new GraphicsLayer();
                this.resultGraphicsLayer.ID = "Clip Result";
                this.resultGraphicsLayer.DisplayName = "Viewshed";
                this.resultGraphicsLayer.Renderer = this.simpleResultRenderer;
                this.resultGraphicsLayer.InitializeAsync();

                this.inputGraphicsLayer = new GraphicsLayer();
                this.inputGraphicsLayer.ID = "Input Line";
                this.inputGraphicsLayer.DisplayName = "Input Line";
                this.inputGraphicsLayer.Renderer = this.inputLineRenderer;
                this.inputGraphicsLayer.InitializeAsync();

                this.mapView.Map.Layers.Add(this.inputGraphicsLayer);
                this.mapView.Map.Layers.Add(this.resultGraphicsLayer);

            });
        }

      
        public async void Clip()
        {
            // get the local GP server's URL
            this.gpUrl = this.localGPService.UrlGeoprocessingService;

            // start the GP 
            this.gpTask = new Geoprocessor(new Uri(this.gpUrl + "/ClipFeatures"));

            //get the user's input line
            var inputLine = await this.mapView.Editor.RequestShapeAsync(DrawShape.Polyline) as Polyline;

            // clear the graphics layers
            this.resultGraphicsLayer.Graphics.Clear();
            this.inputGraphicsLayer.Graphics.Clear();

            // add new graphic to layer
            this.inputGraphicsLayer.Graphics.Add(new Graphic { Geometry = inputLine, Symbol = this.simpleInputLineSymbol });

            // add the parameters
            var parameter = new GPInputParameter();
            parameter.GPParameters.Add(new GPFeatureRecordSetLayer("Input", inputLine));
            parameter.GPParameters.Add(new GPLinearUnit("Linear_Unit", LinearUnits.Miles, this.Distance));

            // poll the task
            var result = await SubmitAndPollStatusAsync(parameter);

            // add successful results to the map
            if (result.JobStatus == GPJobStatus.Succeeded)
            {
                this.Status = "Finished processing. Retrieving results...";

                var resultData = await gpTask.GetResultDataAsync(result.JobID, "Clipped_Counties");
                if (resultData is GPFeatureRecordSetLayer)
                {
                    GPFeatureRecordSetLayer gpLayer = resultData as GPFeatureRecordSetLayer;
                    if (gpLayer.FeatureSet.Features.Count == 0)
                    {
                        // the the map service results
                        var resultImageLayer = await gpTask.GetResultImageLayerAsync(result.JobID, "Clipped_Counties");

                        // make the result image layer opaque
                        GPResultImageLayer gpImageLayer = resultImageLayer;
                        gpImageLayer.Opacity = 0.5;
                        this.mapView.Map.Layers.Add(gpImageLayer);
                        this.Status = "Greater than 500 features returned.  Results drawn using map service.";
                        return;
                    }

                    // get the result features and add them to the GraphicsLayer
                    var features = gpLayer.FeatureSet.Features;
                    foreach (Feature feature in features)
                    {
                        this.resultGraphicsLayer.Graphics.Add(feature as Graphic);
                    }
                  
                }
                this.Status = "Success!!!";
            }

        }
        // Submit GP Job and Poll the server for results every 2 seconds.
        private async Task<GPJobInfo> SubmitAndPollStatusAsync(GPInputParameter parameter)
        {
            // Submit gp service job
            var result = await gpTask.SubmitJobAsync(parameter);

            // Poll for the results async
            while (result.JobStatus != GPJobStatus.Cancelled && result.JobStatus != GPJobStatus.Deleted
                && result.JobStatus != GPJobStatus.Succeeded && result.JobStatus != GPJobStatus.TimedOut)
            {
                result = await gpTask.CheckJobStatusAsync(result.JobID);

                foreach (GPMessage msg in result.Messages)
                {
                    this.Status = string.Join(Environment.NewLine, msg.Description);
                }
                await Task.Delay(2000);
            }

            return result;
        }
        public double Distance
        {
            get { return this.distance; }
            set
            {

                this.distance  = value;
                RaisePropertyChanged("Distance");
            }
        }

        public string Status
        {
            get { return this.status; }
            set
            {
                this.status = value;
                RaisePropertyChanged("Status");
            }
        }
    }
}