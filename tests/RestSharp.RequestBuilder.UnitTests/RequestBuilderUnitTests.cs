using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestSharp.RequestBuilder.Interfaces;

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
            _builder
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
            _builder
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

        [TestMethod]
        public void AddParameter_Null_Argument_Throws_Exception()
        {
            Assert.ThrowsException<ArgumentNullException>(() => _builder.AddParameter(null));
        }

        [TestMethod]
        public void AddParameter_New_Parameter_Is_Added()
        {
            var param = new QueryParameter("test-param", "test-value");
            var request = _builder.AddParameter(param).Create();

            var addedParam = request.Parameters.FirstOrDefault(p => p.Name == "test-param");
            Assert.IsNotNull(addedParam);
            Assert.AreEqual("test-value", addedParam.Value);
        }

        [TestMethod]
        public void AddParameter_Duplicate_Parameter_Is_Replaced()
        {
            var param1 = new QueryParameter("test-param", "value1");
            var param2 = new QueryParameter("test-param", "value2");
            
            var request = _builder
                .AddParameter(param1)
                .AddParameter(param2)
                .Create();

            var matchingParams = request.Parameters.Where(p => p.Name == "test-param").ToList();
            Assert.AreEqual(1, matchingParams.Count);
            Assert.AreEqual("value2", matchingParams[0].Value);
        }

        [TestMethod]
        public void AddParameter_Case_Insensitive_Duplicate_Is_Replaced()
        {
            var param1 = new QueryParameter("Test-Param", "value1");
            var param2 = new QueryParameter("test-param", "value2");
            
            var request = _builder
                .AddParameter(param1)
                .AddParameter(param2)
                .Create();

            var matchingParams = request.Parameters.Where(p => 
                string.Equals(p.Name, "test-param", StringComparison.InvariantCultureIgnoreCase)).ToList();
            Assert.AreEqual(1, matchingParams.Count);
            Assert.AreEqual("value2", matchingParams[0].Value);
        }

        [TestMethod]
        public void AddParameters_All_New_Parameters_Are_Added()
        {
            var parameters = new Parameter[]
            {
                new QueryParameter("param1", "value1"),
                new QueryParameter("param2", "value2"),
                new QueryParameter("param3", "value3")
            };

            var request = _builder.AddParameters(parameters).Create();

            Assert.AreEqual(3, request.Parameters.Count);
            Assert.IsNotNull(request.Parameters.FirstOrDefault(p => p.Name == "param1"));
            Assert.IsNotNull(request.Parameters.FirstOrDefault(p => p.Name == "param2"));
            Assert.IsNotNull(request.Parameters.FirstOrDefault(p => p.Name == "param3"));
        }

        [TestMethod]
        public void AddParameters_Duplicate_Parameters_Are_Replaced()
        {
            var param1 = new QueryParameter("param1", "oldValue");
            _builder.AddParameter(param1);

            var parameters = new Parameter[]
            {
                new QueryParameter("param1", "newValue"),
                new QueryParameter("param2", "value2")
            };

            var request = _builder.AddParameters(parameters).Create();

            var matchingParams = request.Parameters.Where(p => p.Name == "param1").ToList();
            Assert.AreEqual(1, matchingParams.Count);
            Assert.AreEqual("newValue", matchingParams[0].Value);
            Assert.AreEqual(2, request.Parameters.Count);
        }

        [TestMethod]
        public void AddParameters_Case_Insensitive_Duplicates_Are_Replaced()
        {
            var param1 = new QueryParameter("Param1", "oldValue");
            _builder.AddParameter(param1);

            var parameters = new Parameter[]
            {
                new QueryParameter("param1", "newValue"),
                new QueryParameter("PARAM2", "value2")
            };

            var request = _builder.AddParameters(parameters).Create();

            var matchingParams = request.Parameters.Where(p => 
                string.Equals(p.Name, "param1", StringComparison.InvariantCultureIgnoreCase)).ToList();
            Assert.AreEqual(1, matchingParams.Count);
            Assert.AreEqual("newValue", matchingParams[0].Value);
        }

        [TestMethod]
        public void AddParameters_Null_Array_Returns_Builder()
        {
            var result = _builder.AddParameters(null);
            Assert.IsNotNull(result);
            Assert.AreSame(_builder, result);
        }

        [TestMethod]
        public void AddParameters_Empty_Array_Returns_Builder()
        {
            var result = _builder.AddParameters(Array.Empty<Parameter>());
            Assert.IsNotNull(result);
            Assert.AreSame(_builder, result);
        }

        [TestMethod]
        public void AddParameters_Preserves_Insertion_Order()
        {
            var param1 = new QueryParameter("param1", "value1");
            var param2 = new QueryParameter("param2", "value2");
            _builder.AddParameter(param1).AddParameter(param2);

            var parameters = new Parameter[]
            {
                new QueryParameter("param3", "value3"),
                new QueryParameter("param4", "value4")
            };

            var request = _builder.AddParameters(parameters).Create();

            var paramNames = request.Parameters.Select(p => p.Name).ToList();
            Assert.AreEqual("param1", paramNames[0]);
            Assert.AreEqual("param2", paramNames[1]);
            Assert.AreEqual("param3", paramNames[2]);
            Assert.AreEqual("param4", paramNames[3]);
        }

        [TestMethod]
        public void AddParameters_Skips_Null_Parameters()
        {
            var parameters = new Parameter[]
            {
                new QueryParameter("param1", "value1"),
                null,
                new QueryParameter("param2", "value2")
            };

            var request = _builder.AddParameters(parameters).Create();

            Assert.AreEqual(2, request.Parameters.Count);
            Assert.IsNotNull(request.Parameters.FirstOrDefault(p => p.Name == "param1"));
            Assert.IsNotNull(request.Parameters.FirstOrDefault(p => p.Name == "param2"));
        }

        [TestMethod]
        public void AddParameters_Handles_Duplicates_Within_Input_Array()
        {
            var parameters = new Parameter[]
            {
                new QueryParameter("param1", "firstValue"),
                new QueryParameter("param1", "secondValue"),
                new QueryParameter("param1", "thirdValue")
            };

            var request = _builder.AddParameters(parameters).Create();

            var matchingParams = request.Parameters.Where(p => p.Name == "param1").ToList();
            Assert.AreEqual(1, matchingParams.Count);
            Assert.AreEqual("thirdValue", matchingParams[0].Value);
        }
    }
}