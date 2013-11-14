using System;
using System.Collections.Generic;
using System.ServiceProcess;
using System.Threading;

namespace Nortal.Utilities.TaskSchedulerEngine
{

	public abstract class SchedulerEngineExecutionServiceBase : ServiceBase
	{
		public SchedulerEngineExecutionServiceBase(String serviceName)
			: this()
		{
			this.ServiceName = serviceName;
		}

		public SchedulerEngineExecutionServiceBase()
		{
			this.SchedulerEngines = new List<SchedulerEngineBase>();
			this.EngineThreads = new List<Thread>();
		}

		public IList<SchedulerEngineBase> SchedulerEngines { get; private set; }
		protected List<Thread> EngineThreads { get; private set; }

		protected abstract IEnumerable<SchedulerEngineBase> InitializeSchedulerEngines();

		public void Start()
		{
			this.OnStart(new String[] { });
		}

		protected override void OnStart(String[] args)
		{
			base.OnStart(args);

			foreach (var engine in InitializeSchedulerEngines())
			{
				if (engine == null) { throw new InvalidOperationException("Uninitialized SchedulerEngine object passed for execution."); }
				this.SchedulerEngines.Add(engine);
				var thread = engine.RunInSeparateThread();
				this.EngineThreads.Add(thread);
			}
		}

		protected override void OnStop()
		{
			base.OnStop();

			foreach (var engine in this.SchedulerEngines)
			{
				engine.RequestStop();
			}

			foreach (var thread in this.EngineThreads)
			{
				thread.Join();
			}
		}
	}
}
