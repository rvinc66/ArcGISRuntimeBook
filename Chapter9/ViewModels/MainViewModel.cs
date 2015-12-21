using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;

using Esri.ArcGISRuntime.Tasks.Query;
using Esri.ArcGISRuntime.Geometry;

using Chapter9.Services;
using Chapter9.Models;

using Esri.ArcGISRuntime.Controls;
using Esri.ArcGISRuntime.Layers;
using Esri.ArcGISRuntime.LocalServices;
using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Symbology;

namespace Chapter9.ViewModels
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
            public RelayCommand SelectByRectangleRelayCommand { get; private set; }
            public RelayCommand AddPointRelayCommand { get; private set; }
            GraphicsLayer graphicsLayer = null;
            long[] featureLayerRowIDs;
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
                    this.SelectByRectangleRelayCommand = new RelayCommand(SelectByRectangle);
                    this.AddPointRelayCommand = new RelayCommand(AddPoint);

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

            public async void AddPoint()
            {
                // get a point from the user
                var mapPoint = await this.mapView.Editor.RequestPointAsync();
                // open the local geodatabase
                var gdb = await Esri.ArcGISRuntime.Data.Geodatabase.OpenAsync(this.GDB);

                // open the table and create a new feature using its schema
                FeatureTable gdbTable;

                foreach (FeatureTable table in gdb.FeatureTables)
                {
                    gdbTable = table;

                    var newFeature = new Esri.ArcGISRuntime.Data.GeodatabaseFeature(gdbTable.Schema);

                    // assign the point from the user as the feature's geometry
                    newFeature.Geometry = mapPoint;

                    // get the attributes from the feature (a Dictionary<string, object>) and set some values
                    var attributes = newFeature.Attributes;
                    attributes["POST_ID"] = "123456"; 
                    
                    // add the new feature to the table, the OID of the new feature is returned
                    var recNo = await gdbTable.AddAsync(newFeature);
                    
                    break;
                }


                await this.mapView.SetViewAsync(mapPoint, this.mapView.Scale + 1);
            }

            public async void SelectByRectangle()
            {
                try
                {
                    FeatureLayer featureLayer = (FeatureLayer)this.mapView.Map.Layers[1];

                    featureLayer.ClearSelection();

                    // Get the Editor associated with the MapView. The Editor enables drawing and editing graphic objects.
                    Esri.ArcGISRuntime.Controls.Editor myEditor = this.mapView.Editor;

                    // Get the Envelope that the user draws on the Map. Execution of the code stops here until the user is done drawing the rectangle.
                    Geometry geometry = await myEditor.RequestShapeAsync(DrawShape.Rectangle);
                    Envelope envelope = geometry.Extent; // The Extent of the returned geometry should be exactly the same shape as the DrawShape.Rectangle.

                    if (envelope != null)
                    {
                        // Get the lower-left MapPoint (real world coordinates) from the Envelope the user drew on the Map and then translate it into 
                        // a Microsoft Point object.
                        MapPoint mapPoint1 = new MapPoint(envelope.Extent.XMin, envelope.YMin);
                        System.Windows.Point windowsPoint1 = this.mapView.LocationToScreen(mapPoint1);

                        // Get the upper-right MapPoint (real world coordinates) from the Envelope the user drew on the Map and then translate it into 
                        // a Microsoft Point object.
                        MapPoint mapPoint2 = new MapPoint(envelope.Extent.XMax, envelope.YMax);
                        System.Windows.Point windowsPoint2 = this.mapView.LocationToScreen(mapPoint2);

                        // Create a Windows Rectangle from the Windows Point objects.
                        System.Windows.Rect windowsRect = new System.Windows.Rect(windowsPoint1, windowsPoint2);

                        // Get the FeatureTable from the FeatureLayer.
                        FeatureTable featureTable = featureLayer.FeatureTable;

                        // Get the number of records in the FeatureTable.
                        long count = featureTable.RowCount;

                        // Use the FeatureLayer.HitTestAsync Method to retrieve the FeatureLayer IDs that are within or cross the envelope that was
                        // drawn on the Map by the user. It is important to note that by passing in the variable myRowCount (which is the maximum
                        // number of features in the FeatureLayer), you are able to select up to the number of features in the FeatureLayer. If
                        // you were to leave off this optional last parameter then the HitTestAsync would only return one record! 
                        featureLayerRowIDs = await featureLayer.HitTestAsync(this.mapView, windowsRect, System.Convert.ToInt32(count));

                        if (featureLayerRowIDs.Length > 0)
                        {
                            // We have at least one record in the FeatureLayer selected.

                            // Cause the features in the FeatureLayer to highlight (cyan) in the Map.
                            featureLayer.SelectFeatures(featureLayerRowIDs);

                        }
                    }
                }
                catch (System.Threading.Tasks.TaskCanceledException)
                {
                    // This exception occurred because the user has already clicked the button but has not drawn a rectangle on the Map yet.
                    Messenger.Default.Send<NotificationMessage>(new NotificationMessage("Drag a rectangle across the map to select some features."));
                }
                catch (System.Exception ex)
                {
                    // We had some kind of issue. Display to the user so it can be corrected.
                    Messenger.Default.Send<NotificationMessage>(new NotificationMessage(ex.Message));
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
