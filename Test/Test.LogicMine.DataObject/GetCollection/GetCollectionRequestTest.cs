using System;
using System.Collections.Generic;
using System.Linq;
using LogicMine.DataObject.Filter;
using LogicMine.DataObject.GetCollection;
using Test.Common.LogicMine.DataType;
using Xunit;

namespace Test.LogicMine.DataObject.GetCollection
{
    public class GetCollectionRequestTest
    {
        [Fact]
        public void Construct()
        {
            var request = new GetCollectionRequest<Frog<int>>();

            Assert.False(string.IsNullOrWhiteSpace(request.Id.ToString()));
            Assert.True(request.Options != null && !request.Options.Any());
            Assert.Equal(typeof(Frog<int>), request.ObjectType);
            Assert.Null(request.Filter);
            Assert.Null(request.Max);
            Assert.Null(request.Page);
        }

        [Fact]
        public void Construct_Filter()
        {
            var filter = new Filter<Frog<int>>(new List<IFilterTerm>(new List<IFilterTerm>
                {new FilterTerm("abc", FilterOperators.Contains, 5)}));
            var request = new GetCollectionRequest<Frog<int>>(filter);

            Assert.False(string.IsNullOrWhiteSpace(request.Id.ToString()));
            Assert.True(request.Options != null && !request.Options.Any());
            Assert.Equal(typeof(Frog<int>), request.ObjectType);
            Assert.Equal(filter, request.Filter);
            Assert.Null(request.Max);
            Assert.Null(request.Page);
        }

        [Fact]
        public void Construct_Paged()
        {
            var max = DateTime.Now.Millisecond;
            var page = DateTime.Now.Millisecond;
            var request = new GetCollectionRequest<Frog<int>>(max, page);

            Assert.False(string.IsNullOrWhiteSpace(request.Id.ToString()));
            Assert.True(request.Options != null && !request.Options.Any());
            Assert.Equal(typeof(Frog<int>), request.ObjectType);
            Assert.Null(request.Filter);
            Assert.Equal(max, request.Max);
            Assert.Equal(page, request.Page);
        }

        [Fact]
        public void Construct_FilteredPaged()
        {
            var filter = new Filter<Frog<int>>(new List<IFilterTerm>(new List<IFilterTerm>
                {new FilterTerm("abc", FilterOperators.Contains, 5)}));
            
            var max = DateTime.Now.Millisecond;
            var page = DateTime.Now.Millisecond;
            var request = new GetCollectionRequest<Frog<int>>(filter, max, page, null);

            Assert.False(string.IsNullOrWhiteSpace(request.Id.ToString()));
            Assert.True(request.Options != null && !request.Options.Any());
            Assert.Equal(typeof(Frog<int>), request.ObjectType);
            Assert.Equal(filter, request.Filter);
            Assert.Equal(max, request.Max);
            Assert.Equal(page, request.Page);
        }
    }
}