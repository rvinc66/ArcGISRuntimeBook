using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;

using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Controls;

namespace Chapter4a.ViewModels
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class CameraViewModel : ViewModelBase
    {
        Esri.ArcGISRuntime.Controls.SceneView sceneView = null;
        double heading = 0.0;
        double pitch  = 0.0;
        double elevation = 11000000;
        double latitude = 0.0;
        double longitude = 0.0;
        /// <summary>
        /// Initializes a new instance of the LocationViewModel class.
        /// </summary>
        public CameraViewModel()
        {
            Messenger.Default.Register<SceneView>(this, (sceneView) =>
            {
                this.sceneView = sceneView;
            });
        }

        public double Heading
        {
            get { return this.heading; }
            set
            {
                this.heading = value;
                RaisePropertyChanged("Heading");

                MapPoint myLocation = new MapPoint(this.longitude, this.latitude, this.elevation, SpatialReferences.Wgs84);
                Camera myCamera = new Camera(myLocation, value, this.pitch);
                this.sceneView.SetView(myCamera);
            }
        }
        public double Pitch
        {
            get { return this.pitch; }
            set
            {
                this.pitch = value;
                RaisePropertyChanged("Pitch");

                MapPoint myLocation = new MapPoint(this.longitude, this.latitude, this.elevation, SpatialReferences.Wgs84);
                Camera myCamera = new Camera(myLocation, this.heading, value);
                this.sceneView.SetView(myCamera);
            }
        }
        public double Elevation
        {
            get { return this.elevation; }
            set
            {
                this.elevation = value;
                RaisePropertyChanged("Elevation");

                MapPoint myLocation = new MapPoint(this.longitude, this.latitude, value, SpatialReferences.Wgs84);
                Camera myCamera = new Camera(myLocation, this.heading, this.pitch);
                this.sceneView.SetView(myCamera);
            }
        }
        public double Latitude
        {
            get { return this.latitude; }
            set
            {
                this.latitude = value;
                RaisePropertyChanged("Latitude");

                MapPoint myLocation = new MapPoint(this.longitude, value, this.elevation, SpatialReferences.Wgs84);
                Camera myCamera = new Camera(myLocation, this.heading, this.pitch);
                this.sceneView.SetView(myCamera);
            }
        }
        public double Longitude
        {
            get { return this.longitude; }
            set
            {
                this.longitude = value;
                RaisePropertyChanged("Longitude");

                MapPoint myLocation = new MapPoint(value, this.latitude, this.elevation, SpatialReferences.Wgs84);
                Camera myCamera = new Camera(myLocation, this.heading, this.pitch);
                this.sceneView.SetView(myCamera);
            }
        }
    }
}