using System;
using System.Collections.Generic;

namespace Nortal.Utilities.TaskSchedulerEngine
{

	public class SingleSchedulerEngineExecutionService<TEngine> : SchedulerEngineExecutionServiceBase
		where TEngine : SchedulerEngineBase, new()
	{

		public SingleSchedulerEngineExecutionService(String serviceName)
			: base(serviceName)
		{
		}

		public SingleSchedulerEngineExecutionService(String serviceName, ISchedulerEngineLogger logger)
			: this(serviceName)
		{
			if (logger == null) { throw new ArgumentNullException("logger"); }
			this.DefaultLogger = logger;
		}

		public ISchedulerEngineLogger DefaultLogger { get; protected set; }

		protected override IEnumerable<SchedulerEngineBase> InitializeSchedulerEngines()
		{
			var engine = new TEngine();
			if (this.DefaultLogger != null) { engine.Logger = this.DefaultLogger; }
			yield return engine;
		}
	}
}