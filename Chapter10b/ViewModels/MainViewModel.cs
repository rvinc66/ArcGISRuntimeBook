using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;

using Esri.ArcGISRuntime.Tasks.Query;
using Esri.ArcGISRuntime.Geometry;

using Esri.ArcGISRuntime.Controls;
using Esri.ArcGISRuntime.Layers;
using Esri.ArcGISRuntime.LocalServices;
using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Symbology;

namespace Chapter10b.ViewModels
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
 
                   Messenger.Default.Register<Esri.ArcGISRuntime.Controls.MapView>(this, (mapView) =>
                    {
                        this.mapView = mapView;

                        Uri uriTopo = new Uri("http://services.arcgisonline.com/ArcGIS/rest/services/World_Topo_Map/MapServer");
                        ArcGISDynamicMapServiceLayer dynamicMapLayer = new ArcGISDynamicMapServiceLayer(uriTopo);
                        dynamicMapLayer.InitializeAsync();

                        this.mapView.Map.Layers.Add(dynamicMapLayer);

                    });
                }
            }
        }
}
