# Unit Tests Instructions

## Overview

RestSharp.RequestBuilder uses MSTest as the unit testing framework with comprehensive coverage across all public methods and edge cases. This document specifies testing strategy, patterns, coverage goals, and recommendations for test reliability and improvement.

## Test Framework and Setup

### Test Project
- **Project**: tests/RestSharp.RequestBuilder.UnitTests/RestSharp.RequestBuilder.UnitTests.csproj
- **Framework**: MSTest (Microsoft.VisualStudio.TestTools.UnitTesting)
- **Test Runner**: dotnet test
- **Target Frameworks**: net8.0 (primary), netcoreapp2.1 (legacy)

### Test Class Structure
`csharp
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

    [TestMethod]
    public void MethodName_Condition_ExpectedOutcome()
    {
        // Arrange, Act, Assert
    }
}
`

### Lifecycle
- **TestInitialize**: Runs before each test; creates fresh _builder instance
- **TestCleanup**: Runs after each test; cleans up resources
- **Isolation**: Each test is independent; no shared state across tests

## Test Coverage by Category

### 1. Constructor Tests
**Purpose**: Verify initialization and argument validation
**Coverage**:
- null/empty string resource throws ArgumentNullException
- Uri resource validation
- Method parameter handling
- DataFormat parameter handling
- Default values (Method.Get, DataFormat.Json, TimeSpan.FromSeconds(30))

**Example**:
`csharp
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
`

### 2. Body Operation Tests
**Purpose**: Verify request body handling and format configuration
**Coverage**:
- AddBody() with null throws
- AddBody<T>() with null throws
- AddJsonBody<T>() sets format to JSON
- AddXmlBody<T>() sets format to XML
- AddFormUrlEncodedBody() with dictionary data
- Empty dictionary handling

**Example**:
`csharp
[TestMethod]
public void AddJsonBody_Generic_SetsFormatAndBody()
{
    var user = new { Id = 1, Name = "John" };
    var request = _builder.AddJsonBody(user).Create();
    Assert.AreEqual(DataFormat.Json, request.RequestFormat);
    Assert.IsNotNull(request.Body);
}
`

### 3. File Operation Tests
**Purpose**: Verify file attachment handling from multiple sources
**Coverage**:
- AddFile() with disk path
- AddFiles() with multiple files
- AddFileBytes() with byte array
- AddFileStream() with stream
- Null/empty parameter validation
- Stream content type specification
- Mixed file sources in single request
- File count and metadata verification

**Example**:
`csharp
[TestMethod]
public void AddFile_Valid_Path_Adds_File()
{
    var tempPath = Path.GetTempFileName();
    try
    {
        File.WriteAllText(tempPath, "content");
        var request = _builder.AddFile("upload", tempPath).Create();
        Assert.AreEqual(1, request.Files.Count);
    }
    finally
    {
        File.Delete(tempPath);
    }
}

[TestMethod]
public void Mixed_File_Sources_All_Added()
{
    var request = _builder
        .AddFile("disk", tempPath)
        .AddFileBytes("bytes", new byte[] { 1, 2, 3 }, "data.bin")
        .AddFileStream("stream", stream, "file.bin")
        .Create();
    
    Assert.AreEqual(3, request.Files.Count);
}
`

### 4. Header Operation Tests
**Purpose**: Verify header management with case-insensitive semantics
**Coverage**:
- AddHeader() adds single header
- AddHeader() replaces duplicate (case-insensitive)
- AddHeader() skips replacement if value unchanged
- AddHeaders() adds dictionary of headers
- AddHeaders() deduplicates case-insensitively
- RemoveHeader() removes by name
- RemoveHeaders() clears all
- HeaderCount property accuracy
- Null/empty validation

**Example**:
`csharp
[TestMethod]
public void AddHeader_DuplicateNameCaseInsensitive_Replaces()
{
    _builder.AddHeader("Content-Type", "application/json");
    _builder.AddHeader("content-type", "application/xml");
    var request = _builder.Create();
    
    Assert.AreEqual(1, request.Headers.Count);
    Assert.AreEqual("application/xml", request.Headers[0].Value);
}

[TestMethod]
public void RemoveHeader_RemovesHeaderByName()
{
    _builder.AddHeader("X-Custom", "value");
    _builder.RemoveHeader("X-Custom");
    var request = _builder.Create();
    
    Assert.AreEqual(0, request.Headers.Count);
}
`

### 5. Parameter Operation Tests
**Purpose**: Verify parameter handling with case-insensitive deduplication
**Coverage**:
- AddParameter() adds single parameter
- AddParameter() replaces duplicate (case-insensitive)
- AddParameters() adds array of parameters
- AddParameters() deduplicates within array and existing
- AddQueryParameter() creates QueryParameter
- AddQueryParameters() from dictionary
- AddUrlSegment() creates UrlSegmentParameter
- RemoveParameter() by name
- RemoveParameters() clears all
- Null/empty validation
- InvariantCulture string conversion
- Deduplication with mixed case names

