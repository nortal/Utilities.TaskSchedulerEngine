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

	This file is from project https://github.com/NortalLTD/Utilities.TaskSchedulerEngine, Nortal.Utilities.TaskSchedulerEngine, file 'SchedulerEngineExecutionServiceBase.cs'.
*/
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
