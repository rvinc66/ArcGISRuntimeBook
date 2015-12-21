using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;

using Esri.ArcGISRuntime.Tasks.Query;
using Esri.ArcGISRuntime.Geometry;

using Chapter3a.Services;
using Chapter3a.Models;

using Esri.ArcGISRuntime.Controls;
using Esri.ArcGISRuntime.Layers;
using Esri.ArcGISRuntime.LocalServices;
using Esri.ArcGISRuntime.Data;


namespace Chapter3a.ViewModels
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

                    Messenger.Default.Register<Esri.ArcGISRuntime.Controls.MapView>(this, (mapView) =>
                    {
                        this.mapView = mapView;

                        ArcGISLocalTiledLayer localTiledLayer = new ArcGISLocalTiledLayer(this.TilePackage);
                        localTiledLayer.ID = "SF Basemap";
                        localTiledLayer.InitializeAsync();

                        this.mapView.Map.Layers.Add(localTiledLayer);

                        this.CreateLocalServiceAndDynamicLayer();
                        this.CreateFeatureLayers();

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
                        DisplayName = table.Name,
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