**Example**:
`csharp
[TestMethod]
public void AddQueryParameters_WithDuplicates_Replaces()
{
    _builder.AddQueryParameter("page", 1);
    var request = _builder.AddQueryParameters(new Dictionary<string, object>
    {
        { "page", 2 },  // Should replace
        { "limit", 10 }
    }).Create();
    
    Assert.AreEqual(2, request.Parameters.Count);
    var pageParam = request.Parameters.FirstOrDefault(p => p.Name == "page");
    Assert.AreEqual("2", pageParam?.Value?.ToString());
}

[TestMethod]
public void AddParameters_WithinArrayDuplicates_LastWins()
{
    var parameters = new Parameter[]
    {
        new QueryParameter("key", "value1"),
        new QueryParameter("KEY", "value2")  // Case-insensitive duplicate
    };
    
    var request = _builder.AddParameters(parameters).Create();
    var param = request.Parameters.FirstOrDefault(p => p.Name == "key");
    Assert.AreEqual("value2", param?.Value);  // Last occurrence wins
}
`

### 6. Cookie Operation Tests
**Purpose**: Verify cookie handling with HashSet-based deduplication
**Coverage**:
- AddCookie() adds single cookie
- AddCookie() deduplicates case-insensitively
- AddCookie() with path and domain
- RemoveCookies() clears all
- Cookie container population
- Null/empty validation

**Example**:
`csharp
[TestMethod]
public void AddCookie_MultipleCookies_AllAdded()
{
    _builder
        .AddCookie("sessionId", "abc123", "/", "example.com")
        .AddCookie("userId", "user456", "/", "example.com");
    
    var request = _builder.Create();
    Assert.AreEqual(2, request.CookieContainer.Count);
}
`

### 7. Authentication Tests
**Purpose**: Verify authentication header generation
**Coverage**:
- WithBearerToken() formats "Bearer {token}"
- WithBasicAuth() Base64-encodes "username:password"
- WithApiKey() sets custom header (default X-API-Key)
- WithOAuth2() delegates to WithBearerToken()
- Null/empty validation for all
- Header replacement behavior

**Example**:
`csharp
[TestMethod]
public void WithBearerToken_Valid_Token_AddsAuthorizationHeader()
{
    var request = _builder.WithBearerToken("mytoken").Create();
    var authHeader = request.Headers.FirstOrDefault(h => h.Name == "Authorization");
    
    Assert.IsNotNull(authHeader);
    Assert.AreEqual("Bearer mytoken", authHeader.Value);
}

[TestMethod]
public void WithBasicAuth_Username_Password_EncodesCreds()
{
    var request = _builder.WithBasicAuth("user", "pass").Create();
    var authHeader = request.Headers.FirstOrDefault(h => h.Name == "Authorization");
    
    // "user:pass" Base64-encoded = "dXNlcjpwYXNz"
    Assert.IsNotNull(authHeader);
    Assert.IsTrue(authHeader.Value.StartsWith("Basic "));
}
`

### 8. Content Negotiation Tests
**Purpose**: Verify HTTP header convenience methods
**Coverage**:
- WithAccept() with custom media type
- WithAcceptJson() sets "application/json"
- WithAcceptXml() sets "application/xml"
- WithContentType() sets Content-Type header
- WithUserAgent() sets User-Agent
- WithAuthorization() with scheme and value
- WithIfMatch() and WithIfNoneMatch() for ETags
- WithReferer() sets Referer header
- WithOrigin() sets Origin header
- Null/empty validation for all

**Example**:
`csharp
[TestMethod]
public void WithAcceptJson_SetsAcceptHeader()
{
    var request = _builder.WithAcceptJson().Create();
    var acceptHeader = request.Headers.FirstOrDefault(h => h.Name == "Accept");
    
    Assert.AreEqual("application/json", acceptHeader?.Value);
}
`

### 9. Format and Method Tests
**Purpose**: Verify request configuration
**Coverage**:
- SetFormat() changes DataFormat
- SetMethod() changes HTTP method
- SetTimeout() sets request timeout
- Default values verification
- Timeout bounds checking

**Example**:
`csharp
[TestMethod]
public void SetFormat_ChangesDataFormat()
{
    var request = _builder.SetFormat(DataFormat.Xml).Create();
    Assert.AreEqual(DataFormat.Xml, request.RequestFormat);
}

[TestMethod]
public void SetTimeout_ValidTimespan_SetsTimeout()
{
    var timeout = TimeSpan.FromSeconds(60);
    var request = _builder.SetTimeout(timeout).Create();
    Assert.AreEqual(timeout, request.Timeout);
}
`

