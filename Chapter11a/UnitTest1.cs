using System;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.ObjectModel;
using System.Threading;

using Esri.ArcGISRuntime.Controls;
using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Layers;
using Esri.ArcGISRuntime.Tasks.Query;

using GalaSoft.MvvmLight.Messaging;

using Chapter4a.ViewModels;

namespace Chapter11a
{
    [TestClass]
    public class UnitTest1
    {
        private ViewModelLocator locator = null;

        [TestInitialize]
        public void Setup()
        {
            // provide a SynchronizationContext so that Sync/Async calls can be made
            //SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
        }

        [TestMethod]
        public  void GetSearchCount_CountFeatures_NineFeatureFound()
        {
            // instantiate the locator so that our view model is created
            locator = new ViewModelLocator();
            // get the MainViewModel
            MainViewModel mainViewModel = locator.MainViewModel;
            // create a MapView
            MapView mapView = new MapView();
            // send the MapView to the MainViewModel
            Messenger.Default.Send<MapView>(mapView);
            // search string
            mainViewModel.SearchText = "Lancaster";
            
            // Act. search for the search text
            mainViewModel.Search(4326);

            Thread.Sleep(6000);

            Assert.IsNotNull(mainViewModel, "Null mainViewModel");
            Assert.IsNotNull(mainViewModel.GridDataResults, "Null GridDataResults");
            Assert.IsNotNull(mainViewModel.GridDataResults.Count, "Null Count");
            Assert.AreEqual(9, mainViewModel.GridDataResults.Count);
            
        }
    }
}
