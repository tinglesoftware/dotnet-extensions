namespace System.Threading.Tasks
{
    /// <summary>
    /// Extensions for <see cref="Task"/>
    /// </summary>
    public static class TaskExtensions
    {
        #region Faulting

        /// <summary>
        /// Attach a fault action to a task, executed if and when the task status is <see cref="TaskStatus.Faulted"/> of when execution is synchronous
        /// </summary>
        /// <param name="task">the task to be attached to</param>
        /// <param name="faultAction">the action to be executed</param>
        public static void OnFault(this Task task, Action<Task> faultAction)
        {
            switch (task.Status)
            {
                case TaskStatus.RanToCompletion:
                case TaskStatus.Canceled:
                    break;
                case TaskStatus.Faulted:
                    faultAction(task);
                    break;
                default:
                    task.ContinueWith(faultAction, TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously);
                    break;
            }
        }

        /// <summary>
        /// Attach a fault action to a task, executed if and when the task status is <see cref="TaskStatus.Faulted"/> of when execution is synchronous
        /// </summary>
        /// <typeparam name="TResult">the result returned by the task</typeparam>
        /// <param name="task">the task to be attached to</param>
        /// <param name="faultAction">the action to be executed</param>
        public static void OnFault<TResult>(this Task<TResult> task, Action<Task<TResult>> faultAction)
        {
            switch (task.Status)
            {
                case TaskStatus.RanToCompletion:
                case TaskStatus.Canceled:
                    break;
                case TaskStatus.Faulted:
                    faultAction(task);
                    break;
                default:
                    task.ContinueWith(faultAction, TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously);
                    break;
            }
        }

        /// <summary>
        /// Attach a fault action to a task with state, executed if and when the task status is <see cref="TaskStatus.Faulted"/> of when execution is synchronous
        /// </summary>
        /// <param name="task">the task to be attached to</param>
        /// <param name="faultAction">the action to be executed</param>
        /// <param name="state">the state to be passed along</param>
        public static void OnFault(this Task task, Action<Task, object> faultAction, object state)
        {
            switch (task.Status)
            {
                case TaskStatus.RanToCompletion:
                case TaskStatus.Canceled:
                    break;
                case TaskStatus.Faulted:
                    faultAction(task, state);
                    break;
                default:
                    task.ContinueWith(faultAction, state, TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously);
                    break;
            }
        }

        /// <summary>
        /// Attach a fault action to a task with state, executed if and when the task status is <see cref="TaskStatus.Faulted"/> of when execution is synchronous
        /// </summary>
        /// <typeparam name="TResult">the result returned by the task</typeparam>
        /// <param name="task">the task to be attached to</param>
        /// <param name="faultAction">the action to be executed</param>
        /// <param name="state">the state to be passed along</param>
        public static void OnFault<TResult>(this Task<TResult> task, Action<Task<TResult>, object> faultAction, object state)
        {
            switch (task.Status)
            {
                case TaskStatus.RanToCompletion:
                case TaskStatus.Canceled:
                    break;
                case TaskStatus.Faulted:
                    faultAction(task, state);
                    break;
                default:
                    task.ContinueWith(faultAction, state, TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously);
                    break;
            }
        }
        #endregion

        #region Timeout

        /// <summary>
        /// Set timeout for a task
        /// </summary>
        /// <typeparam name="TResult">the result returned by the task</typeparam>
        /// <param name="task">the task to be awaited</param>
        /// <param name="timeout">the amount of time to wait</param>
        /// <returns></returns>
        public static async Task<TResult> WithTimeout<TResult>(this Task<TResult> task, TimeSpan timeout)
        {
            if (task.IsCompleted || (timeout == Timeout.InfiniteTimeSpan)) return task.Result;

            using (var cts = new CancellationTokenSource())
            {
                if (task == await Task.WhenAny(task, Task.Delay(timeout, cts.Token)))
                {
                    cts.Cancel();
                    return await task;
                }
            }

            throw new TimeoutException();
        }

        /// <summary>
        /// Set timeout for a task
        /// </summary>
        /// <param name="task">the task to be awaited</param>
        /// <param name="timeout">the amount of time to wait</param>
        /// <returns></returns>
        public static async Task WithTimeout(this Task task, TimeSpan timeout)
        {
            if (task.IsCompleted || (timeout == Timeout.InfiniteTimeSpan)) return;

            using (var cts = new CancellationTokenSource())
            {
                if (task == await Task.WhenAny(task, Task.Delay(timeout, cts.Token)))
                {
                    cts.Cancel();
                    await task;
                    return;
                }
            }

            throw new TimeoutException();
        }

        #endregion
    }
}
