namespace Nortal.Utilities.TaskSchedulerEngine
{
	public enum EngineLogEntryType: byte
	{
		EngineStarting,
		EngineStarted,
		ScheduleStarted,
		EngineStopRequested,
		EngineStopped,
		EngineError,
		ScheduleReport,
	}
}
