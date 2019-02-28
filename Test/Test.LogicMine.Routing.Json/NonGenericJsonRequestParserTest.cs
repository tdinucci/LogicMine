using System;
using System.IO;
using LogicMine;
using LogicMine.Routing.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Test.LogicMine.Routing.Json
{
    public class NonGenericJsonRequestParserTest
    {
        [Fact]
        public void Base64()
        {
            var aString = Guid.NewGuid().ToString();
            var anInt = DateTime.Now.Millisecond;

            var fileBytes = File.ReadAllBytes("testfile");
            var base64 = Convert.ToBase64String(fileBytes);

            var rawRequest = new JObject
            {
                {"requestType", "byteArrayTestType"},
                {"someString", aString},
                {"someInt", anInt},
                {"someByteArray", base64}
            };

            var request =
                new NonGenericJsonRequestParser(typeof(ByteArrayTestType)).Parse<ByteArrayTestType>(rawRequest);

            Assert.Equal(aString, request.SomeString);
            Assert.Equal(anInt, request.SomeInt);
            Assert.Equal(fileBytes, request.SomeByteArray);
        }

        private class ByteArrayTestType : Request
        {
            public string SomeString { get; set; }
            public int SomeInt { get; set; }
            public byte[] SomeByteArray { get; set; }
        }
    }
}