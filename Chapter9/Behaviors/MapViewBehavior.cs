using Esri.ArcGISRuntime.Controls;
using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Layers;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.Tasks.Query;
using GalaSoft.MvvmLight.Messaging; //added
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Media;

namespace Chapter9.Behaviors
{
    public class MapViewBehavior : Behavior<MapView>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Loaded += AssociatedObject_Loaded;
            AssociatedObject.MapViewTapped += AssociatedObject_MapViewTapped;
           
        }




        private async void AssociatedObject_MapViewTapped(object sender, MapViewInputEventArgs e)
        {

            MapView mapView = sender as MapView;

            // get the location tapped on the map
            var mapPoint = e.Location;

            // Get possible features and if none found, move to next layer
            FeatureLayer featureLayer = (FeatureLayer)mapView.Map.Layers[1];

            // Get possible features and if none found, move to next layer
            var foundFeatures = await featureLayer.HitTestAsync(mapView, new Rect(e.Position, new Size(10, 10)), 1);

            featureLayer.SelectFeatures(foundFeatures);

        }
        void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            MapView mapView = sender as MapView;
            Messenger.Default.Send<MapView>(mapView);
        }
    }
}
