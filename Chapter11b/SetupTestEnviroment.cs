using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;

namespace Chapter11b
{
	[TestClass]
	public class SetupTestEnviroment
	{
		[AssemblyInitialize()]
		public static void SetupTests(TestContext context)
		{
			var dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;
			ThreadHelper.Initialize(dispatcher);
		}
	}
}