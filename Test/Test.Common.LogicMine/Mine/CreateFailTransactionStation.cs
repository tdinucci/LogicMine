using System;
using System.Threading.Tasks;
using LogicMine;
using LogicMine.DataObject.CreateObject;

namespace Test.Common.LogicMine.Mine
{
    public class CreateFailTransactionStation<T, TId> : Station<CreateObjectRequest<T>, CreateObjectResponse<T, TId>>
        where T : class
    {
        private readonly bool _throwException;

        public CreateFailTransactionStation(bool throwException)
        {
            _throwException = throwException;
        }

        public override Task AscendFromAsync(IBasket<CreateObjectRequest<T>, CreateObjectResponse<T, TId>> basket)
        {
            // do this on the ascent because the record will have been inserted at this point.
            // the test is to ensure that the inserted record is not committed
            if (_throwException)
                throw new InvalidOperationException("A failure has occurred");

            return base.AscendFromAsync(basket);
        }

        public override Task DescendToAsync(IBasket<CreateObjectRequest<T>, CreateObjectResponse<T, TId>> basket)
        {
            return Task.CompletedTask;
        }
    }
}