using System;
using System.Text;
using System.Threading;
using Nortal.Utilities.TaskSchedulerEngine;

namespace ExampleApplication.MyTasks
{

	public class AGoodTask : ISchedulerTask
	{
		#region ISchedulerTask Members

		public bool IsEnabled { get { return true; } }
		public DateTime ExecutionTime { get { return DateTime.Today; } }
		public TimeSpan Interval { get { return TimeSpan.FromSeconds(5); } }

		public void Run(StringBuilder messagesToMainLog)
		{
			Random random = new Random();
			Thread.Sleep(1000 + random.Next(3000));
			messagesToMainLog.Append("Updated " + random.Next(10) + "records, all fine.");
		}

		#endregion
	}
}
