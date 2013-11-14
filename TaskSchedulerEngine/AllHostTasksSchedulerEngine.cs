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
