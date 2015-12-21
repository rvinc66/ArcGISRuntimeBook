using System;
using System.Windows;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Esri.ArcGISRuntime.Controls;
using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Layers;
using Esri.ArcGISRuntime.Tasks.Query;



namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        //public IAsyncAction ExecuteOnUIThread(Windows.UI.Core.DispatchedHandler action)
        //{
        //    return Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, action);
        //}


        [TestMethod]
        public async Task GeteRedlineCountAsync_CountFeatures_FiveFeaturesFound()
        {

            QueryCountResult results = null;
            Map map = null;

            var table = new ServiceFeatureTable(
                new Uri("http://sampleserver6.arcgisonline.com/arcgis/rest/services/Water_Network/FeatureServer/2"));

            await table.InitializeAsync();
            map = new Map();
            map.Layers.Add(new FeatureLayer(table) { ID = "test" });
            

            var queryTask = new QueryTask(new Uri(table.ServiceUri));
            results = await queryTask.ExecuteCountAsync(new Query("1=1"));

            Assert.IsNotNull(map);
            Assert.IsNotNull(results);
            Assert.AreEqual(5, results.Count);
        }

    }
}
