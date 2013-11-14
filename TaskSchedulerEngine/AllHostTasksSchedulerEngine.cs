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

	This file is from project https://github.com/NortalLTD/Utilities.TaskSchedulerEngine, Nortal.Utilities.TaskSchedulerEngine, file 'AllHostTasksSchedulerEngine.cs'.
*/
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Nortal.Utilities.TaskSchedulerEngine
{
	/// <summary>
	/// Task scheduler engine which automatically discovers and executes tasks from entrypoint assembly.
	/// </summary>
	public class AllHostTasksSchedulerEngine : SchedulerEngineBase
	{
		protected override IEnumerable<ISchedulerTask> InitializeTasks()
		{
			var assemblyTasks = LoadInterfaceImplementations<ISchedulerTask>(Assembly.GetEntryAssembly());
			foreach (var assemblyTask in assemblyTasks)
			{
				yield return assemblyTask;
			}
		}

		/// <summary>
		/// Loads classes that implement given interface
		/// </summary>
		internal static IEnumerable<T> LoadInterfaceImplementations<T>(Assembly assembly)
		{
			if (assembly == null) { throw new ArgumentNullException("assembly"); }

			Type interfaceType = typeof(T);
			foreach (var type in assembly.GetTypes())
			{
				if (type.IsInterface || !interfaceType.IsAssignableFrom(type)) { continue; }
				var matchingTask = (T)Activator.CreateInstance(type);
				yield return matchingTask;
			}
		}

	}
}
