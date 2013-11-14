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

	This file is from project https://github.com/NortalLTD/Utilities.TaskSchedulerEngine, Nortal.Utilities.TaskSchedulerEngine, file 'ISchedulerTask.cs'.
*/
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
