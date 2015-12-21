using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace Chapter11b
{
	public static class ThreadHelper
	{
		static CoreDispatcher dispatcher;

		public static void Initialize(CoreDispatcher dispather)
		{
			ThreadHelper.dispatcher = dispather;
		}

		public static Task Run(Action action)
		{
			if (dispatcher == null)
				throw new InvalidOperationException("You must initialize ThreadHelper before calling Run()");

			var tcs = new TaskCompletionSource<bool>();
			var ignore = dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
			{
				try
				{
					action();
					tcs.TrySetResult(true);
				}
				catch (Exception ex)
				{
					tcs.TrySetException(ex);
				}
			});
			return tcs.Task;
		}

		public static Task Run(Task task)
		{
			return Run(() => task);
		}

		public static Task Run(Func<Task> taskFunc)
		{
			if (dispatcher == null)
				throw new InvalidOperationException("You must initialize ThreadHelper before calling Run()");

			var tcs = new TaskCompletionSource<bool>();
			var ignore = dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
			{
				try
				{
					await taskFunc();
					tcs.TrySetResult(true);
				}
				catch (Exception ex)
				{
					tcs.TrySetException(ex);
				}
			});
			return tcs.Task;
		}
	}
}
