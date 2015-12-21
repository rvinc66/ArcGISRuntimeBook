using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;

using Esri.ArcGISRuntime.Tasks.Query;
using Esri.ArcGISRuntime.Geometry;

using Chapter8.Services;
using Chapter8.Models;

using Esri.ArcGISRuntime.Controls;
using Esri.ArcGISRuntime.Layers;
using Esri.ArcGISRuntime.LocalServices;
using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.Tasks.NetworkAnalyst;

namespace Chapter8.ViewModels
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
        private MapView mapView = null;
        public RelayCommand AddStopsRelayCommand { get; private set; }
        GraphicsLayer routeGraphicsLayer = null;
        GraphicsLayer stopsGraphicsLayer = null;
        LocalRouteTask routeTask;
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
                this.AddStopsRelayCommand = new RelayCommand(AddStops);

                Messenger.Default.Register<Esri.ArcGISRuntime.Controls.MapView>(this, (mapView) =>
                {
                    this.mapView = mapView;
                    this.mapView.MaxScale = 500;

                    ArcGISLocalTiledLayer localTiledLayer = new ArcGISLocalTiledLayer(this.TilePackage);
                    localTiledLayer.ID = "SF Basemap";
                    localTiledLayer.InitializeAsync();

                    this.mapView.Map.Layers.Add(localTiledLayer);

                    this.CreateLocalServiceAndDynamicLayer();
                    this.CreateFeatureLayers();


                    //============================
                    // Create a new renderer to symbolize the routing polyline and apply it to the GraphicsLayer
                    SimpleLineSymbol polylineRouteSymbol = new SimpleLineSymbol();
                    polylineRouteSymbol.Color = System.Windows.Media.Colors.Red;
                    polylineRouteSymbol.Style = Esri.ArcGISRuntime.Symbology.SimpleLineStyle.Solid;
                    polylineRouteSymbol.Width = 4;
                    SimpleRenderer polylineRouteRenderer = new SimpleRenderer();
                    polylineRouteRenderer.Symbol = polylineRouteSymbol;

                    // Create a new renderer to symbolize the start and end points that define the route and apply it to the GraphicsLayer
                    SimpleMarkerSymbol stopSymbol = new SimpleMarkerSymbol();
                    stopSymbol.Color = System.Windows.Media.Colors.Green;
                    stopSymbol.Size = 12;
                    stopSymbol.Style = SimpleMarkerStyle.Circle;
                    SimpleRenderer stopRenderer = new SimpleRenderer();
                    stopRenderer.Symbol = stopSymbol;
                   
                    // create the route results graphics layer
                    this.routeGraphicsLayer = new GraphicsLayer();
                    this.routeGraphicsLayer.ID = "RouteResults";
                    this.routeGraphicsLayer.DisplayName = "Routes";
                    this.routeGraphicsLayer.Renderer = polylineRouteRenderer;
                    this.routeGraphicsLayer.InitializeAsync();
                    
                    this.mapView.Map.Layers.Add(this.routeGraphicsLayer);

                    // create the stops graphics layer
                    this.stopsGraphicsLayer = new GraphicsLayer();
                    this.stopsGraphicsLayer.ID = "Stops";
                    this.stopsGraphicsLayer.DisplayName = "Stops";
                    this.stopsGraphicsLayer.Renderer = stopRenderer;
                    this.stopsGraphicsLayer.InitializeAsync();
                    this.mapView.Map.Layers.Add(this.stopsGraphicsLayer);

                    // Offline routing task.
                    routeTask = new LocalRouteTask(@"C:\ArcGISRuntimeBook\Data\Networks\RuntimeSanFrancisco.geodatabase", "Streets_ND");


                });
            }
        }

        private async void CreateFeatureLayers()
        {
            var gdb = await Geodatabase.OpenAsync(this.GDB);

            Envelope extent = null;
            foreach (var table in gdb.FeatureTables)
            {
                var flayer = new FeatureLayer()
                {
                    ID = table.Name,
                    DisplayName = "Parking Meters",
                    FeatureTable = table
                };


                if (!Geometry.IsNullOrEmpty(table.ServiceInfo.Extent))
                {
                    if (Geometry.IsNullOrEmpty(extent))
                        extent = table.ServiceInfo.Extent;
                    else
                        extent = extent.Union(table.ServiceInfo.Extent);
                }

                this.mapView.Map.Layers.Add(flayer);
            }

            await this.mapView.SetViewAsync(extent.Expand(2.10));
        }



        public async void AddStops()
        {
            // If the user clicked the SolveRouteButton more than once, clear out any existing stops and routes graphics.
            routeGraphicsLayer.Graphics.Clear();
            stopsGraphicsLayer.Graphics.Clear();

            try
            {

                // Mouse click 1: setting the start point for the route
                // ---------------------------------------------------

                // Get the Editor from the MapView.
                Editor startPointEditor = this.mapView.Editor;

                // Get the MapPoint from where the user clicks in the Map Control. This will be the starting point for the route.
                MapPoint startLocationMapPoint = await startPointEditor.RequestPointAsync();

                // Create a new Graphic and set it's geometry to the user clicked MapPoint
                Graphic startPointGraphic = new Graphic();
                startPointGraphic.Geometry = startLocationMapPoint;

                // Add the start point graphic to the stops GraphicsLayer.
                stopsGraphicsLayer.Graphics.Add(startPointGraphic);

                // Mouse click 2: setting the end point for the route
                // ---------------------------------------------------

                // Get the Editor from the MapView.
                Editor endPointEditor = this.mapView.Editor;

                // Get the MapPoint from where the user clicks in the Map Control. This will be the ending point for the route.
                MapPoint endLocationMapPoint = await startPointEditor.RequestPointAsync();

                // Create a new Graphic and set it's geometry to the user clicked MapPoint
                Graphic endPointGraphic = new Graphic();
                endPointGraphic.Geometry = endLocationMapPoint;

                // Add the start point graphic to the stops GraphicsLayer.
                stopsGraphicsLayer.Graphics.Add(endPointGraphic);

                // Set the arguments for the RouteTask:

                // Get the RouteParameters from the RouteTask.
                RouteParameters routeParameters = await routeTask.GetDefaultParametersAsync();

                // Define the settings for the RouteParameters. This includes setting the SpatialReference, 
                // ReturnDirections, DirectionsLengthUnit and Stops. 
                routeParameters.OutSpatialReference = this.mapView.SpatialReference;
                routeParameters.ReturnDirections = false;
                routeParameters.DirectionsLengthUnit = LinearUnits.Kilometers;


                // Define a List of Graphics based upon the user start and end clicks in the Map Control that will serve as input
                // parameters for the RouteTask operation.
                List<Graphic> graphicsStops = new List<Graphic>();
                graphicsStops.Add(new Graphic(startLocationMapPoint));
                graphicsStops.Add(new Graphic(endLocationMapPoint));

                // Set the stops for the Route.
                routeParameters.SetStops(graphicsStops);

                // Call the asynchronous function to solve the RouteTask.
                RouteResult routeResult = await routeTask.SolveAsync(routeParameters);

                // Ensure we got at least one route back.
                if (routeResult.Routes.Count > 0)
                {

                    // Get the first Route from the List of Routes
                    Route firstRoute = routeResult.Routes[0];

                    // Get the Geometry from the Graphic in the first Route.
                    Geometry routeGeometry = firstRoute.RouteFeature.Geometry;

                    // Create a new Graphic based upon the Graphic.
                    Graphic routeGraphic = new Graphic(routeGeometry);

                    // Add the Graphic (a polyline) to the route GraphicsLayer. 
                    this.routeGraphicsLayer.Graphics.Add(routeGraphic);

                }
            }
            catch (System.AggregateException ex)
            {
                // There was a problem, display the results to the user.
                var innermostExceptions = ex.Flatten().InnerExceptions;
                if (innermostExceptions != null && innermostExceptions.Count > 0)
                        Messenger.Default.Send<NotificationMessage>(new NotificationMessage((innermostExceptions[0].Message.ToString())));
                else
                    Messenger.Default.Send<NotificationMessage>(new NotificationMessage(ex.Message.ToString()));
            }
            catch (System.Exception ex)
            {
                // There was a problem, display the results to the user.
                Messenger.Default.Send<NotificationMessage>(new NotificationMessage("Error: " + ex.Message.ToString()));
            }
            finally
            {
            }
        }

        public async void CreateLocalServiceAndDynamicLayer()
        {
            LocalMapService localMapService = new LocalMapService(this.MapPackage);
            await localMapService.StartAsync();

            ArcGISDynamicMapServiceLayer arcGISDynamicMapServiceLayer = new ArcGISDynamicMapServiceLayer()
            {
                ID = "Restaurants",
                ServiceUri = localMapService.UrlMapService,
            };

            this.mapView.Map.Layers.Add(arcGISDynamicMapServiceLayer);
        }

        public string TilePackage
        {
            get { return this.myModel.TilePackage; }
            set
            {
                this.myModel.TilePackage = value;
                RaisePropertyChanged("TilePackage");
            }
        }
        public string MapPackage
        {
            get { return myModel.MapPackage; }
            set
            {
                this.myModel.MapPackage = value;
                RaisePropertyChanged("MapPackage");
            }
        }
        public string GDB
        {
            get { return myModel.GDB; }
            set
            {
                this.myModel.GDB = value;
                RaisePropertyChanged("USAUri");
            }
        }

    }
}
