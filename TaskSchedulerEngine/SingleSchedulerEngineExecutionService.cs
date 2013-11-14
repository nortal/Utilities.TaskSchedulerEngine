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

	This file is from project https://github.com/NortalLTD/Utilities.TaskSchedulerEngine, Nortal.Utilities.TaskSchedulerEngine, file 'SingleSchedulerEngineExecutionService.cs'.
*/
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