using Esri.ArcGISRuntime.Controls;
using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Layers;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.Tasks.Geoprocessing;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Windows.Media;

namespace Chapter10.ViewModels
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class ViewshedViewModel : ViewModelBase
    {
        private MapView mapView;
        public RelayCommand CreateViewshedRelayCommand { get; private set; }
        SimpleMarkerSymbol sms = null;
        SimpleRenderer simpleRenderer = null;
        SimpleLineSymbol simpleLineSymbol = null;
        SimpleFillSymbol simpleFillSymbol = null;
        public string status = "Ready!";

        SimpleRenderer viewshedRenderer = null;
        double distance = 10.0;
        GraphicsLayer viewshedGraphicsLayer = null;
        GraphicsLayer inputGraphicsLayer = null;
        private Geoprocessor gpTask;
        private const string viewshedServiceUrl =
       "http://sampleserver6.arcgisonline.com/arcgis/rest/services/Elevation/ESRI_Elevation_World/GPServer/Viewshed";

        /// <summary>
        /// Initializes a new instance of the ViewshedViewModel class.
        /// </summary>
        public ViewshedViewModel()
        {
            this.CreateViewshedRelayCommand = new RelayCommand(CreateViewshed);

            Messenger.Default.Register<Esri.ArcGISRuntime.Controls.MapView>(this, (mapView) =>
            {
                this.mapView = mapView;



                this.sms = new SimpleMarkerSymbol();
                this.sms.Color = System.Windows.Media.Colors.Black;
                sms.Style = SimpleMarkerStyle.X;
                sms.Size = 20;

                this.simpleRenderer = new SimpleRenderer();
                this.simpleRenderer.Symbol = sms;

                this.simpleLineSymbol = new SimpleLineSymbol();
                this.simpleLineSymbol.Color = System.Windows.Media.Colors.Red;
                this.simpleLineSymbol.Width = 2;
                this.simpleLineSymbol.Style = SimpleLineStyle.Solid;

                this.simpleFillSymbol = new SimpleFillSymbol();
                this.simpleFillSymbol.Color =  (Color)ColorConverter.ConvertFromString("#44FF9999");
                this.simpleFillSymbol.Outline = simpleLineSymbol;

                this.viewshedRenderer = new SimpleRenderer();
                this.viewshedRenderer.Symbol = this.simpleFillSymbol;

                gpTask = new Geoprocessor(new Uri(viewshedServiceUrl));

                this.viewshedGraphicsLayer = new GraphicsLayer();
                this.viewshedGraphicsLayer.ID = "Viewshed";
                this.viewshedGraphicsLayer.DisplayName = "Viewshed";
                this.viewshedGraphicsLayer.Renderer = this.viewshedRenderer;
                this.viewshedGraphicsLayer.InitializeAsync();

                this.inputGraphicsLayer = new GraphicsLayer();
                this.inputGraphicsLayer.ID = "Input Point";
                this.inputGraphicsLayer.DisplayName = "Input Point";
                this.inputGraphicsLayer.Renderer = this.simpleRenderer;
                this.inputGraphicsLayer.InitializeAsync();

                this.mapView.Map.Layers.Add(this.inputGraphicsLayer);
                this.mapView.Map.Layers.Add(this.viewshedGraphicsLayer);

            });


        }

      
        public async void CreateViewshed()
        {
           // // get a point from the user
           var mapPoint = await this.mapView.Editor.RequestPointAsync();

           // clear the graphics layers
           this.viewshedGraphicsLayer.Graphics.Clear();
           this.inputGraphicsLayer.Graphics.Clear();
           
            // add new graphic to layer
           this.inputGraphicsLayer.Graphics.Add(new Graphic{ Geometry = mapPoint, Symbol = this.sms });

           // specify the input parameters
           var parameter = new GPInputParameter() { OutSpatialReference = SpatialReferences.WebMercator };
           parameter.GPParameters.Add(new GPFeatureRecordSetLayer("Input_Observation_Point", mapPoint));
           parameter.GPParameters.Add(new GPLinearUnit("Viewshed_Distance", LinearUnits.Miles, this.distance));

           // Send to the server
           this.Status = "Processing on server...";
           var result = await gpTask.ExecuteAsync(parameter);
           if (result == null || result.OutParameters == null || !(result.OutParameters[0] is GPFeatureRecordSetLayer))
               throw new ApplicationException("No viewshed graphics returned for this start point.");
           
           // process the output
           this.Status = "Finished processing. Retrieving results...";
           var viewshedLayer = result.OutParameters[0] as GPFeatureRecordSetLayer;
           var features = viewshedLayer.FeatureSet.Features;
            foreach (Feature feature in features)
            {
                this.viewshedGraphicsLayer.Graphics.Add(feature as Graphic);
            }
            this.Status = "Finished!!";
        }

        public double Distance
        {
            get { return this.distance; }
            set
            {
                if (value > 12.4) // Task only works out to 20,000 meters.
                    value = 12.4;

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