using System.Collections.Generic;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;

using Esri.ArcGISRuntime.Tasks.Query;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Controls;

using Chapter7.Services;
using Chapter7.Models;
using Esri.ArcGISRuntime.Layers;
using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Symbology;
using System.Windows.Media;
using System;

namespace Chapter7.ViewModels
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
    public class MainViewModel : ViewModelBase
    {
        private Model myModel = null;
        private MapView mapView;
        private ObservableCollection<FindItem> listResults;
        public RelayCommand<int> SearchRelayCommand { get; private set; }
        GraphicsLayer graphicsLayerCity = null;
        GraphicsLayer graphicsLayerState = null;
        GraphicsLayer graphicsLayerCounty = null;

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

                this.SearchRelayCommand = new RelayCommand<int>(Search);


                Messenger.Default.Register<Esri.ArcGISRuntime.Controls.MapView>(this, (mapView) =>
                {
                    this.mapView = mapView;


                    Uri uriBasemap = new Uri(this.BasemapUri);
                    ArcGISTiledMapServiceLayer basemapLayer = new ArcGISTiledMapServiceLayer(uriBasemap);
                    basemapLayer.InitializeAsync();
                    this.mapView.Map.Layers.Add(basemapLayer);

                    this.graphicsLayerCity = new Esri.ArcGISRuntime.Layers.GraphicsLayer();
                    this.graphicsLayerCity.ID = "City Results";
                    this.graphicsLayerCity.InitializeAsync();

                    this.graphicsLayerCounty = new Esri.ArcGISRuntime.Layers.GraphicsLayer();
                    this.graphicsLayerCounty.ID = "County Results";
                    this.graphicsLayerCounty.InitializeAsync();

                    this.graphicsLayerState = new Esri.ArcGISRuntime.Layers.GraphicsLayer();
                    this.graphicsLayerState.ID = "State Results";
                    this.graphicsLayerState.InitializeAsync();

                    this.mapView.Map.Layers.Add(this.graphicsLayerCity);
                    this.mapView.Map.Layers.Add(this.graphicsLayerState);
                    this.mapView.Map.Layers.Add(this.graphicsLayerCounty);
                    
                    

                   
                });     
            }
        }

        public async void Search(int wkid)
        {

            // Create the symbol
            SimpleMarkerSymbol markerSymbol = new SimpleMarkerSymbol();
            markerSymbol.Size = 15;
            markerSymbol.Color = Colors.Green;
            markerSymbol.Style = SimpleMarkerStyle.Diamond;

            SimpleFillSymbol sfsState = new SimpleFillSymbol()
            {
                Color = Colors.Red,
                Style = SimpleFillStyle.Solid
            };
            SimpleFillSymbol sfsCounty = new SimpleFillSymbol()
            {
                Color = Colors.Red,
                Style = SimpleFillStyle.Solid
            };

            SpatialReference sr = new SpatialReference(wkid);

            Query queryCity = new Query("areaname = '" + this.SearchText + "'");
            queryCity.OutSpatialReference = sr;
            queryCity.OutFields.Add("*");
            QueryTask queryTaskCity = new QueryTask(new System.Uri(this.USAUri + "/0"));
            QueryResult queryResultCity = await queryTaskCity.ExecuteAsync(queryCity);

            Query queryStates = new Query("state_name = '" + this.SearchText + "'");
            queryStates.OutSpatialReference = sr;
            queryStates.OutFields.Add("*");
            QueryTask queryTaskStates = new QueryTask(new System.Uri(this.USAUri + "/2"));
            QueryResult queryResultStates = await queryTaskStates.ExecuteAsync(queryStates);

            Query queryCounties = new Query("name = '" + this.SearchText + "'");
            queryCounties.OutSpatialReference = sr;
            queryCounties.OutFields.Add("*");
            QueryTask queryTaskCounties = new QueryTask(new System.Uri(this.USAUri + "/3"));
            QueryResult queryResultCounties= await queryTaskCounties.ExecuteAsync(queryCounties);

            // Get the list of features (graphics) from the result
            IReadOnlyList<Feature> featuresCity = queryResultCity.FeatureSet.Features;
            foreach (Feature featureCity in featuresCity)
            {
                Graphic graphicCity = (Graphic)featureCity;
                graphicCity.Symbol = markerSymbol;
                this.graphicsLayerCity.Graphics.Add(graphicCity);
            }

            // Get the list of features (graphics) from the result
            IReadOnlyList<Feature> featuresStates = queryResultStates.FeatureSet.Features;
            foreach (Feature featureState in featuresStates)
            {
                Graphic graphicState = (Graphic)featureState;
                graphicState.Symbol = sfsState;
                this.graphicsLayerState.Graphics.Add(graphicState);
            }
            // Get the list of features (graphics) from the result
            IReadOnlyList<Feature> featuresCounties = queryResultCounties.FeatureSet.Features;
            foreach (Feature featureCounty in featuresCounties)
            {
                Graphic graphicCounty = (Graphic)featureCounty;
                graphicCounty.Symbol = sfsCounty;
                this.graphicsLayerCounty.Graphics.Add(graphicCounty);
            }

                      
        }

        public ObservableCollection<FindItem> GridDataResults
        {
            get { return this.listResults; }
            set
            {
                this.listResults = value;
                RaisePropertyChanged("GridDataResults");
            }
        }
        public string SearchText
        {
            get { return this.myModel.SearchText; }
            set
            {
                this.myModel.SearchText = value;
                RaisePropertyChanged("SearchText");
            }
        }
        public string BasemapUri
        {
            get { return myModel.BasemapLayerUri; }
            set
            {
                this.myModel.BasemapLayerUri = value;
                RaisePropertyChanged("BasemapUri");
            }
        }
        public string USAUri
        {
            get { return myModel.USALayerUri; }
            set
            {
                this.myModel.USALayerUri = value;
                RaisePropertyChanged("USAUri");
            }
        }

    }
}