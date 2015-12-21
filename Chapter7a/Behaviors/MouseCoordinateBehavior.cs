using Esri.ArcGISRuntime.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows; 
using System.Windows.Interactivity;

using Esri.ArcGISRuntime.Geometry;

namespace Chapter7a.Behaviors
{
    public class MouseCoordinateBehavior : Behavior<MapView>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.MouseMove += AssociatedObject_MouseMove;
        }
        void AssociatedObject_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            MapView mapView = sender as MapView;

            System.Windows.Point screenPoint = e.GetPosition(mapView);
            MapPoint mapPoint = mapView.ScreenToLocation(screenPoint);
            if (mapView.WrapAround)
                mapPoint = GeometryEngine.NormalizeCentralMeridian(mapPoint) as MapPoint;

        }

    }
}
