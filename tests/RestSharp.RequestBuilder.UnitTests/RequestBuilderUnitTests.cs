using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestSharp;
using RestSharp.RequestBuilder.Interfaces;
using System;
using System.Linq;

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
            Assert.ThrowsException<ArgumentNullException>(static () => new RequestBuilder((string)null));
        }

        [TestMethod]
        public void Constructor_With_Resource_Is_Added()
        {
            var request = _builder.SetMethod(Method.Get).Create();

            Assert.AreEqual("test", request.Resource);
        }

        [TestMethod]
        public void AddBody_Null_Argument_Throws_Exception()
        {
            Assert.ThrowsException<ArgumentNullException>(() => _builder.AddBody(null));
        }

        [TestMethod]
        public void SetFormat_With_Value_Is_Correct()
        {
            var request = _builder.SetFormat(DataFormat.Json).Create();

            Assert.AreEqual(DataFormat.Json, request.RequestFormat);
        }

        [TestMethod]
        public void AddHeader_With_Dupe_Header_Returns_Valid_Count_1()
        {
            var request = _builder
                .SetFormat(DataFormat.Json)
                .SetMethod(Method.Get)
                .AddHeader("test-header", "header-value")
                .AddHeader("test-header", "header-value")
                .Create();

            Assert.AreEqual(1, _builder.HeaderCount);
        }

        [TestMethod]
        public void RemoveHeaders_Returns_Valid_Count_0()
        {
            var request = _builder
                .SetFormat(DataFormat.Json)
                .SetMethod(Method.Get)
                .AddHeader("test-header", "header-value")
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

        [TestMethod]
        public void AddCookie_With_Values_Is_Correct()
        {
            var request = _builder.AddCookie("cookie-name", "cookie-value", "/", "domain.com").Create();

            Assert.AreEqual("cookie-value", request.CookieContainer.GetAllCookies().FirstOrDefault(p => p.Name == "cookie-name").Value);
        }

        [TestMethod]
        public void SetMethod_With_Value_Is_Correct()
        {
            var request = _builder.SetMethod(Method.Post).Create();

            Assert.AreEqual(Method.Post, request.Method);
        }

        [TestMethod]
        public void SetTimeout_With_Value_Is_Correct()
        {
            var timeout = TimeSpan.FromSeconds(60);
            var request = _builder.SetTimeout(timeout).Create();

            Assert.AreEqual(timeout, request.Timeout);
        }
    }
}