using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Diagnostics;
using System.Collections.Generic;
using Esri.ArcGISRuntime.Controls;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.Layers;

namespace Chapter11c
{
    public partial class MainWindow : Window
    {
        Random random = null;

        public MainWindow()
        {
            InitializeComponent();

            //=====================================================
            // Add graphics individually
            //=====================================================
            // get the graphics layer
            GraphicsLayer graphicsLayer = MyMapView.Map.Layers["graphicsLayer"] as GraphicsLayer;
            this.random = new Random();

            // create a stopwatch
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            // create a bunch of graphics
            for (int i = 0; i < 50000; i++)
            {
                int latitude = random.Next(-90, 90);
                int longitude = random.Next(-180, 180);
                MapPoint mapPoint = new MapPoint(longitude, latitude);

                SimpleMarkerSymbol sms = new SimpleMarkerSymbol();
                sms = new Esri.ArcGISRuntime.Symbology.SimpleMarkerSymbol();
                sms.Color = System.Windows.Media.Colors.Red;
                sms.Style = Esri.ArcGISRuntime.Symbology.SimpleMarkerStyle.Circle;
                sms.Size = 2;
                Graphic graphic = new Graphic(mapPoint, sms);
                graphicsLayer.Graphics.Add(graphic);
            }
            // stop timing 
            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;

            Console.WriteLine("Time elapsed: {0}", stopWatch.Elapsed);


        //////=======================================================
        //// Simple Renderer
        ////========================================================
        //// get the graphics layer
        //GraphicsLayer graphicsLayer = MyMapView.Map.Layers["graphicsLayer"] as GraphicsLayer;
        //this.random = new Random();

        //// create a stopwatch
        //Stopwatch stopWatch = new Stopwatch();
        //stopWatch.Start();

        //SimpleMarkerSymbol sms = new SimpleMarkerSymbol();
        //sms = new Esri.ArcGISRuntime.Symbology.SimpleMarkerSymbol();
        //sms.Color = System.Windows.Media.Colors.Red;
        //sms.Style = Esri.ArcGISRuntime.Symbology.SimpleMarkerStyle.Circle;
        //sms.Size = 2;

        //SimpleRenderer simpleRenderer = new SimpleRenderer();
        //simpleRenderer.Symbol = sms;

        //// create a enerable list
        //List<Graphic> graphics = new List<Graphic>(50001);
        //// create a bunch of graphics
        //for (int i = 0; i < 50000; i++)
        //{
        //    int latitude = random.Next(-90, 90);
        //    int longitude = random.Next(-180, 180);
        //    MapPoint mapPoint = new MapPoint(longitude, latitude);
        //    Graphic graphic = new Graphic(mapPoint);
        //    graphics.Add(graphic);
        //}
        //graphicsLayer.Renderer = simpleRenderer;
        //graphicsLayer.GraphicsSource = graphics; ;

        //// stop timing 
        //stopWatch.Stop();
        //TimeSpan ts = stopWatch.Elapsed;

        //Console.WriteLine("Time elapsed: {0}", stopWatch.Elapsed);

            ////=======================================================
            //// Add using GraphicsSource
            ////========================================================
            //// get the graphics layer
            //GraphicsLayer graphicsLayer = MyMapView.Map.Layers["graphicsLayer"] as GraphicsLayer;
            //this.random = new Random();

            //// create a stopwatch
            //Stopwatch stopWatch = new Stopwatch();
            //stopWatch.Start();

            //// create a enerable list
            //List<Graphic> graphics = new List<Graphic>(50001);
            //// create a bunch of graphics
            //for (int i = 0; i < 50000; i++)
            //{
            //    int latitude = random.Next(-90, 90);
            //    int longitude = random.Next(-180, 180);
            //    MapPoint mapPoint = new MapPoint(longitude, latitude);

            //    SimpleMarkerSymbol sms = new SimpleMarkerSymbol();
            //    sms = new Esri.ArcGISRuntime.Symbology.SimpleMarkerSymbol();
            //    sms.Color = System.Windows.Media.Colors.Red;
            //    sms.Style = Esri.ArcGISRuntime.Symbology.SimpleMarkerStyle.Circle;
            //    sms.Size = 2;
            //    Graphic graphic = new Graphic(mapPoint, sms);

            //    graphics.Add(graphic);
            //}
            //graphicsLayer.GraphicsSource = graphics; ;

            //// stop timing 
            //stopWatch.Stop();
            //TimeSpan ts = stopWatch.Elapsed;

            //Console.WriteLine("Time elapsed: {0}", stopWatch.Elapsed);



        }

        private void MyMapView_LayerLoaded(object sender, LayerLoadedEventArgs e)
        {
            if (e.LoadError == null)
                return;

            Debug.WriteLine(string.Format("Error while loading layer : {0} - {1}", e.Layer.ID, e.LoadError.Message));
        }
    }
}
