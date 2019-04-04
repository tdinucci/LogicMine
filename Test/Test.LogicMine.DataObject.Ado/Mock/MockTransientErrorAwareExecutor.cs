using System;
using System.Threading.Tasks;
using LogicMine;

namespace Test.LogicMine.DataObject.Ado.Mock
{
    public class MockTransientErrorAwareExecutor : ITransientErrorAwareExecutor
    {
        private readonly string _retryOnExMessage;
        private readonly Func<Task<object>> _passOnThirdAttemptFunc;


        public MockTransientErrorAwareExecutor(string retryOnExMessage,
            Func<Task<object>> passOnThirdAttemptFunc = null)
        {
            _retryOnExMessage = retryOnExMessage;
            _passOnThirdAttemptFunc = passOnThirdAttemptFunc;
        }

        public void Execute(Action operation)
        {
            throw new NotImplementedException();
        }

        public TResult Execute<TResult>(Func<TResult> operation)
        {
            throw new NotImplementedException();
        }

        public Task<TResult> ExecuteAsync<TResult>(Func<Task<TResult>> operation)
        {
            return DoExecuteAsync(operation, 1);
        }

        public Task ExecuteAsync(Func<Task> operation)
        {
            return DoExecuteAsync(operation, 1);
        }

        private async Task<TResult> DoExecuteAsync<TResult>(Func<Task<TResult>> operation, int attempt)
        {
            try
            {
                if (attempt == 3 && _passOnThirdAttemptFunc != null)
                {
                    var result = await _passOnThirdAttemptFunc().ConfigureAwait(false);
                    return (TResult) result;
                }

                return await operation().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (!ex.Message.StartsWith(_retryOnExMessage))
                    throw;

                if (attempt < 3)
                    return await DoExecuteAsync(operation, attempt + 1).ConfigureAwait(false);

                throw new InvalidOperationException($"Failed after {attempt} attempts");
            }
        }

        private async Task DoExecuteAsync(Func<Task> operation, int attempt)
        {
            try
            {
                if (attempt == 3 && _passOnThirdAttemptFunc != null)
                {
                    await _passOnThirdAttemptFunc().ConfigureAwait(false);
                }

                await operation().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (!ex.Message.StartsWith(_retryOnExMessage))
                    throw;

                if (attempt < 3)
                    await DoExecuteAsync(operation, attempt + 1).ConfigureAwait(false);
                else
                    throw new InvalidOperationException($"Failed after {attempt} attempts");
            }
        }
    }
}