using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;

using Esri.ArcGISRuntime.Controls;
using Esri.ArcGISRuntime.Geometry;
using System;

namespace Chapter6.ViewModels
{
    
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class CoordinateDisplayViewModel : ViewModelBase
    {
        private double latitude;
        private double longitude;

        private MapView mapView;
        /// <summary>
        /// Initializes a new instance of the CoordinateDisplayViewModel class.
        /// </summary>
        public CoordinateDisplayViewModel()
        {
            Messenger.Default.Register<Esri.ArcGISRuntime.Controls.MapView>(this, (mapView) =>
            {
                this.mapView = mapView;
                this.mapView.MouseMove += mapView_MouseMove;
            });
        }

        void mapView_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (this.mapView.Extent == null)
                return;
            System.Windows.Point screenPoint = e.GetPosition(this.mapView);
            MapPoint mapPoint = this.mapView.ScreenToLocation(screenPoint);
            if (this.mapView.WrapAround)
                mapPoint = GeometryEngine.NormalizeCentralMeridian(mapPoint) as MapPoint;

            this.Latitude = Math.Round(mapPoint.Y, 4);
            this.Longitude = Math.Round(mapPoint.X, 4);
        }
        public double Latitude
        {
            set 
            {
                this.latitude = value;
                RaisePropertyChanged("Latitude");
            }
            get { return this.latitude; }        
        }
        public double Longitude
        {
            set
            {
                this.longitude = value;
                RaisePropertyChanged("Longitude");
            }
            get { return this.longitude; } 
        }
    }
}