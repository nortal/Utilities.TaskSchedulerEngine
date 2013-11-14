using System;
using System.Text;

namespace Nortal.Utilities.TaskSchedulerEngine
{
	/// <summary>
	/// Describes a business task. Tasks implementing this interface are automatically discovered and executed by SchedulerEngine.
	/// </summary>
	public interface ISchedulerTask
	{
		/// <summary>
		/// If false then the task is NOT executed regardless of configured execution time or interval.
		/// </summary>
		Boolean IsEnabled { get; }

		/// <summary>
		/// Time of task execution -or- base date of execution times if non-zero Interval is specified
		/// </summary>
		DateTime ExecutionTime { get; }

		/// <summary>
		/// Interval between periodic task start times. Set to TimeSpan.Zero for non-periodic tasks.
		/// </summary>
		TimeSpan Interval { get; }

		/// <summary>
		/// Action to be triggered on scheduled times
		/// </summary>
		/// <param name="messagesToMainLog">result message to be included in JobScheduler task execution log entry.</param>
		void Run(StringBuilder messagesToMainLog);
	}
}
