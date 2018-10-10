using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestSharp.RequestBuilder.Interfaces;
using System;

namespace RestSharp.RequestBuilder.UnitTests
{
    [TestClass]
    public class RequestBuilderUnitTests
    {
        private IRequestBuilder _builder;

        [TestInitialize]
        public void SetUp()
        {
            _builder = new RequestBuilder("test");
        }

        [TestCleanup]
        public void CleanUp()
        {
            _builder = null;
        }

        [TestMethod]
        public void Constructor_Null_Resource_Throws_Exception()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new RequestBuilder(null));
        }

        [TestMethod]
        public void Constructor_With_Resource_Is_Added()
        {
            var request = _builder.SetMethod(Method.GET).Create();

            Assert.AreEqual("test", request.Resource);
        }

        [TestMethod]
        public void AddBody_Null_Argument_Throws_Exception()
        {
            Assert.ThrowsException<ArgumentNullException>(() => _builder.AddBody(null));
        }

        [TestMethod]
        public void SetMethod_With_Value_Is_Correct()
        {
            var request = _builder.SetMethod(Method.GET).Create();

            Assert.AreEqual(Method.GET, request.Method);
        }

        [TestMethod]
        public void SetFormat_With_Value_Is_Correct()
        {
            var request = _builder.SetFormat(DataFormat.Json).Create();

            Assert.AreEqual(DataFormat.Json, request.RequestFormat);
        }

        [TestMethod]
        public void AddHeader_With_Value_Is_Created()
        {
            var request = _builder
                .SetFormat(DataFormat.Json)
                .SetMethod(Method.GET)
                .AddHeader("test-header", "asdfasdfasdfasdfasdfasdfasdfasdfas")
                .Create();

            Assert.IsNotNull(request);
        }

        [TestMethod]
        public void AddHeader_With_Dupe_Header_Returns_Valid_Count_1()
        {
            var request = _builder
                .SetFormat(DataFormat.Json)
                .SetMethod(Method.GET)
                .AddHeader("test-header", "asdfasdfasdfasdfasdfasdfasdfasdfas")
                .AddHeader("test-header", "asdfasdfasdfasdfasdfasdfasdfasdfas")
                .Create();

            Assert.AreEqual(1, _builder.HeaderCount);
        }

        [TestMethod]
        public void RemoveHeaders_Returns_Valid_Count_0()
        {
            var request = _builder
                .SetFormat(DataFormat.Json)
                .SetMethod(Method.GET)
                .AddHeader("test-header", "asdfasdfasdfasdfasdfasdfasdfasdfas")
                .RemoveHeaders()
                .Create();

            Assert.AreEqual(0, _builder.HeaderCount);
        }

        [TestMethod]
        public void RemoveHeader_Null_Argument_Throws_Exception()
        {
            Assert.ThrowsException<ArgumentNullException>(() => _builder.RemoveHeader(null));
        }

        [TestMethod]
        public void RemoveParameter_Null_Argument_Throws_Exception()
        {
            Assert.ThrowsException<ArgumentNullException>(() => _builder.RemoveParameter(null));
        }

    }
}
