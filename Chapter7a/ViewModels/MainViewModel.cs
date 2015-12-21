using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;

using Esri.ArcGISRuntime.Tasks.Query;
using Esri.ArcGISRuntime.Geometry;

using Chapter7a.Services;
using Chapter7a.Models;

using Esri.ArcGISRuntime.Controls;
using Esri.ArcGISRuntime.Layers;
using Esri.ArcGISRuntime.LocalServices;
using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Symbology;

namespace Chapter7a.ViewModels
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
            public RelayCommand<string> SearchRelayCommand { get; private set; }
            GraphicsLayer graphicsLayer = null;

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
                    this.SearchRelayCommand = new RelayCommand<string>(Search);

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

                        this.graphicsLayer = new Esri.ArcGISRuntime.Layers.GraphicsLayer();
                        this.graphicsLayer.ID = "Results";
                        this.graphicsLayer.InitializeAsync();
                        this.mapView.Map.Layers.Add(this.graphicsLayer);
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

                await this.mapView.SetViewAsync(extent.Expand(1.10));
            }

            public async void SearchByMeterID(string searchString)
            {
                SimpleLineSymbol sls = new SimpleLineSymbol()
                {
                    Color = System.Windows.Media.Colors.Red,
                    Style = SimpleLineStyle.Solid,
                    Width = 2
                };

                // get the layer and table
                FeatureLayer featureLayer = this.mapView.Map.Layers[1] as FeatureLayer;
                GeodatabaseFeatureTable table = featureLayer.FeatureTable as GeodatabaseFeatureTable;

                // Define an attribute query
                var filter = new Esri.ArcGISRuntime.Data.QueryFilter();
                filter.WhereClause = "POST_ID = '" + searchString + "'"; // 666-13080

                // Execute the query and await results
                IEnumerable<Feature> features = await table.QueryAsync(filter);

                // iterate the feature. Should be one in this case.
                foreach (Feature feature in features)
                {
                    // Get the MapPoint, Project to Mercator so that we are working in meters
                    MapPoint mapPoint = feature.Geometry as MapPoint;
                    MapPoint pointMercator = GeometryEngine.Project(mapPoint, SpatialReferences.WebMercator) as MapPoint;
                    Geometry polygon = GeometryEngine.Buffer(pointMercator, 200);

                    // Re-project the polygon to WGS84 so that we can query against the layer which is in WGS84
                    Polygon polygonWgs84 = GeometryEngine.Project(polygon, SpatialReferences.Wgs84) as Polygon;

                    // add the circle (buffer)
                    Graphic graphic = new Graphic();
                    graphic.Symbol = sls;
                    graphic.Geometry = polygonWgs84;
                    this.graphicsLayer.Graphics.Add(graphic);

                    // Make sure the table supports querying
                    if (table.SupportsQuery)
                    {
                        // setup the query to use the polygon that's in WGS84
                        var query = new Esri.ArcGISRuntime.Data.SpatialQueryFilter();
                        query.Geometry = polygonWgs84 as Geometry;
                        query.SpatialRelationship = SpatialRelationship.Intersects;

                        var result = await table.QueryAsync(query);

                        int i = 1;
                        // Loop through query results
                        foreach (Esri.ArcGISRuntime.Data.Feature f in result)
                        {
                            // do something with results
                            System.Diagnostics.Debug.WriteLine(i.ToString());
                            i++;
                        }

                    }
                }
            }

            public async void Search(string searchString)
            {
                FeatureLayer featureLayer = this.mapView.Map.Layers[1] as FeatureLayer;
                GeodatabaseFeatureTable table = featureLayer.FeatureTable as GeodatabaseFeatureTable;

                // Define an attribute query
                var filter = new Esri.ArcGISRuntime.Data.QueryFilter();
                filter.WhereClause = "POST_ID = '" + searchString + "'"; // 666-13080

                // Execute the query and await results
                IEnumerable<Feature> features = await table.QueryAsync(filter);
               
                foreach(Feature feature in features)
                {
                    string address = feature.Attributes["STREETNAME"] as string;
                    System.Diagnostics.Debug.WriteLine("Address: " + address);
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
