using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestSharp.RequestBuilder.Interfaces;

namespace RestSharp.RequestBuilder.UnitTests
{
    [TestClass]
    public class RequestBuilderTests
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

        // --- Constructors ---
        [TestMethod]
        public void Constructor_Uri_Null_Throws()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new RequestBuilder((Uri)null));
        }

        [TestMethod]
        public void Constructor_Null_Resource_Throws_Exception()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new RequestBuilder((string)null));
        }

        [TestMethod]
        public void Constructor_String_Sets_Resource()
        {
            var builder = new RequestBuilder("resource");
            var request = builder.Create();
            Assert.AreEqual("resource", request.Resource);
        }

        [TestMethod]
        public void Constructor_Uri_Sets_Resource()
        {
            var uri = new Uri("http://localhost/api");
            var builder = new RequestBuilder(uri);
            var request = builder.Create();
            Assert.AreEqual(uri.ToString(), request.Resource);
        }

        [TestMethod]
        public void Constructor_String_Method_Sets_Method()
        {
            var builder = new RequestBuilder("resource", Method.Post);
            var request = builder.Create();
            Assert.AreEqual(Method.Post, request.Method);
        }

        [TestMethod]
        public void Constructor_String_Method_Format_Sets_Format()
        {
            var builder = new RequestBuilder("resource", Method.Put, DataFormat.Xml);
            var request = builder.Create();
            Assert.AreEqual(DataFormat.Xml, request.RequestFormat);
        }

        // --- AddBody ---
        [TestMethod]
        public void AddBody_Null_Throws()
        {
            var builder = new RequestBuilder("resource");
            Assert.ThrowsException<ArgumentNullException>(() => builder.AddBody(null));
        }

        [TestMethod]
        public void AddBody_Null_Argument_Throws_Exception()
        {
            Assert.ThrowsException<ArgumentNullException>(() => _builder.AddBody(null));
        }

        // --- AddFile ---
        [TestMethod]
        public void AddFile_Null_Name_Throws()
        {
            var builder = new RequestBuilder("resource");
            Assert.ThrowsException<ArgumentNullException>(() => builder.AddFile(null, "path"));
        }

        [TestMethod]
        public void AddFile_Null_Path_Throws()
        {
            var builder = new RequestBuilder("resource");
            Assert.ThrowsException<ArgumentNullException>(() => builder.AddFile("file", null));
        }

        // --- SetFormat ---
        [TestMethod]
        public void SetFormat_Sets_Format()
        {
            var builder = new RequestBuilder("resource");
            var request = builder.SetFormat(DataFormat.Xml).Create();
            Assert.AreEqual(DataFormat.Xml, request.RequestFormat);
        }

        [TestMethod]
        public void SetFormat_With_Value_Is_Correct()
        {
            var request = _builder.SetFormat(DataFormat.Json).Create();
            Assert.AreEqual(DataFormat.Json, request.RequestFormat);
        }

        // --- Headers ---
        [TestMethod]
        public void AddHeader_Adds_Header()
        {
            var builder = new RequestBuilder("resource");
            builder.AddHeader("h", "v");
            Assert.AreEqual(1, builder.HeaderCount);
        }

        [TestMethod]
        public void AddHeader_Replaces_Header_If_Different()
        {
            var builder = new RequestBuilder("resource");
            builder.AddHeader("h", "v1");
            builder.AddHeader("h", "v2");
            Assert.AreEqual(1, builder.HeaderCount);
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
        public void AddHeaders_Adds_Multiple_Headers()
        {
            var builder = new RequestBuilder("resource");
            builder.AddHeaders(new System.Collections.Generic.Dictionary<string, string>
            {
                { "h1", "v1" },
                { "h2", "v2" }
            });
            Assert.AreEqual(2, builder.HeaderCount);
        }

        [TestMethod]
        public void RemoveHeader_Null_Throws()
        {
            var builder = new RequestBuilder("resource");
            Assert.ThrowsException<ArgumentNullException>(() => builder.RemoveHeader(null));
        }

        [TestMethod]
        public void RemoveHeader_Null_Argument_Throws_Exception()
        {
            Assert.ThrowsException<ArgumentNullException>(() => _builder.RemoveHeader(null));
        }

        [TestMethod]
        public void RemoveHeader_Removes_Header()
        {
            var builder = new RequestBuilder("resource");
            builder.AddHeader("h", "v");
            builder.RemoveHeader("h");
            Assert.AreEqual(0, builder.HeaderCount);
        }

        [TestMethod]
        public void RemoveHeaders_Clears_Headers()
        {
            var builder = new RequestBuilder("resource");
            builder.AddHeader("h1", "v1").AddHeader("h2", "v2");
            builder.RemoveHeaders();
            Assert.AreEqual(0, builder.HeaderCount);
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

        // --- Cookies ---
        [TestMethod]
        public void AddCookie_Adds_Cookie()
        {
            var builder = new RequestBuilder("resource");
            var request = builder.AddCookie("c", "v", "/", "d").Create();
            Assert.IsTrue(request.CookieContainer.GetAllCookies().Any(c => c.Name == "c"));
        }

        [TestMethod]
        public void AddCookie_With_Values_Is_Correct()
        {
            var request = _builder.AddCookie("cookie-name", "cookie-value", "/", "domain.com").Create();
            Assert.AreEqual("cookie-value", request.CookieContainer.GetAllCookies().FirstOrDefault(p => p.Name == "cookie-name").Value);
        }

        [TestMethod]
        public void RemoveCookies_Clears_Cookies()
        {
            var builder = new RequestBuilder("resource");
            builder.AddCookie("c", "v", "/", "d");
            builder.RemoveCookies();
            var request = builder.Create();
            Assert.IsNull(request.CookieContainer?.GetAllCookies()?.Count);
        }

        // --- Method/Timeout ---
        [TestMethod]
        public void SetMethod_Sets_Method()
        {
            var builder = new RequestBuilder("resource");
            var request = builder.SetMethod(Method.Delete).Create();
            Assert.AreEqual(Method.Delete, request.Method);
        }

        [TestMethod]
        public void SetMethod_With_Value_Is_Correct()
        {
            var request = _builder.SetMethod(Method.Post).Create();
            Assert.AreEqual(Method.Post, request.Method);
        }

        [TestMethod]
        public void SetTimeout_Sets_Timeout()
        {
            var builder = new RequestBuilder("resource");
            var timeout = TimeSpan.FromSeconds(10);
            var request = builder.SetTimeout(timeout).Create();
            Assert.AreEqual(timeout, request.Timeout);
        }

        [TestMethod]
        public void SetTimeout_With_Value_Is_Correct()
        {
            var timeout = TimeSpan.FromSeconds(60);
            var request = _builder.SetTimeout(timeout).Create();
            Assert.AreEqual(timeout, request.Timeout);
        }

        // --- Parameters ---
        [TestMethod]
        public void AddParameter_Null_Throws()
        {
            var builder = new RequestBuilder("resource");
            Assert.ThrowsException<ArgumentNullException>(() => builder.AddParameter(null));
        }

        [TestMethod]
        public void AddParameter_Null_Argument_Throws_Exception()
        {
            Assert.ThrowsException<ArgumentNullException>(() => _builder.AddParameter(null));
        }

        [TestMethod]
        public void AddParameter_Adds_Parameter()
        {
            var builder = new RequestBuilder("resource");
            var param = new QueryParameter("p", "v");
            var request = builder.AddParameter(param).Create();
            Assert.IsTrue(request.Parameters.Any(p => p.Name == "p"));
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
        public void AddParameter_Replaces_If_Exists()
        {
            var builder = new RequestBuilder("resource");
            var p1 = new QueryParameter("p", "v1");
            var p2 = new QueryParameter("p", "v2");
            var request = builder.AddParameter(p1).AddParameter(p2).Create();
            var found = request.Parameters.FirstOrDefault(p => p.Name == "p");
            Assert.AreEqual("v2", found.Value);
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
        public void AddParameters_Null_Returns_Same()
        {
            var builder = new RequestBuilder("resource");
            var result = builder.AddParameters(null);
            Assert.AreSame(builder, result);
        }

        [TestMethod]
        public void AddParameters_Null_Array_Returns_Builder()
        {
            var result = _builder.AddParameters(null);
            Assert.IsNotNull(result);
            Assert.AreSame(_builder, result);
        }

        [TestMethod]
        public void AddParameters_Empty_Returns_Same()
        {
            var builder = new RequestBuilder("resource");
            var result = builder.AddParameters(new Parameter[0]);
            Assert.AreSame(builder, result);
        }

        [TestMethod]
        public void AddParameters_Empty_Array_Returns_Builder()
        {
            var result = _builder.AddParameters(Array.Empty<Parameter>());
            Assert.IsNotNull(result);
            Assert.AreSame(_builder, result);
        }

        [TestMethod]
        public void AddParameters_Adds_And_Replaces()
        {
            var builder = new RequestBuilder("resource");
            builder.AddParameter(new QueryParameter("p1", "v1"));
            var arr = new Parameter[]
            {
                new QueryParameter("p1", "v2"),
                new QueryParameter("p2", "v3")
            };
            var request = builder.AddParameters(arr).Create();
            Assert.AreEqual(2, request.Parameters.Count);
            Assert.AreEqual("v2", request.Parameters.First(p => p.Name == "p1").Value);
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

        [TestMethod]
        public void RemoveParameters_Clears_Parameters()
        {
            var builder = new RequestBuilder("resource");
            builder.AddParameter(new QueryParameter("p", "v"));
            builder.RemoveParameters();
            var request = builder.Create();
            Assert.AreEqual(0, request.Parameters.Count);
        }

        [TestMethod]
        public void RemoveParameter_Null_Throws()
        {
            var builder = new RequestBuilder("resource");
            Assert.ThrowsException<ArgumentNullException>(() => builder.RemoveParameter(null));
        }

        [TestMethod]
        public void RemoveParameter_Null_Argument_Throws_Exception()
        {
            Assert.ThrowsException<ArgumentNullException>(() => _builder.RemoveParameter(null));
        }

        [TestMethod]
        public void RemoveParameter_Removes_Parameter()
        {
            var builder = new RequestBuilder("resource");
            var param = new QueryParameter("p", "v");
            builder.AddParameter(param);
            builder.RemoveParameter(param);
            var request = builder.Create();
            Assert.IsFalse(request.Parameters.Any(p => p.Name == "p"));
        }
    }
}