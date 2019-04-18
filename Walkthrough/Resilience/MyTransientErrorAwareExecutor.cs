using System;
using System.Threading;
using System.Threading.Tasks;
using LogicMine;

namespace Resilience
{
    public class MyTransientErrorAwareExecutor : ITransientErrorAwareExecutor
    {
        public void Execute(Action operation)
        {
            // we won't need this
            throw new NotImplementedException();
        }

        public Task ExecuteAsync(Func<Task> operation)
        {
            // we won't need this
            throw new NotImplementedException();
        }

        public TResult Execute<TResult>(Func<TResult> operation)
        {
            // we won't need this
            throw new NotImplementedException();
        }

        public async Task<TResult> ExecuteAsync<TResult>(Func<Task<TResult>> operation)
        {
            Exception exception = null;
            for (var i = 0; i < 3; i++)
            {
                try
                {
                    return await operation();
                }
                catch (Exception ex)
                {
                    exception = ex;
                }

                Thread.Sleep(750);
            }

            throw new InvalidOperationException($"Operation failed after 3 attempts: {exception?.Message}", exception);
        }
    }
}