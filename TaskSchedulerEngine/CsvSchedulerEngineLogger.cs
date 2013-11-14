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

	This file is from project https://github.com/NortalLTD/Utilities.TaskSchedulerEngine, Nortal.Utilities.TaskSchedulerEngine, file 'CsvSchedulerEngineLogger.cs'.
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Nortal.Utilities.TaskSchedulerEngine
{
	public class CsvSchedulerEngineLogger : ISchedulerEngineLogger
	{
		public CsvSchedulerEngineLogger()
		{
			this.CsvFieldSeparator = ',';
			this.CsvFieldSeparatorTrailingWhiteSpace = " ";
			this.CsvLineSeparator = Environment.NewLine;
			this.CsvValueWrapper = '"';

			PrepareDefaultPaths();
		}

		private void PrepareDefaultPaths()
		{
			String localDirectory = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.FullName;
			this.EngineLogFile = new FileInfo(Path.Combine(localDirectory, "TaskScheduler.Engine.log"));
			this.TaskLogFile = new FileInfo(Path.Combine(localDirectory, "TaskScheduler.TaskExecutions.log"));
		}

		private readonly char CsvValueWrapper;
		private readonly char CsvFieldSeparator;
		private readonly String CsvFieldSeparatorTrailingWhiteSpace;
		private readonly String CsvLineSeparator;

		public FileInfo EngineLogFile { get; set; }
		public FileInfo TaskLogFile { get; set; }

		#region ISchedulerEngineLogger Members

		protected virtual Boolean ShouldLog(EngineLogEntryType type)
		{
			return true;
		}

		protected virtual IEnumerable<String> BuildLogEntryFields(SchedulerEngineBase engine, EngineLogEntryType type, String additionalInfo)
		{
			yield return DateTime.Now.ToString("s");
			yield return engine.Name;
			yield return type.ToString();
			yield return WrapAsCsvValue(additionalInfo);
		}

		public void LogEngineMessage(SchedulerEngineBase engine, EngineLogEntryType type, String additionalInfo)
		{
			if (!this.ShouldLog(type)) { return; }
			var values = BuildLogEntryFields(engine, type, additionalInfo);
			var entry = String.Join(", ", values);

			WriteSchedulerMessage(type, PolishMessage(entry));
		}


		protected virtual Boolean ShouldLog(TaskLogEntryType type)
		{
			return type != TaskLogEntryType.TaskStarting;
		}

		protected virtual IEnumerable<String> BuildLogEntryFields(SchedulerEngineBase engine, TaskLogEntryType type, ISchedulerTask task, DateTime taskStartedOn, String additionalInfo)
		{
			yield return DateTime.Now.ToString("s");
			yield return engine.Name;
			yield return type.ToString();
			yield return task.GetType().Name;
			yield return (DateTime.Now - taskStartedOn).ToString();
			yield return WrapAsCsvValue(additionalInfo);
		}

		public void LogTaskMessage(SchedulerEngineBase engine, TaskLogEntryType type, ISchedulerTask task, DateTime taskStartedOn, String additionalInfo)
		{
			if (!this.ShouldLog(type)) { return; }
			var values = BuildLogEntryFields(engine, type, task, taskStartedOn, additionalInfo);
			var entry = String.Join(", ", values);
			WriteSchedulerMessage(type, PolishMessage(entry));
		}

		#endregion

		private Object EngineLogLock = new Object();
		protected virtual void WriteSchedulerMessage(EngineLogEntryType type, string message)
		{
			//duplicate logged messages to console if working on console mode:
			if (Environment.UserInteractive) { Console.Write(message); }
			lock (EngineLogLock) { File.AppendAllText(this.EngineLogFile.FullName, message); }
		}

		private Object TaskLogLock = new Object();
		protected virtual void WriteSchedulerMessage(TaskLogEntryType type, string message)
		{
			//duplicate logged messages to console if working on console mode:
			if (Environment.UserInteractive) { Console.Write(message); }
			lock (TaskLogLock) { File.AppendAllText(this.TaskLogFile.FullName, message); }
		}

		#region cosmetics:
		private String PolishMessage(String message)
		{
			if (message == null) { return null; }

			//indent lines after the first:
			message = message.Replace(Environment.NewLine, Environment.NewLine + "\t");

			//ensure linefeed at end
			if (!message.EndsWith(Environment.NewLine)) { message += Environment.NewLine; }
			return message;
		}

		private String WrapAsCsvValue(String input)
		{
			if (input == null) { return null; }
			if (input.Contains(this.CsvLineSeparator) || input.Contains(this.CsvValueWrapper) || input.Contains(this.CsvFieldSeparator))
			{
				return @"" + this.CsvValueWrapper
					+ input.Replace(this.CsvValueWrapper.ToString(), this.CsvValueWrapper.ToString() + this.CsvValueWrapper) //escape by writing wrapper twice
					+ this.CsvValueWrapper;
			}
			return input;
		}
		#endregion
	}
}
