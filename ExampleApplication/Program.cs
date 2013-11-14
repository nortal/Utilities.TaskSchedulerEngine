using System;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading;
using Nortal.Utilities.TaskSchedulerEngine;

namespace ExampleApplication
{
	class Program
	{
		static void Main(string[] args)
		{
			try
			{
				// service that contains a single engine which automatically discovers ISchedulerTask implementations
				var service = new SingleSchedulerEngineExecutionService<AllHostTasksSchedulerEngine>();
				
				// to manually control included ISchedulerTasks, create your own engine by inheriting from SchedulerEngine.
				// to have more engines running concurrently, create your own scheduler engine executor by inheriting from SchedulerEngineExecutionServiceBase

				//runnning the service as any other windows service:
				if (Environment.UserInteractive)
				{
					Console.WriteLine("Starting ExampleService in command-line mode ..");
					Console.WriteLine("Press CTRL-C to terminate ..");

					service.Start();
					Console.ReadLine();
					service.Stop();
				}
				else
				{
					var ServicesToRun = new ServiceBase[] { service };
					ServiceBase.Run(ServicesToRun);
				}
			}
			catch (Exception exception)
			{
				Debugger.Break();
			}
		}

	}
}
