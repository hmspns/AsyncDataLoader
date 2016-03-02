using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace gribov.pro
{
    public static class TaskLogger
    {
        public enum TaskLogLevel { None, Pending }
        public static TaskLogLevel LogLevel { get; set; }

        public sealed class TaskLogEntry
        {
            public Task Task { get; internal set; }
            public string Tag { get; internal set; }
            public DateTime LogTime { get; internal set; }
            public string CallerMemberName { get; internal set; }
            public string CallerFilePath { get; internal set; }
            public int CallerLineNumber { get; internal set; }

            public override string ToString()
            {
                return string.Format("LogTime={0}, Tag={1}, Member={2}, File={3}({4})",
                   LogTime, Tag ?? "(none)", CallerMemberName, CallerFilePath, CallerLineNumber);
            }
        }

        private static readonly ConcurrentDictionary<Task, TaskLogEntry> _log = new ConcurrentDictionary<Task, TaskLogEntry>();

        public static IEnumerable<TaskLogEntry> GetLogEntries() { return _log.Values; }

        public static Task<TResult> Log<TResult>(this Task<TResult> task, string tag = null,
           [CallerMemberName] string callerMemberName = null,
           [CallerFilePath] string callerFilePath = null,
           [CallerLineNumber] int callerLineNumber = -1)
        {
            return (Task<TResult>)Log((Task)task, tag, callerMemberName, callerFilePath, callerLineNumber);
        }

        public static Task Log(this Task task, string tag = null,
           [CallerMemberName] string callerMemberName = null,
           [CallerFilePath] string callerFilePath = null,
           [CallerLineNumber] int callerLineNumber = -1)
        {
            if (LogLevel == TaskLogLevel.None)
                return task;
            TaskLogEntry logEntry = new TaskLogEntry
            {
                Task = task,
                LogTime = DateTime.Now,
                Tag = tag,
                CallerMemberName = callerMemberName,
                CallerFilePath = callerFilePath,
                CallerLineNumber = callerLineNumber
            };
            _log[task] = logEntry;
            task.ContinueWith(t => { TaskLogEntry entry; _log.TryRemove(t, out entry); },
               TaskContinuationOptions.ExecuteSynchronously);
            return task;
        }
    }
}
