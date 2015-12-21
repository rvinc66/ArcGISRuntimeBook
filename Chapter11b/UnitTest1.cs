using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

using Esri.ArcGISRuntime.Controls;
using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Layers;
using Esri.ArcGISRuntime.Tasks.Query;

using Windows.Foundation;

namespace Chapter11b
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public async Task GetRedlineCountAsync_CountFeatures_FiveFeaturesFound()
        {
            await ThreadHelper.Run(async () =>
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
                System.Diagnostics.Debug.WriteLine("Results count: " + results.Count.ToString());
            });
        }
    }
}