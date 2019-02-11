//using System;
//using LogicMine;
//using Test.Common.LogicMine.Mine.GetDeconstructedDate;
//using Test.Common.LogicMine.Mine.GetTime;
//using Xunit;
//
//namespace Test.LogicMine
//{
//    public class BasketPayloadTest
//    {
//        [Fact]
//        public void ConstructNonGeneric()
//        {
//            var request = new GetDeconstructedDateRequest();
//            var payload = new BasketPayload(request, typeof(GetDeconstructedDateRespone));
//
//            Assert.Equal(typeof(GetDeconstructedDateRequest), payload.RequestType);
//            Assert.Equal(typeof(GetDeconstructedDateRespone), payload.ResponseType);
//
//            Assert.Equal(request, payload.Request);
//            Assert.Null(payload.Response);
//        }
//
//        [Fact]
//        public void ConstructNonGeneric_BadArgs()
//        {
//            var request = new GetDeconstructedDateRequest();
//
//            Assert.Throws<ArgumentNullException>(() => new BasketPayload(null, null));
//            Assert.Throws<ArgumentNullException>(() => new BasketPayload(null, typeof(GetDeconstructedDateRespone)));
//            Assert.Throws<ArgumentNullException>(() => new BasketPayload(request, null));
//
//            var ex = Assert.Throws<ArgumentException>(() => new BasketPayload(request, typeof(IRequest)));
//            Assert.Equal($"Expected 'responseType' to descend from '{typeof(IResponse)}'", ex.Message);
//        }
//
//        [Fact]
//        public void ConstructGeneric()
//        {
//            var request = new GetDeconstructedDateRequest();
//            var payload = new BasketPayload<GetDeconstructedDateRequest, GetDeconstructedDateRespone>(request);
//
//            Assert.Equal(typeof(GetDeconstructedDateRequest), payload.RequestType);
//            Assert.Equal(typeof(GetDeconstructedDateRespone), payload.ResponseType);
//
//            Assert.Equal(request, payload.Request);
//            Assert.Null(payload.Response);
//        }
//
//        [Fact]
//        public void ConstructGeneric_BadArgs()
//        {
//            Assert.Throws<ArgumentNullException>(() =>
//                new BasketPayload<GetDeconstructedDateRequest, GetDeconstructedDateRespone>(null));
//        }
//
////        [Fact]
////        public void Unwrap()
////        {
////            var request = new GetDeconstructedDateRequest();
////
////            var payload = new BasketPayload(request, typeof(GetDeconstructedDateRespone));
////
////            var unwrapped = payload.Unwrap<GetDeconstructedDateRequest, GetDeconstructedDateRespone>();
////
////            Assert.True(unwrapped is BasketPayload<GetDeconstructedDateRequest, GetDeconstructedDateRespone>);
////
////            Assert.Equal(typeof(GetDeconstructedDateRequest), unwrapped.RequestType);
////            Assert.Equal(typeof(GetDeconstructedDateRespone), unwrapped.ResponseType);
////            Assert.Equal(request, unwrapped.Request);
////            Assert.Null(unwrapped.Response);
////
////            var response = new GetDeconstructedDateRespone(request);
////            payload.Response = response;
////
////            unwrapped = payload.Unwrap<GetDeconstructedDateRequest, GetDeconstructedDateRespone>();
////            Assert.Equal(response, unwrapped.Response);
////        }
////
////        [Fact]
////        public void Unwrap_Subtype()
////        {
////            var request = new GetDeconstructedDateRequest();
////
////            var payload = new BasketPayload(request, typeof(GetDeconstructedDateRespone));
////
////            var unwrapped = payload.Unwrap<IRequest, IResponse>();
////
////            Assert.True(unwrapped is BasketPayload<IRequest, IResponse>);
////
////            Assert.Equal(typeof(GetDeconstructedDateRequest), unwrapped.RequestType);
////            Assert.Equal(typeof(IResponse), unwrapped.ResponseType);
////            Assert.Equal(request, unwrapped.Request);
////            Assert.Null(unwrapped.Response);
////
////            var response = new GetDeconstructedDateRespone(request);
////            payload.Response = response;
////
////            unwrapped = payload.Unwrap<IRequest, IResponse>();
////            Assert.Equal(typeof(GetDeconstructedDateRequest), unwrapped.RequestType);
////            Assert.Equal(typeof(IResponse), unwrapped.ResponseType);
////            Assert.Equal(response, unwrapped.Response);
////        }
////
////        [Fact]
////        public void Unwrap_Bad()
////        {
////            var request = new GetDeconstructedDateRequest();
////            var responseType = typeof(GetDeconstructedDateRespone);
////
////            var payload = new BasketPayload(request, responseType);
////
////            var ex = Assert.Throws<InvalidOperationException>(() =>
////                payload.Unwrap<GetTimeRequest, GetDeconstructedDateRespone>());
////            Assert.Equal($"The request type '{request.GetType()}' is not a '{typeof(GetTimeRequest)}'", ex.Message);
////
////            ex = Assert.Throws<InvalidOperationException>(() =>
////                payload.Unwrap<GetDeconstructedDateRequest, GetTimeResponse>());
////            Assert.Equal($"The response type '{responseType}' is not a '{typeof(GetTimeResponse)}'", ex.Message);
////        }
//    }
//}