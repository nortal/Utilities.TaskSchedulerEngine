using System;

namespace Nortal.Utilities.TaskSchedulerEngine
{
	public interface ISchedulerEngineLogger
	{
		void LogEngineMessage(SchedulerEngineBase engine, EngineLogEntryType type, String additionalInfo);
		void LogTaskMessage(SchedulerEngineBase engine, TaskLogEntryType type, ISchedulerTask task, DateTime taskStartedOn, String additionalInfo);
	}
}
