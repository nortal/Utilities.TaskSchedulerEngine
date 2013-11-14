/*
	Copyright 2013 Imre Pühvel, AS Nortal
	
	Licensed under the Apache License, Version 2.0 (the "License");
	you may not use this file except in compliance with the License.
	You may obtain a copy of the License at

		http://www.apache.org/licenses/LICENSE-2.0

	Unless required by applicable law or agreed to in writing, software
	distributed under the License is distributed on an "AS IS" BASIS,
	WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	See the License for the specific language governing permissions and
	limitations under the License.

	This file is from project https://github.com/NortalLTD/Utilities.TaskSchedulerEngine, Nortal.Utilities.TaskSchedulerEngine, file 'ScheduledTaskWrapper.cs'.
*/
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
