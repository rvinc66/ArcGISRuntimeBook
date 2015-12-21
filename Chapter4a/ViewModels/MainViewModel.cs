using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;

using Esri.ArcGISRuntime.Tasks.Query;
using Esri.ArcGISRuntime.Geometry;

using Chapter4a.Services;
using Chapter4a.Models;

using Esri.ArcGISRuntime.Controls;
using Esri.ArcGISRuntime.Layers;
using Esri.ArcGISRuntime.Symbology.SceneSymbology;
using Esri.ArcGISRuntime.Symbology;
using System.Windows.Media;

namespace Chapter4a.ViewModels
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

    /// <summary>
    /// Initializes a new instance of the MainViewModel class.
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private Model myModel = null;
        private ObservableCollection<FindItem> listResults;
        public RelayCommand<int> SearchRelayCommand { get; private set; }
        public RelayCommand ZoomRelayCommand { get; private set; }
        public RelayCommand<LayerLoadedEventArgs> LayerLoadedCommand { get; private set; }
        private Camera camera;
        private SceneView sceneView = null;
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
                this.ZoomRelayCommand = new RelayCommand(Zoom);
                //this.LayerLoadedCommand = new RelayCommand<LayerLoadedEventArgs>(e => MyMapView_LayerLoaded(e));
                
                Messenger.Default.Register<Esri.ArcGISRuntime.Controls.SceneView>(this, (sceneView) =>
                {
                    this.sceneView = sceneView;

                    Uri uriBasemap = new Uri("http://services.arcgisonline.com/arcgis/rest/services/World_Imagery/MapServer");
                    ArcGISTiledMapServiceLayer basemapLayer = new ArcGISTiledMapServiceLayer(uriBasemap);

                    Uri uriUSA = new Uri(this.USAUri);
                    ArcGISDynamicMapServiceLayer dynamicMapLayer = new ArcGISDynamicMapServiceLayer(uriUSA);

                    this.sceneView.Scene.Layers.Add(basemapLayer);
                    this.sceneView.Scene.Layers.Add(dynamicMapLayer);


                    //DrawSphere();

                    //DrawModel();

                    this.camera = this.sceneView.Camera;

                });
            }
        }

        private void DrawModel()
        {
            ModelMarkerSymbol mms = new ModelMarkerSymbol();
            mms.SourceUri = "Data/M-14/M-14.obj";
            mms.Scale = 1000;

            SimpleRenderer sr = new SimpleRenderer();
            sr.Symbol = mms;

            GraphicsOverlay graphicsOverlay = new GraphicsOverlay()
            {
                RenderingMode = GraphicsRenderingMode.Dynamic,
                Renderer = sr,
                SceneProperties = new LayerSceneProperties()
                {
                    SurfacePlacement = SurfacePlacement.Relative
                }
            };

            MapPoint mp = new MapPoint(-122.4167, 37.7833, 55000);

            Graphic gm = new Graphic(mp);
            graphicsOverlay.Graphics.Add(gm);

            this.sceneView.GraphicsOverlays.Add(graphicsOverlay);
        }

        private void DrawSphere()
        {
            // create a new point (MapPoint); pass x, y, and z coordinates in the constructor
            var point = new MapPoint(-122.4167, 37.7833,  6000);

            // create a graphics layer
            GraphicsLayer graphicsLayer = new GraphicsLayer();
            graphicsLayer.ID = "Graphics Layer";
            graphicsLayer.SceneProperties.SurfacePlacement =
                                SurfacePlacement.Absolute;
            graphicsLayer.InitializeAsync();

            // creates a red sphere with a radius of 20,000 meters
            var sphereSym = new SphereMarkerSymbol();
            sphereSym.Color = Colors.Red;

            sphereSym.Radius = 20000;

            // create the graphic
            var graphic = new Graphic(point, sphereSym);
           
            graphicsLayer.Graphics.Add(graphic);

            // add the graphics layers to the scene
            this.sceneView.Scene.Layers.Add(graphicsLayer);
        }
        //private void MyMapView_LayerLoaded(LayerLoadedEventArgs e)
        //{
        //    if (e.LoadError == null)
        //        return;

        //    System.Diagnostics.Debug.WriteLine(string.Format("Error while loading layer : {0} - {1}", e.Layer.ID, e.LoadError.Message));
        //}
        public async void Zoom()
        {
            // define a new camera over Italy
            var camera = new Camera(41.9, 12.5, 330.0, 180, 73.0);
            // create a ViewPoint with the camera, a target geometry (Envelope), and rotation (same as camera heading)
            var viewPoint = new Viewpoint(camera, new Envelope(12.5, 41.85, 12.6, 41.95, SpatialReferences.Wgs84), 126.0);
            var animationDuration = new TimeSpan(0, 0, 2);

            await this.sceneView.SetViewAsync(viewPoint, animationDuration, true);
        }

        //public async void Zoom()
        //{
        //    // define a new camera over San Francisco
        //    var camera = new Camera(32.715, -117.1625, 330.0, 180, 73.0);
        //    // create a ViewPoint with the camera, a target geometry (Envelope), and rotation (same as camera heading)
        //    var viewPoint = new Viewpoint(camera, new Envelope(31, -116, 33, 118, SpatialReferences.Wgs84), 126.0);
        //    var animationDuration = new TimeSpan(0, 0, 2);

        //    await this.sceneView.SetViewAsync(viewPoint, animationDuration, true);
        //}
        //public async void Zoom()
        //{
        //    // define a new camera over Everest
        //    var camera = new Camera(28.1, 86.9253, 8000, 180, 73.0);
        //    // create a ViewPoint with the camera, a target geometry (Envelope), and rotation (same as camera heading)
        //    var viewPoint = new Viewpoint(camera, new Envelope(27.9, 86.7, 28.1, 87.3, SpatialReferences.Wgs84), 126.0);
        //    var animationDuration = new TimeSpan(0, 0, 2);

        //    await this.sceneView.SetViewAsync(viewPoint, animationDuration, true);
        //}
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

            FindResult findResult = 
                     findTask.ExecuteAsync(findParameters).Result;

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
        public string ElevationUri
        {
            get { return myModel.ElevationLayerUri; }
            set
            {
                this.myModel.ElevationLayerUri = value;
                RaisePropertyChanged("ElevationUri");
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
