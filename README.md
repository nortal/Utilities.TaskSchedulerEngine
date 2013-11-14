Nortal.Utilities.TaskSchedulerEngine
====================================

This assembly provides tools to build a task scheduler engine for triggering prepared tasks at specified time and/or intervals. Tools are provided to support hosting as a windows service.

Requirements:
 * Microsoft .Net Framework 4 Client Profile.

Main features
-------------
* Can set tasks execution time
* Can configure tasks to automatically reschedule itself to run after fixed interval.
* Task execution logs
* Tools to simplify running as a windows service
* tools for graceful engine stopping without interrupting ongoing tasks
* option to automatically discover all tasks from host assembly
* Example project to help getting started
* It is with open source (Apache v2.0 licence)

Getting started
-------------------

* Create a console application project
* Reference Nortal.Utilities.TaskSchedulerEngine
* Implement tasks by using ISchedulerTask interface:

		public class MyTask : ISchedulerTask
		{
			public bool IsEnabled { get { return true; } }
			public DateTime ExecutionTime { get { return DateTime.Now; } }
			public TimeSpan Interval { get { return TimeSpan.FromSeconds(5); } }
		
			public void Run(StringBuilder log)
			{
				log.Append("Hello, world!");
			}
		}

* run the scheduler engine to start executing the prepared task. 

		// service that contains a single engine which automatically discovers ISchedulerTask implementations
		var engine = new AllHostTasksSchedulerEngine();
		engine.Run();

* All done!

Check the example application included with source code for other examples.

Extension points
------------------
* Can configure engine ticking interval
* Task configuration entirely left up to programmer. You can hardcode values, user xml configuration, database or any other configuration source.
* Option to replace the default logging (CSV files) by implementing your own logger using ISchedulerEngineLogger interface.