### 10. Fluent Chaining Tests
**Purpose**: Verify that all methods support method chaining
**Coverage**:
- Each public method returns IRequestBuilder (or derived)
- Return value is same instance (this)
- Multiple operations chainable
- Order-independence of most operations

**Example**:
`csharp
[TestMethod]
public void AddHeader_Returns_Builder_For_Chaining()
{
    var result = _builder.AddHeader("X-Custom", "value");
    Assert.AreSame(_builder, result);
}

[TestMethod]
public void Fluent_Chain_Multiple_Operations()
{
    var request = _builder
        .SetMethod(Method.Post)
        .WithBearerToken("token")
        .AddQueryParameter("id", 123)
        .AddHeader("X-Custom", "value")
        .SetFormat(DataFormat.Json)
        .Create();
    
    Assert.IsNotNull(request);
    Assert.AreEqual(Method.Post, request.Method);
}
`

## Test Statistics

- **Total Test Methods**: 100+
- **Test File Size**: ~2219 lines
- **Coverage Focus**: All public methods and error conditions
- **Average Per Method**: 3-5 tests (positive + null + edge cases)

## Test Patterns

### Arrange-Act-Assert (AAA)
`csharp
[TestMethod]
public void SampleTest()
{
    // Arrange - Set up test data and builder state
    var builder = new RequestBuilder("resource");
    
    // Act - Call the method being tested
    var request = builder.AddHeader("Name", "Value").Create();
    
    // Assert - Verify the result
    Assert.AreEqual(1, request.Headers.Count);
}
`

### Observable Behavior Testing
Tests inspect Create() output, never private fields:
`csharp
// GOOD: Test observable behavior
var request = builder.AddQueryParameter("id", "123").Create();
Assert.AreEqual("123", request.Parameters[0].Value);

// AVOID: Inspecting private fields
var parametersField = typeof(RequestBuilder).GetField("_parameters", ...);
`

### Exception Testing
`csharp
[TestMethod]
public void MethodName_InvalidInput_ThrowsException()
{
    Assert.ThrowsException<ArgumentNullException>(() => 
        _builder.AddHeader(null, "value")
    );
}
`

## Coverage Gaps & Recommendations

### Current Coverage
- ✅ All public methods tested
- ✅ Null/empty validation
- ✅ Case-insensitive deduplication
- ✅ Fluent chaining
- ✅ File handling from multiple sources
- ✅ Authentication helpers
- ✅ HTTP header helpers

### Potential Improvements

1. **Concurrent Modification Testing**
   - Test thread-safety if needed
   - Current design is single-threaded (acceptable)

2. **Performance Testing**
   - Benchmark AddParameters() with large arrays
   - Measure parameter lookup efficiency

3. **Integration Testing**
   - Test with actual RestSharp execution (beyond builder)
   - Verify parameter order preservation
   - Test file stream lifecycle management

4. **Boundary Testing**
   - Very large parameter/header counts
   - Very long string values
   - Special characters in parameter names/values

5. **Culture-Specific Testing**
   - InvariantCulture behavior with various locale settings
   - Numeric conversion edge cases

## Running Tests

### Run All Tests
`powershell
dotnet test tests/RestSharp.RequestBuilder.UnitTests/RestSharp.RequestBuilder.UnitTests.csproj
`

### Run Specific Test Method
`powershell
dotnet test --filter "AddHeader_ValidInput_ReturnsBuilder"
`

### Run with Coverage
`powershell
dotnet test /p:CollectCoverage=true /p:CoverageFormat=cobertura
`

### Run in Visual Studio
- Test Explorer (Ctrl+E, T)
- Select test and Run

## Best Practices for New Tests

1. **Follow Naming Convention**: MethodName_Condition_ExpectedOutcome
2. **Use AAA Pattern**: Arrange, Act, Assert clearly separated
3. **Test One Thing**: Each test verifies single behavior
4. **Independent Tests**: No dependencies between tests
5. **Observable Behavior**: Inspect Create() output, not internals
6. **Descriptive Assertions**: Use Assert.AreEqual with descriptive messages if needed
7. **Reuse Builder**: Use _builder instance for consistency
8. **Handle Resources**: Clean up temp files, streams in finally blocks

## Troubleshooting

### Test Failures
1. Check isolation: Tests should not depend on order
2. Verify _builder setup in TestInitialize
3. Ensure Create() is called before asserting
4. Check for case sensitivity issues in parameter names

### Flaky Tests
1. Temp file cleanup can fail on Windows; use Path.GetTempFileName()
2. Stream lifecycle issues; keep streams open until Create() is called
3. Culture-dependent behavior; use InvariantCulture

---

**Last Updated**: December 2025
**Test Count**: 100+ methods
**Framework**: MSTest
**Maintenance**: Add tests for every new public method
