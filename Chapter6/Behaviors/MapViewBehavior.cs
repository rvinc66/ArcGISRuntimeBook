using Esri.ArcGISRuntime.Controls;
using Esri.ArcGISRuntime.Layers;
using GalaSoft.MvvmLight.Messaging; //added
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows; //added
using System.Windows.Interactivity;

namespace Chapter6.Behaviors
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
            OverlayItemsControl overlays = mapView.Overlays;
            OverlayItemsCollection collection = overlays.Items;
            System.Windows.Controls.Border overlayControl = (System.Windows.Controls.Border)collection[0];

            // get the location tapped on the map
            var mapPoint = e.Location;

            // Get possible features and if none found, move to next layer
            FeatureLayer featureLayer = (FeatureLayer)mapView.Map.Layers[1];
           
            // Get possible features and if none found, move to next layer
            var foundFeatures = await featureLayer.HitTestAsync(mapView, new Rect(e.Position, new Size(10, 10)), 1);

            if (foundFeatures.Count() == 0)
                return;

            var feature = await featureLayer.FeatureTable.QueryAsync(foundFeatures[0]);

            overlayControl.DataContext = feature.Attributes;

            Esri.ArcGISRuntime.Controls.MapView.SetViewOverlayAnchor(overlayControl, mapPoint);
            overlayControl.Visibility = Visibility.Visible;
 
        }
        void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            MapView mapView = sender as MapView;
            Messenger.Default.Send<MapView>(mapView);
        }
    }
}
