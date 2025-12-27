using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestSharp;
using RestSharp.RequestBuilder.Extensions;
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

        // --- AddQueryParameter ---
        [TestMethod]
        public void AddQueryParameter_Null_Name_Throws()
        {
            var builder = new RequestBuilder("resource");
            Assert.ThrowsException<ArgumentNullException>(() => builder.AddQueryParameter(null, "value"));
        }

        [TestMethod]
        public void AddQueryParameter_Empty_Name_Throws()
        {
            var builder = new RequestBuilder("resource");
            Assert.ThrowsException<ArgumentNullException>(() => builder.AddQueryParameter("", "value"));
        }

        [TestMethod]
        public void AddQueryParameter_Null_Value_Throws()
        {
            var builder = new RequestBuilder("resource");
            Assert.ThrowsException<ArgumentNullException>(() => builder.AddQueryParameter("name", null));
        }

        [TestMethod]
        public void AddQueryParameter_Adds_Query_Parameter()
        {
            var builder = new RequestBuilder("resource");
            var request = builder.AddQueryParameter("page", 1).Create();
            var param = request.Parameters.FirstOrDefault(p => p.Name == "page");
            Assert.IsNotNull(param);
            Assert.AreEqual("1", param.Value);
            Assert.AreEqual(ParameterType.QueryString, param.Type);
        }

        [TestMethod]
        public void AddQueryParameter_String_Value()
        {
            var builder = new RequestBuilder("resource");
            var request = builder.AddQueryParameter("name", "test").Create();
            var param = request.Parameters.FirstOrDefault(p => p.Name == "name");
            Assert.IsNotNull(param);
            Assert.AreEqual("test", param.Value);
        }

        [TestMethod]
        public void AddQueryParameter_Int_Value()
        {
            var builder = new RequestBuilder("resource");
            var request = builder.AddQueryParameter("count", 42).Create();
            var param = request.Parameters.FirstOrDefault(p => p.Name == "count");
            Assert.IsNotNull(param);
            Assert.AreEqual("42", param.Value);
        }

        [TestMethod]
        public void AddQueryParameter_Decimal_Value_Uses_InvariantCulture()
        {
            var builder = new RequestBuilder("resource");
            var request = builder.AddQueryParameter("price", 19.99m).Create();
            var param = request.Parameters.FirstOrDefault(p => p.Name == "price");
            Assert.IsNotNull(param);
            Assert.AreEqual("19.99", param.Value);
        }

        [TestMethod]
        public void AddQueryParameter_Bool_Value()
        {
            var builder = new RequestBuilder("resource");
            var request = builder.AddQueryParameter("active", true).Create();
            var param = request.Parameters.FirstOrDefault(p => p.Name == "active");
            Assert.IsNotNull(param);
            Assert.AreEqual("True", param.Value);
        }

        [TestMethod]
        public void AddQueryParameter_Replaces_Existing()
        {
            var builder = new RequestBuilder("resource");
            var request = builder
                .AddQueryParameter("page", 1)
                .AddQueryParameter("page", 2)
                .Create();
            var matchingParams = request.Parameters.Where(p => p.Name == "page").ToList();
            Assert.AreEqual(1, matchingParams.Count);
            Assert.AreEqual("2", matchingParams[0].Value);
        }

        [TestMethod]
        public void AddQueryParameter_Case_Insensitive_Replacement()
        {
            var builder = new RequestBuilder("resource");
            var request = builder
                .AddQueryParameter("Page", 1)
                .AddQueryParameter("page", 2)
                .Create();
            var matchingParams = request.Parameters.Where(p =>
                string.Equals(p.Name, "page", StringComparison.InvariantCultureIgnoreCase)).ToList();
            Assert.AreEqual(1, matchingParams.Count);
            Assert.AreEqual("2", matchingParams[0].Value);
        }

        [TestMethod]
        public void AddQueryParameter_Returns_Builder_For_Chaining()
        {
            var builder = new RequestBuilder("resource");
            var result = builder.AddQueryParameter("test", "value");
            Assert.AreSame(builder, result);
        }

        // --- AddQueryParameters ---
        [TestMethod]
        public void AddQueryParameters_Null_Returns_Builder()
        {
            var builder = new RequestBuilder("resource");
            var result = builder.AddQueryParameters(null);
            Assert.AreSame(builder, result);
        }

        [TestMethod]
        public void AddQueryParameters_Empty_Returns_Builder()
        {
            var builder = new RequestBuilder("resource");
            var result = builder.AddQueryParameters(new Dictionary<string, object>());
            Assert.AreSame(builder, result);
        }

        [TestMethod]
        public void AddQueryParameters_Adds_Multiple_Parameters()
        {
            var builder = new RequestBuilder("resource");
            var request = builder.AddQueryParameters(new Dictionary<string, object>
            {
                { "page", 1 },
                { "limit", 50 },
                { "sort", "desc" }
            }).Create();
            Assert.AreEqual(3, request.Parameters.Count);
            Assert.IsNotNull(request.Parameters.FirstOrDefault(p => p.Name == "page"));
            Assert.IsNotNull(request.Parameters.FirstOrDefault(p => p.Name == "limit"));
            Assert.IsNotNull(request.Parameters.FirstOrDefault(p => p.Name == "sort"));
        }

        [TestMethod]
        public void AddQueryParameters_Values_Converted_To_String()
        {
            var builder = new RequestBuilder("resource");
            var request = builder.AddQueryParameters(new Dictionary<string, object>
            {
                { "page", 1 },
                { "active", true },
                { "price", 29.99m }
            }).Create();
            Assert.AreEqual("1", request.Parameters.FirstOrDefault(p => p.Name == "page")?.Value);
            Assert.AreEqual("True", request.Parameters.FirstOrDefault(p => p.Name == "active")?.Value);
            Assert.AreEqual("29.99", request.Parameters.FirstOrDefault(p => p.Name == "price")?.Value);
        }

        [TestMethod]
        public void AddQueryParameters_Skips_Null_Values()
        {
            var builder = new RequestBuilder("resource");
            var request = builder.AddQueryParameters(new Dictionary<string, object>
            {
                { "page", 1 },
                { "empty", null },
                { "sort", "asc" }
            }).Create();
            Assert.AreEqual(2, request.Parameters.Count);
            Assert.IsNull(request.Parameters.FirstOrDefault(p => p.Name == "empty"));
        }

        [TestMethod]
        public void AddQueryParameters_Replaces_Existing()
        {
            var builder = new RequestBuilder("resource");
            builder.AddQueryParameter("page", 1);
            var request = builder.AddQueryParameters(new Dictionary<string, object>
            {
                { "page", 2 },
                { "limit", 50 }
            }).Create();
            var matchingParams = request.Parameters.Where(p => p.Name == "page").ToList();
            Assert.AreEqual(1, matchingParams.Count);
            Assert.AreEqual("2", matchingParams[0].Value);
            Assert.AreEqual(2, request.Parameters.Count);
        }

        [TestMethod]
        public void AddQueryParameters_Case_Insensitive_Replacement()
        {
            var builder = new RequestBuilder("resource");
            builder.AddQueryParameter("Page", 1);
            var request = builder.AddQueryParameters(new Dictionary<string, object>
            {
                { "page", 2 }
            }).Create();
            var matchingParams = request.Parameters.Where(p =>
                string.Equals(p.Name, "page", StringComparison.InvariantCultureIgnoreCase)).ToList();
            Assert.AreEqual(1, matchingParams.Count);
            Assert.AreEqual("2", matchingParams[0].Value);
        }

        [TestMethod]
        public void AddQueryParameters_Returns_Builder_For_Chaining()
        {
            var builder = new RequestBuilder("resource");
            var result = builder.AddQueryParameters(new Dictionary<string, object> { { "test", "value" } });
            Assert.AreSame(builder, result);
        }

        // --- AddUrlSegment ---
        [TestMethod]
        public void AddUrlSegment_Null_Name_Throws()
        {
            var builder = new RequestBuilder("resource");
            Assert.ThrowsException<ArgumentNullException>(() => builder.AddUrlSegment(null, "value"));
        }

        [TestMethod]
        public void AddUrlSegment_Empty_Name_Throws()
        {
            var builder = new RequestBuilder("resource");
            Assert.ThrowsException<ArgumentNullException>(() => builder.AddUrlSegment("", "value"));
        }

        [TestMethod]
        public void AddUrlSegment_Null_Value_Throws()
        {
            var builder = new RequestBuilder("resource");
            Assert.ThrowsException<ArgumentNullException>(() => builder.AddUrlSegment("id", null));
        }

        [TestMethod]
        public void AddUrlSegment_Adds_Url_Segment()
        {
            var builder = new RequestBuilder("users/{id}");
            var request = builder.AddUrlSegment("id", "123").Create();
            var param = request.Parameters.FirstOrDefault(p => p.Name == "id");
            Assert.IsNotNull(param);
            Assert.AreEqual("123", param.Value);
            Assert.AreEqual(ParameterType.UrlSegment, param.Type);
        }

        [TestMethod]
        public void AddUrlSegment_Replaces_Existing()
        {
            var builder = new RequestBuilder("users/{id}");
            var request = builder
                .AddUrlSegment("id", "123")
                .AddUrlSegment("id", "456")
                .Create();
            var matchingParams = request.Parameters.Where(p => p.Name == "id").ToList();
            Assert.AreEqual(1, matchingParams.Count);
            Assert.AreEqual("456", matchingParams[0].Value);
        }

        [TestMethod]
        public void AddUrlSegment_Case_Insensitive_Replacement()
        {
            var builder = new RequestBuilder("users/{id}");
            var request = builder
                .AddUrlSegment("Id", "123")
                .AddUrlSegment("id", "456")
                .Create();
            var matchingParams = request.Parameters.Where(p =>
                string.Equals(p.Name, "id", StringComparison.InvariantCultureIgnoreCase)).ToList();
            Assert.AreEqual(1, matchingParams.Count);
            Assert.AreEqual("456", matchingParams[0].Value);
        }

        [TestMethod]
        public void AddUrlSegment_Returns_Builder_For_Chaining()
        {
            var builder = new RequestBuilder("users/{id}");
            var result = builder.AddUrlSegment("id", "123");
            Assert.AreSame(builder, result);
        }

        // --- Fluent Chaining Integration ---
        [TestMethod]
        public void FluentChaining_Mixed_Methods()
        {
            var request = new RestRequest().WithBuilder("users/{id}/posts")
                .AddUrlSegment("id", "123")
                .AddQueryParameter("page", 1)
                .AddQueryParameter("limit", 50)
                .SetMethod(Method.Get)
                .Create();

            Assert.AreEqual(Method.Get, request.Method);
            Assert.AreEqual(3, request.Parameters.Count);
            Assert.IsNotNull(request.Parameters.FirstOrDefault(p => p.Name == "id" && p.Type == ParameterType.UrlSegment));
            Assert.IsNotNull(request.Parameters.FirstOrDefault(p => p.Name == "page" && p.Type == ParameterType.QueryString));
            Assert.IsNotNull(request.Parameters.FirstOrDefault(p => p.Name == "limit" && p.Type == ParameterType.QueryString));
        }

        [TestMethod]
        public void FluentChaining_With_Bulk_Query_Parameters()
        {
            var request = new RestRequest().WithBuilder("users/{id}/posts")
                .AddUrlSegment("id", "123")
                .AddQueryParameters(new Dictionary<string, object>
                {
                    { "page", 1 },
                    { "sort", "desc" }
                })
                .Create();

            Assert.AreEqual(3, request.Parameters.Count);
            Assert.AreEqual("123", request.Parameters.FirstOrDefault(p => p.Name == "id")?.Value);
            Assert.AreEqual("1", request.Parameters.FirstOrDefault(p => p.Name == "page")?.Value);
            Assert.AreEqual("desc", request.Parameters.FirstOrDefault(p => p.Name == "sort")?.Value);
        }

        // --- Authentication Methods ---
        [TestMethod]
        public void WithBearerToken_Null_Throws()
        {
            var builder = new RequestBuilder("resource");
            Assert.ThrowsException<ArgumentNullException>(() => builder.WithBearerToken(null));
        }

        [TestMethod]
        public void WithBearerToken_Empty_Throws()
        {
            var builder = new RequestBuilder("resource");
            Assert.ThrowsException<ArgumentNullException>(() => builder.WithBearerToken(""));
        }

        [TestMethod]
        public void WithBearerToken_Adds_Authorization_Header()
        {
            var builder = new RequestBuilder("resource");
            var token = "mySecretToken123";
            var request = builder.WithBearerToken(token).Create();
            var authHeader = request.Parameters.FirstOrDefault(p => p.Name == "Authorization" && p.Type == ParameterType.HttpHeader);
            Assert.IsNotNull(authHeader);
            Assert.AreEqual($"Bearer {token}", authHeader.Value);
        }

        [TestMethod]
        public void WithBearerToken_Replaces_Existing_Authorization()
        {
            var builder = new RequestBuilder("resource");
            var request = builder
                .WithBearerToken("oldToken")
                .WithBearerToken("newToken")
                .Create();
            var authHeaders = request.Parameters.Where(p => p.Name == "Authorization" && p.Type == ParameterType.HttpHeader).ToList();
            Assert.AreEqual(1, authHeaders.Count);
            Assert.AreEqual("Bearer newToken", authHeaders[0].Value);
        }

        [TestMethod]
        public void WithBearerToken_Returns_Builder_For_Chaining()
        {
            var builder = new RequestBuilder("resource");
            var result = builder.WithBearerToken("token");
            Assert.AreSame(builder, result);
        }

        [TestMethod]
        public void WithBasicAuth_Null_Username_Throws()
        {
            var builder = new RequestBuilder("resource");
            Assert.ThrowsException<ArgumentNullException>(() => builder.WithBasicAuth(null, "password"));
        }

        [TestMethod]
        public void WithBasicAuth_Empty_Username_Throws()
        {
            var builder = new RequestBuilder("resource");
            Assert.ThrowsException<ArgumentNullException>(() => builder.WithBasicAuth("", "password"));
        }

        [TestMethod]
        public void WithBasicAuth_Null_Password_Throws()
        {
            var builder = new RequestBuilder("resource");
            Assert.ThrowsException<ArgumentNullException>(() => builder.WithBasicAuth("user", null));
        }

        [TestMethod]
        public void WithBasicAuth_Empty_Password_Throws()
        {
            var builder = new RequestBuilder("resource");
            Assert.ThrowsException<ArgumentNullException>(() => builder.WithBasicAuth("user", ""));
        }

        [TestMethod]
        public void WithBasicAuth_Adds_Authorization_Header()
        {
            var builder = new RequestBuilder("resource");
            var username = "testuser";
            var password = "testpass";
            var request = builder.WithBasicAuth(username, password).Create();
            var authHeader = request.Parameters.FirstOrDefault(p => p.Name == "Authorization" && p.Type == ParameterType.HttpHeader);
            Assert.IsNotNull(authHeader);

            // Verify it starts with "Basic "
            var authValue = authHeader.Value as string;
            Assert.IsTrue(authValue.StartsWith("Basic "));

            // Decode and verify the credentials
            var encodedPart = authValue.Substring(6);
            var decoded = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(encodedPart));
            Assert.AreEqual($"{username}:{password}", decoded);
        }

        [TestMethod]
        public void WithBasicAuth_Replaces_Existing_Authorization()
        {
            var builder = new RequestBuilder("resource");
            var request = builder
                .WithBasicAuth("user1", "pass1")
                .WithBasicAuth("user2", "pass2")
                .Create();
            var authHeaders = request.Parameters.Where(p => p.Name == "Authorization" && p.Type == ParameterType.HttpHeader).ToList();
            Assert.AreEqual(1, authHeaders.Count);

            // Verify the second auth is used
            var authValue = authHeaders[0].Value as string;
            var encodedPart = authValue.Substring(6);
            var decoded = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(encodedPart));
            Assert.AreEqual("user2:pass2", decoded);
        }

        [TestMethod]
        public void WithBasicAuth_Returns_Builder_For_Chaining()
        {
            var builder = new RequestBuilder("resource");
            var result = builder.WithBasicAuth("user", "pass");
            Assert.AreSame(builder, result);
        }

        [TestMethod]
        public void WithApiKey_Null_Key_Throws()
        {
            var builder = new RequestBuilder("resource");
            Assert.ThrowsException<ArgumentNullException>(() => builder.WithApiKey(null));
        }

        [TestMethod]
        public void WithApiKey_Empty_Key_Throws()
        {
            var builder = new RequestBuilder("resource");
            Assert.ThrowsException<ArgumentNullException>(() => builder.WithApiKey(""));
        }

        [TestMethod]
        public void WithApiKey_Null_HeaderName_Throws()
        {
            var builder = new RequestBuilder("resource");
            Assert.ThrowsException<ArgumentNullException>(() => builder.WithApiKey("key123", null));
        }

        [TestMethod]
        public void WithApiKey_Empty_HeaderName_Throws()
        {
            var builder = new RequestBuilder("resource");
            Assert.ThrowsException<ArgumentNullException>(() => builder.WithApiKey("key123", ""));
        }

        [TestMethod]
        public void WithApiKey_Adds_Default_Header()
        {
            var builder = new RequestBuilder("resource");
            var apiKey = "myApiKey123";
            var request = builder.WithApiKey(apiKey).Create();
            var apiKeyHeader = request.Parameters.FirstOrDefault(p => p.Name == "X-API-Key" && p.Type == ParameterType.HttpHeader);
            Assert.IsNotNull(apiKeyHeader);
            Assert.AreEqual(apiKey, apiKeyHeader.Value);
        }

        [TestMethod]
        public void WithApiKey_Adds_Custom_Header()
        {
            var builder = new RequestBuilder("resource");
            var apiKey = "myApiKey123";
            var headerName = "X-Custom-Api-Key";
            var request = builder.WithApiKey(apiKey, headerName).Create();
            var apiKeyHeader = request.Parameters.FirstOrDefault(p => p.Name == headerName && p.Type == ParameterType.HttpHeader);
            Assert.IsNotNull(apiKeyHeader);
            Assert.AreEqual(apiKey, apiKeyHeader.Value);
        }

        [TestMethod]
        public void WithApiKey_Replaces_Existing_Key()
        {
            var builder = new RequestBuilder("resource");
            var request = builder
                .WithApiKey("oldKey")
                .WithApiKey("newKey")
                .Create();
            var apiKeyHeaders = request.Parameters.Where(p => p.Name == "X-API-Key" && p.Type == ParameterType.HttpHeader).ToList();
            Assert.AreEqual(1, apiKeyHeaders.Count);
            Assert.AreEqual("newKey", apiKeyHeaders[0].Value);
        }

        [TestMethod]
        public void WithApiKey_Returns_Builder_For_Chaining()
        {
            var builder = new RequestBuilder("resource");
            var result = builder.WithApiKey("key");
            Assert.AreSame(builder, result);
        }

        [TestMethod]
        public void WithOAuth2_Null_Throws()
        {
            var builder = new RequestBuilder("resource");
            Assert.ThrowsException<ArgumentNullException>(() => builder.WithOAuth2(null));
        }

        [TestMethod]
        public void WithOAuth2_Empty_Throws()
        {
            var builder = new RequestBuilder("resource");
            Assert.ThrowsException<ArgumentNullException>(() => builder.WithOAuth2(""));
        }

        [TestMethod]
        public void WithOAuth2_Adds_Authorization_Header()
        {
            var builder = new RequestBuilder("resource");
            var accessToken = "oauth2AccessToken123";
            var request = builder.WithOAuth2(accessToken).Create();
            var authHeader = request.Parameters.FirstOrDefault(p => p.Name == "Authorization" && p.Type == ParameterType.HttpHeader);
            Assert.IsNotNull(authHeader);
            Assert.AreEqual($"Bearer {accessToken}", authHeader.Value);
        }

        [TestMethod]
        public void WithOAuth2_Replaces_Existing_Authorization()
        {
            var builder = new RequestBuilder("resource");
            var request = builder
                .WithOAuth2("oldToken")
                .WithOAuth2("newToken")
                .Create();
            var authHeaders = request.Parameters.Where(p => p.Name == "Authorization" && p.Type == ParameterType.HttpHeader).ToList();
            Assert.AreEqual(1, authHeaders.Count);
            Assert.AreEqual("Bearer newToken", authHeaders[0].Value);
        }

        [TestMethod]
        public void WithOAuth2_Returns_Builder_For_Chaining()
        {
            var builder = new RequestBuilder("resource");
            var result = builder.WithOAuth2("token");
            Assert.AreSame(builder, result);
        }

        [TestMethod]
        public void Authentication_Chaining_With_Other_Methods()
        {
            var request = new RestRequest().WithBuilder("api/users/{id}")
                .AddUrlSegment("id", "123")
                .WithBearerToken("myToken")
                .AddQueryParameter("page", 1)
                .SetMethod(Method.Get)
                .Create();

            Assert.AreEqual(Method.Get, request.Method);
            var authHeader = request.Parameters.FirstOrDefault(p => p.Name == "Authorization" && p.Type == ParameterType.HttpHeader);
            Assert.IsNotNull(authHeader);
            Assert.AreEqual("Bearer myToken", authHeader.Value);
            Assert.IsNotNull(request.Parameters.FirstOrDefault(p => p.Name == "id" && p.Type == ParameterType.UrlSegment));
            Assert.IsNotNull(request.Parameters.FirstOrDefault(p => p.Name == "page" && p.Type == ParameterType.QueryString));
        }

        [TestMethod]
        public void Authentication_Bearer_Replaces_Basic()
        {
            var builder = new RequestBuilder("resource");
            var request = builder
                .WithBasicAuth("user", "pass")
                .WithBearerToken("token")
                .Create();
            var authHeaders = request.Parameters.Where(p => p.Name == "Authorization" && p.Type == ParameterType.HttpHeader).ToList();
            Assert.AreEqual(1, authHeaders.Count);
            Assert.AreEqual("Bearer token", authHeaders[0].Value);
        }

        [TestMethod]
        public void Authentication_Basic_Replaces_Bearer()
        {
            var builder = new RequestBuilder("resource");
            var request = builder
                .WithBearerToken("token")
                .WithBasicAuth("user", "pass")
                .Create();
            var authHeaders = request.Parameters.Where(p => p.Name == "Authorization" && p.Type == ParameterType.HttpHeader).ToList();
            Assert.AreEqual(1, authHeaders.Count);
            var authValue = authHeaders[0].Value as string;
            Assert.IsTrue(authValue.StartsWith("Basic "));
        }
    }
}
