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

	This file is from project https://github.com/NortalLTD/Utilities.TaskSchedulerEngine, Nortal.Utilities.TaskSchedulerEngine, file 'ISchedulerEngineLogger.cs'.
*/
using System;

namespace Nortal.Utilities.TaskSchedulerEngine
{
	public interface ISchedulerEngineLogger
	{
		void LogEngineMessage(SchedulerEngineBase engine, EngineLogEntryType type, String additionalInfo);
		void LogTaskMessage(SchedulerEngineBase engine, TaskLogEntryType type, ISchedulerTask task, DateTime taskStartedOn, String additionalInfo);
	}
}
