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

	This file is from project https://github.com/NortalLTD/Utilities.TaskSchedulerEngine, Nortal.Utilities.TaskSchedulerEngine, file 'SchedulerEngineBase.cs'.
*/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace Nortal.Utilities.TaskSchedulerEngine
{
	/// <summary>
	/// The core of task scheduling - finds tasks, builds schedule, executes tasks by schedule.
	/// </summary>
	public abstract class SchedulerEngineBase
	{
		public SchedulerEngineBase()
		{
			this.ResponsivenessInterval = TimeSpan.FromSeconds(5);
			this.Name = BuildDefaultName();
			this.Logger = new CsvSchedulerEngineLogger(); //default logger to write to console and file.
		}

		//main constuctor
		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="logger">If not set then initialized to CsvSchedulerEngineLogger</param>
		public SchedulerEngineBase(String name, ISchedulerEngineLogger logger)
			: this()
		{
			if (logger == null) { throw new ArgumentNullException("logger"); }
			if (name == null) { throw new ArgumentNullException("name"); }

			this.Name = name;
			this.Logger = logger;
		}


		private List<ScheduledTaskWrapper> Schedule = new List<ScheduledTaskWrapper>(10);
		protected Boolean IsRunning { get; set; }
		private Boolean _stopRequested;
		protected Boolean StopRequested
		{
			get { return _stopRequested; }
			set
			{
				if (value) { this.Logger.LogEngineMessage(this, EngineLogEntryType.EngineStopRequested, null); }
				_stopRequested = value;
			}
		}
		private DateTime LastScheduleReportOn { get; set; }

		public String Name { get; private set; }
		private ISchedulerEngineLogger logger;
		public ISchedulerEngineLogger Logger
		{
			get { return logger; }
			set
			{
				if (value == null) { throw new ArgumentNullException(); }
				this.logger = value;
			}
		}

		/// <summary>
		/// Configures how often the current schedule is reported to log.
		/// </summary>
		public TimeSpan? ScheduleReportInterval { get; set; }
		/// <summary>
		/// Configures how often the schedule and service stop requests are checked.
		/// NB! If set too high then service may appear unresponsive and exact execution time of schduled tasks may drift more.
		/// </summary>
		public TimeSpan ResponsivenessInterval { get; set; }

		private string BuildDefaultName()
		{
			String currentClassName = this.GetType().Name;
			String baseName = typeof(SchedulerEngineBase).Name;
			if (currentClassName.EndsWith(baseName)) { return currentClassName.Substring(0, currentClassName.Length - baseName.Length); }
			return currentClassName;
		}

		/// <summary>
		/// Initialize tasks to execute.
		/// </summary>
		/// <returns></returns>
		protected abstract IEnumerable<ISchedulerTask> InitializeTasks();

		/// <summary>
		/// Sets up service, resets tasks & their configuration.
		/// </summary>
		internal void Initialize()
		{
			this.Schedule.Clear();
			var tasksToAdd = InitializeTasks().ToList().ConvertAll(i => new ScheduledTaskWrapper(i));
			this.Schedule.AddRange(tasksToAdd);

			ReportSchedule();
		}

		/// <summary>
		/// Runs the engine and starts executing tasks by configured schedule. Does not return until RequestStop() is called by another thread.
		/// </summary>
		public void Run()
		{
			try
			{
				this.Logger.LogEngineMessage(this, EngineLogEntryType.EngineStarting, null);
				if (this.IsRunning) { throw new InvalidOperationException("Cannot run engine because engine instance is already running."); }
				IsRunning = true;
				StopRequested = false;

				Initialize();
				this.Logger.LogEngineMessage(this, EngineLogEntryType.EngineStarted, "Enabled tasks found: " + this.Schedule.Where(t => t.Task.IsEnabled).Count());

				while (!StopRequested)
				{
					MakeRound();
				}
				this.Logger.LogEngineMessage(this, EngineLogEntryType.EngineStopped, null);
			}
			catch (Exception exception)
			{
				this.Logger.LogEngineMessage(this, EngineLogEntryType.EngineError, exception.ToString());
				throw;
			}
			finally
			{
				IsRunning = false;
				StopRequested = false;
			}
		}

		/// <summary>
		/// Notifies the engine to stop running as soon as its safe to do so (finishes the task currently executing).
		/// </summary>
		public void RequestStop()
		{
			this.StopRequested = true;
		}

		/// <summary>
		/// Engine execution step - waits a configured time (pollInterval) or executes a task.
		/// </summary>
		private void MakeRound()
		{
			var scheduleInterval = this.ScheduleReportInterval;
			if (scheduleInterval != null && this.LastScheduleReportOn.Add(scheduleInterval.Value) < DateTime.Now) { ReportSchedule(); }

			var taskToRun = GetNextTaskToRun();
			if (taskToRun == null)
			{
				System.Threading.Thread.Sleep(this.ResponsivenessInterval);
				return;
			}

			Debug.Assert(taskToRun != null);
			Debug.Assert(taskToRun.NextPlannedRunTime != null);

			DateTime now = DateTime.Now;
			DateTime plannedTaskStart = taskToRun.NextPlannedRunTime.Value;

			if (now.Add(this.ResponsivenessInterval) <= plannedTaskStart)
			{
				//empty round, return earlier than plannedTaskStart to ensure service responsiveness.
				Thread.Sleep(this.ResponsivenessInterval);
				return;
			}

			//delay to fine-tune start without exiting MakeRound:
			while ((now = DateTime.Now) < plannedTaskStart) { Thread.Sleep((int)this.ResponsivenessInterval.TotalMilliseconds / 10); }

			StringBuilder messageToLog = new StringBuilder(200);
			DateTime taskStarted = now;
			try
			{
				this.Logger.LogTaskMessage(this, TaskLogEntryType.TaskStarting, taskToRun.Task, taskStarted, messageToLog.ToString());
				taskToRun.Run(messageToLog);
				this.Logger.LogTaskMessage(this, TaskLogEntryType.TaskFinished, taskToRun.Task, taskStarted, messageToLog.ToString());
			}
			catch (Exception exception)
			{
				messageToLog.AppendLine()
					.AppendLine("!!EXCEPTION!!")
					.AppendLine(exception.ToString())
					.AppendLine();
				this.Logger.LogTaskMessage(this, TaskLogEntryType.TaskError, taskToRun.Task, taskStarted, messageToLog.ToString());
				// swallow exception to allow service to continue working. All details are passed to logger.
			}
		}

		private ScheduledTaskWrapper GetNextTaskToRun()
		{
			ScheduledTaskWrapper nextTask = null;
			foreach (var nextTaskCandidate in this.Schedule)
			{
				if (nextTaskCandidate.NextPlannedRunTime == null) { continue; }
				if (nextTask == null || nextTaskCandidate.NextPlannedRunTime < nextTask.NextPlannedRunTime)
				{
					nextTask = nextTaskCandidate;
				}
			}
			return nextTask;
		}

		private void ReportSchedule()
		{
			StringBuilder messageToLog = new StringBuilder(400);
			messageToLog.AppendLine("Current schedule:");
			foreach (var scheduleItem in this.Schedule)
			{
				messageToLog.AppendLine(scheduleItem.ToStringWithNextRunTime());
			}
			this.Logger.LogEngineMessage(this, EngineLogEntryType.ScheduleReport, messageToLog.ToString());
			LastScheduleReportOn = DateTime.Now;
		}

		/// <summary>
		/// Starts to schedule and execute the tasks in background thread.
		/// </summary>
		/// <returns></returns>
		public Thread RunInSeparateThread()
		{
			var threadStarter = new ThreadStart(this.Run);
			var engineThread = new Thread(threadStarter);
			engineThread.Start();
			return engineThread;
		}






	}
}
