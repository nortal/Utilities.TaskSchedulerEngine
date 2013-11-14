using System;
using System.Text;

namespace Nortal.Utilities.TaskSchedulerEngine
{
	/// <summary>
	/// Task as stored internally by SchedulerEngine.
	/// </summary>
	internal class ScheduledTaskWrapper
	{
		public ScheduledTaskWrapper(ISchedulerTask businessTask)
		{
			if (businessTask == null) { throw new ArgumentNullException("businessTask"); }
			this.Task = businessTask;
			this.NextPlannedRunTime = CalculateNextRunTime();
		}

		public ISchedulerTask Task { get; private set; }
		public DateTime? NextPlannedRunTime { get; private set; }

		internal void Run(StringBuilder messagesToMainLog)
		{
			try
			{
				this.Task.Run(messagesToMainLog);
			}
			finally
			{
				NextPlannedRunTime = CalculateNextRunTime(lastPlannedRuntime: NextPlannedRunTime);
			}
		}

		private DateTime? CalculateNextRunTime(DateTime? lastPlannedRuntime = null)
		{
			if (!this.Task.IsEnabled) { return null; }

			DateTime now = DateTime.Now;
			if (this.Task.ExecutionTime > now) { return this.Task.ExecutionTime; }

			if (this.Task.Interval == TimeSpan.Zero) { return null; }

			if (lastPlannedRuntime != null)
			{
				return lastPlannedRuntime.Value.Add(this.Task.Interval);
			}
			else
			{
				DateTime nextTimeCandidate = this.Task.ExecutionTime;
				while (nextTimeCandidate < now) { nextTimeCandidate = nextTimeCandidate.Add(this.Task.Interval); }
				return nextTimeCandidate;
			}
		}

		public override string ToString()
		{
			return this.Task.GetType().Name;
		}

		internal string ToStringWithNextRunTime()
		{
			return String.Format("{0} (to run: {1})", this, NextPlannedRunTime);
		}
	}
}
