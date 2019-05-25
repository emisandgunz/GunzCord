using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace GunzCord.Application
{
	public static class ApplicationExtensions
	{
		/// <summary>
		/// Attempts to gracefully stop the application with the given timeout.
		/// </summary>
		/// <param name="application"></param>
		/// <param name="timeout">The timeout for stopping gracefully. Once expired the
		/// server may terminate any remaining active connections.</param>
		/// <returns></returns>
		public static Task StopAsync(this IApplication application, TimeSpan timeout)
		{
			return application.StopAsync(new CancellationTokenSource(timeout).Token);
		}

		/// <summary>
		/// Block the calling thread until shutdown is triggered via Ctrl+C or SIGTERM.
		/// </summary>
		/// <param name="application">The running <see cref="IApplication"/>.</param>
		public static void WaitForShutdown(this IApplication application)
		{
			application.WaitForShutdownAsync().GetAwaiter().GetResult();
		}

		/// <summary>
		/// Returns a Task that completes when shutdown is triggered via the given token, Ctrl+C or SIGTERM.
		/// </summary>
		/// <param name="application">The running <see cref="IApplication"/>.</param>
		/// <param name="token">The token to trigger shutdown.</param>
		public static async Task WaitForShutdownAsync(this IApplication application, CancellationToken token = default)
		{
			var done = new ManualResetEventSlim(false);
			using (var cts = CancellationTokenSource.CreateLinkedTokenSource(token))
			{
				AttachCtrlcSigtermShutdown(cts, done, shutdownMessage: string.Empty);

				try
				{
					await application.WaitForTokenShutdownAsync(cts.Token);
				}
				finally
				{
					done.Set();
				}
			}
		}

		/// <summary>
		/// Runs a web application and block the calling thread until host shutdown.
		/// </summary>
		/// <param name="application">The <see cref="IApplication"/> to run.</param>
		public static void Run(this IApplication application)
		{
			application.RunAsync().GetAwaiter().GetResult();
		}

		/// <summary>
		/// Runs a web application and returns a Task that only completes when the token is triggered or shutdown is triggered.
		/// </summary>
		/// <param name="application">The <see cref="IApplication"/> to run.</param>
		/// <param name="token">The token to trigger shutdown.</param>
		public static async Task RunAsync(this IApplication application, CancellationToken token = default)
		{
			// Wait for token shutdown if it can be canceled
			if (token.CanBeCanceled)
			{
				await application.RunAsync(token, shutdownMessage: null);
				return;
			}

			// If token cannot be canceled, attach Ctrl+C and SIGTERM shutdown
			var done = new ManualResetEventSlim(false);
			using (var cts = new CancellationTokenSource())
			{
				var shutdownMessage = "Application is shutting down...";
				AttachCtrlcSigtermShutdown(cts, done, shutdownMessage: shutdownMessage);

				try
				{
					await application.RunAsync(cts.Token, "Application started. Press Ctrl+C to shut down.");
				}
				finally
				{
					done.Set();
				}
			}
		}

		private static async Task RunAsync(this IApplication application, CancellationToken token, string shutdownMessage)
		{
			using (application)
			{
				await application.StartAsync(token);

				if (!string.IsNullOrEmpty(shutdownMessage))
				{
					Console.WriteLine(shutdownMessage);
				}

				await application.WaitForTokenShutdownAsync(token);
			}
		}

		private static void AttachCtrlcSigtermShutdown(CancellationTokenSource cts, ManualResetEventSlim resetEvent, string shutdownMessage)
		{
			void Shutdown()
			{
				if (!cts.IsCancellationRequested)
				{
					if (!string.IsNullOrEmpty(shutdownMessage))
					{
						Console.WriteLine(shutdownMessage);
					}
					try
					{
						cts.Cancel();
					}
					catch (ObjectDisposedException) { }
				}

				// Wait on the given reset event
				resetEvent.Wait();
			};

			AppDomain.CurrentDomain.ProcessExit += (sender, eventArgs) => Shutdown();
			Console.CancelKeyPress += (sender, eventArgs) =>
			{
				Shutdown();
				// Don't terminate the process immediately, wait for the Main thread to exit gracefully.
				eventArgs.Cancel = true;
			};
		}

		private static async Task WaitForTokenShutdownAsync(this IApplication application, CancellationToken token)
		{
			var applicationLifetime = application.Services.GetService<IApplicationLifetime>();

			token.Register(state =>
			{
				((IApplicationLifetime)state).StopApplication();
			},
			applicationLifetime);

			var waitForStop = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);
			applicationLifetime.ApplicationStopping.Register(obj =>
			{
				var tcs = (TaskCompletionSource<object>)obj;
				tcs.TrySetResult(null);
			}, waitForStop);

			await waitForStop.Task;

			// WebHost will use its default ShutdownTimeout if none is specified.
			await application.StopAsync();
		}
	}
}
