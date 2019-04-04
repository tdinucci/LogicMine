using System;
using System.Threading.Tasks;

namespace LogicMine
{
    /// <summary>
    /// Implementations of this interface can decide how operations are executed and how errors during execution are
    /// handled.
    /// Each operation within the LogicMine framework which reaches out to an external system (and is therefore subject to
    /// transient failures) can be executed within an instance of this type and it is up to the application using this
    /// framework to decide on the implementation.
    ///
    /// At present there is no plan to provide an implementation of this interface with LogicMine.  An easy way to
    /// create your own implementation would be to create an instance of this type which wraps Polly:
    /// https://github.com/App-vNext/Polly
    /// </summary>
    public interface ITransientErrorAwareExecutor
    {
        /// <summary>
        /// Decides how to execute an action
        /// </summary>
        /// <param name="operation">The action to execute</param>
        void Execute(Action operation);

        /// <summary>
        /// Decides how to execute an asynchronous action
        /// </summary>
        /// <param name="operation">The action to execute</param>
        /// <returns></returns>
        Task ExecuteAsync(Func<Task> operation);

        /// <summary>
        /// Decides how to execute a function
        /// </summary>
        /// <param name="operation">The function to execute</param>
        /// <typeparam name="TResult">The return type of the function to execute</typeparam>
        /// <returns></returns>
        TResult Execute<TResult>(Func<TResult> operation);

        /// <summary>
        /// Decides how to execute an asynchronous function
        /// </summary>
        /// <param name="operation">The function to execute</param>
        /// <typeparam name="TResult">The return type of the function to execute</typeparam>
        /// <returns></returns>
        Task<TResult> ExecuteAsync<TResult>(Func<Task<TResult>> operation);
    }
}