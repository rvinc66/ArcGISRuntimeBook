using Esri.ArcGISRuntime.Controls;
using GalaSoft.MvvmLight.Messaging; //added
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows; //added
using System.Windows.Interactivity;

namespace Chapter3a.Behaviors
{
    public class MapViewBehavior : Behavior<MapView>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Loaded += AssociatedObject_Loaded;
        }
        void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            MapView mapView = sender as MapView;
            Messenger.Default.Send<MapView>(mapView);
        }
    }
}
