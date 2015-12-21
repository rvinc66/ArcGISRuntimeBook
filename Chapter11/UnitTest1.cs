using System;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Esri.ArcGISRuntime.Controls;
using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Layers;
using Esri.ArcGISRuntime.Tasks.Query;

namespace Chapter11
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public async Task GetRedlineCountAsync_CountFeatures_FiveFeaturesFound()
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

            //Assert.IsNotNull(map);
            //Assert.IsNotNull(results);
            Assert.AreEqual(5, results.Count);
        }
    }
}
