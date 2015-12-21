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

namespace Chapter7a.Behaviors
{
    public class MapViewBehavior : Behavior<MapView>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Loaded += AssociatedObject_Loaded;
            AssociatedObject.MapViewTapped += AssociatedObject_MapViewTapped;
            AssociatedObject.MouseUp += AssociatedObject_MouseUp;
        }

        //async void AssociatedObject_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        //{
        //    MapView mapView = sender as MapView;
        //    var screenPoint = e.GetPosition(mapView);

        //    // Convert the screen point to a point in map coordinates
        //    var mapPoint = mapView.ScreenToLocation(screenPoint);

        //    // get the FeatureLayer
        //    FeatureLayer featureLayer = mapView.Map.Layers[1] as FeatureLayer;
        //    // Get the FeatureTable from the FeatureLayer.
        //    Esri.ArcGISRuntime.Data.FeatureTable featureTable = featureLayer.FeatureTable;

        //    // Translate the MapPoint into Microsoft Point object.
        //    System.Windows.Point windowsPoint = mapView.LocationToScreen(mapPoint);
        //    // get the Row IDs of the features that are hit
        //    long[] featureLayerRowIDs = await featureLayer.HitTestAsync(mapView, windowsPoint);

        //    if (featureLayerRowIDs.Length == 1)
        //    {

        //        // Cause the features in the FeatureLayer to highlight (cyan) in the Map.
        //        featureLayer.SelectFeatures(featureLayerRowIDs);

        //        // Perform a Query on the FeatureLayer.FeatureTable using the ObjectID of the feature tapped/clicked on in the map. Actually it is a List of ObjectID values 
        //        // but since we are trying for an Identify type of operation we really only have one ObjectID in the list.
        //        IEnumerable<GeodatabaseFeature> geoDatabaseFeature = (IEnumerable<GeodatabaseFeature>)await featureTable.QueryAsync(featureLayerRowIDs);

        //        foreach (Esri.ArcGISRuntime.Data.GeodatabaseFeature oneGeoDatabaseFeature in geoDatabaseFeature)
        //        {
        //            // Get the desired Field attribute values from the GeodatabaseFeature.
        //            System.Collections.Generic.IDictionary<string, object> attributes = oneGeoDatabaseFeature.Attributes;

        //            object postID = attributes["POST_ID"];

        //            // Construct a StringBuilder to hold the text from the Field attributes.
        //            System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
        //            stringBuilder.AppendLine("POST ID: " + postID.ToString());

        //            Messenger.Default.Send<NotificationMessage>(new NotificationMessage(stringBuilder.ToString()));
        //        }
        //    }
        //}

        async void AssociatedObject_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MapView mapView = sender as MapView;
            var screenPoint = e.GetPosition(mapView);

            // Convert the screen point to a point in map coordinates
            var mapPoint = mapView.ScreenToLocation(screenPoint);

            // Create a new IdentifyTask pointing to the map service to identify (USA)
            var uri = new Uri("http://sampleserver6.arcgisonline.com/arcgis/rest/services/USA/MapServer");
            var identifyTask = new IdentifyTask(uri);

            // Create variables to store identify parameter information  
            //--current map extent (Envelope) 
            var extent = mapView.Extent;
            //--tolerance, in pixels, for finding features  
            var tolerance = 7;
            //--current height, in pixels, of the map control               
            var height = (int)mapView.ActualHeight;
            //--current width, in pixels, of the map control
            var width = (int)mapView.ActualWidth;

            // Create a new IdentifyParameter; pass the variables above to the constructor
            var identifyParams = new Esri.ArcGISRuntime.Tasks.Query.IdentifyParameters(mapPoint, extent, tolerance, height, width);

            // Identify only the top most visible layer in the service
            identifyParams.LayerOption = LayerOption.Top;

            // Set the spatial reference to match with the map's
            identifyParams.SpatialReference = mapView.SpatialReference;

            // Execute the task and await the result
            IdentifyResult idResult = await identifyTask.ExecuteAsync(identifyParams);

            // See if a result was returned 
            if (idResult != null && idResult.Results.Count > 0)
            {
                // Get the feature for the first result
                var topLayerFeature = idResult.Results[0].Feature as Graphic;

                // do something
            }
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
