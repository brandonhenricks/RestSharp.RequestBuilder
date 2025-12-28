# Code Exemplars Instructions

## Overview

This document highlights exemplary code patterns, classes, and methods within RestSharp.RequestBuilder that demonstrate clean code, SOLID principles, and testability best practices.

## Exemplary Classes

### RequestBuilder - Fluent Builder Pattern Example
**File**: src/RestSharp.RequestBuilder/RequestBuilder.cs
**Why Exemplary**:
- Single Responsibility: Focuses solely on accumulating REST request configuration
- Immutability: All state is private; no public fields or properties that expose mutable state
- Sealed Class: Prevents inheritance, ensuring invariants are maintained
- Chainable API: Every method returns IRequestBuilder for expressive syntax
- Null Safety: Validates all input arguments before use

**Key Pattern**:
`csharp
public sealed class RequestBuilder : IRequestBuilder
{
    private readonly Dictionary<string, string> _headers;
    private readonly List<Parameter> _parameters;
    
    public IRequestBuilder AddHeader(string name, string value)
    {
        // Validation
        if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
        
        // Mutation
        _headers[name] = value;
        
        // Return for chaining
        return this;
    }
}
`

### CookieValueComparer - Custom Equality Implementation
**File**: src/RestSharp.RequestBuilder/Models/CookieValueComparer.cs
**Why Exemplary**:
- Implements IEqualityComparer<T> correctly with both GetHashCode() and Equals()
- Case-insensitive comparison respects HTTP cookie semantics
- Enables efficient HashSet-based deduplication
- Immutable implementation (no state mutation)

**Key Pattern**:
`csharp
public class CookieValueComparer : IEqualityComparer<CookieValue>
{
    public bool Equals(CookieValue x, CookieValue y) =>
        string.Equals(x?.Name, y?.Name, StringComparison.InvariantCultureIgnoreCase);

    public int GetHashCode(CookieValue obj) =>
        obj?.Name?.ToLowerInvariant().GetHashCode() ?? 0;
}
`

### FileAttachment Polymorphism - Sealed Subclasses
**File**: src/RestSharp.RequestBuilder/Models/FileAttachment.cs
**Why Exemplary**:
- Abstract base class with three sealed concrete implementations
- Pattern matching support in Create() method
- Type-safe handling without casting
- Clear separation of concerns: Path-based, Bytes-based, Stream-based

**Key Pattern**:
`csharp
public abstract class FileAttachment
{
    public string Name { get; }
    public string ContentType { get; }
    protected FileAttachment(string name, string contentType) { }
}

public sealed class PathFileAttachment : FileAttachment
{
    public string Path { get; }
    public PathFileAttachment(string name, string path, string contentType) : base(name, contentType) { }
}

// In RequestBuilder.Create():
foreach (var file in _files)
{
    switch (file)
    {
        case PathFileAttachment pathFile:
            request.AddFile(pathFile.Name, pathFile.Path, pathFile.ContentType);
            break;
        // ... other cases
    }
}
`

## Exemplary Methods

### AddParameters() - Two-Pass Deduplication Algorithm
**File**: src/RestSharp.RequestBuilder/RequestBuilder.cs (lines ~420-460)
**Why Exemplary**:
- O(n+m) time complexity using Dictionary-based lookup
- Handles duplicates within both existing and input arrays
- Respects case-insensitive parameter identity (HTTP semantics)
- Clear, maintainable algorithm with minimal overhead

**Key Pattern**:
`csharp
public IRequestBuilder AddParameters(Parameter[] parameters)
{
    if (parameters == null || parameters.Length == 0)
        return this;

    // Build lookup of existing parameters by name
    var existingLookup = new Dictionary<string, int>(
        StringComparer.InvariantCultureIgnoreCase);

    for (int i = 0; i < _parameters.Count; i++)
    {
        if (!existingLookup.ContainsKey(_parameters[i].Name))
        {
            existingLookup[_parameters[i].Name] = i;
        }
    }

    // Process new parameters, respecting duplicates
    foreach (var parameter in parameters)
    {
        if (parameter == null) continue;

        if (existingLookup.TryGetValue(parameter.Name, out int index))
        {
            _parameters[index] = parameter;
        }
        else
        {
            _parameters.Add(parameter);
            existingLookup[parameter.Name] = _parameters.Count - 1;
        }
    }

    return this;
}
`

### WithBearerToken() - Authentication Helper Pattern
**File**: src/RestSharp.RequestBuilder/RequestBuilder.cs
**Why Exemplary**:
- Simple delegation to AddHeader() with standard HTTP formatting
- Null/empty argument validation
- Maintainable and extensible pattern for other auth schemes

**Key Pattern**:
`csharp
public IRequestBuilder WithBearerToken(string token)
{
    if (string.IsNullOrEmpty(token))
        throw new ArgumentNullException(nameof(token));

    return AddHeader("Authorization", $"Bearer {token}");
}
`

### AddQueryParameters() - Bulk Operation Pattern
**File**: src/RestSharp.RequestBuilder/RequestBuilder.cs
**Why Exemplary**:
- Handles null/empty collection gracefully
- Skips null values without error
- Uses InvariantCulture for culture-independent string conversion
- Delegates to single-item AddParameter() for consistent deduplication

**Key Pattern**:
`csharp
public IRequestBuilder AddQueryParameters(IDictionary<string, object> parameters)
{
    if (parameters == null || parameters.Count == 0)
        return this;

    foreach (var kvp in parameters)
    {
        if (kvp.Value is null) continue;

        var stringValue = Convert.ToString(kvp.Value, 
            System.Globalization.CultureInfo.InvariantCulture);
        var parameter = new QueryParameter(kvp.Key, stringValue);
        AddParameter(parameter);
    }

    return this;
}
`

### Create() - Pattern-Matched Assembly
**File**: src/RestSharp.RequestBuilder/RequestBuilder.cs
**Why Exemplary**:
- Pattern matching simplifies polymorphic assembly
- Comprehensive assembly of all request components
- Clear, readable switch expression
- Handles optional fields (body, timeout, files) gracefully

**Key Pattern**:
`csharp
public RestRequest Create()
{
    var request = new RestRequest(_resource, _method);

    foreach (var param in _parameters)
        request.AddParameter(param);

    if (_body != null)
        request.AddBody(_body);

    request.RequestFormat = _dataFormat;

    foreach (var header in _headers)
        request.AddHeader(header.Key, header.Value);

    foreach (var cookie in _cookieValues)
        request.AddCookie(cookie.Name, cookie.Value, cookie.Path, cookie.Domain);

    foreach (var file in _files)
    {
        switch (file)
        {
            case PathFileAttachment pathFile:
                request.AddFile(pathFile.Name, pathFile.Path, pathFile.ContentType);
                break;
            case ByteFileAttachment byteFile:
                request.AddFile(byteFile.Name, byteFile.Bytes, byteFile.FileName, byteFile.ContentType);
                break;
            case StreamFileAttachment streamFile:
                request.AddFile(streamFile.Name, () => streamFile.Stream, streamFile.FileName, streamFile.ContentType);
                break;
        }
    }

    if (_timeOut != null)
        request.Timeout = _timeOut;

    return request;
}
`

## SOLID Principles Demonstrated

### Single Responsibility Principle (SRP)
- RequestBuilder: Request assembly only, not HTTP execution
- CookieValue: Cookie data holder only
- CookieValueComparer: Equality comparison only
- FileAttachment subclasses: File source abstraction only

### Open/Closed Principle (OCP)
- IRequestBuilder interface allows new implementations
- FileAttachment abstract base allows new file source types
- Extension methods (WithBuilder) extend without modifying RestRequest

### Liskov Substitution Principle (LSP)
- All FileAttachment subclasses are substitutable for FileAttachment base class
- RequestBuilder implements IRequestBuilder fully without surprises
- Subclass contract is always honored

### Interface Segregation Principle (ISP)
- IRequestBuilder is cohesive; no unused methods
- CookieValueComparer implements only IEqualityComparer<CookieValue>
- FileAttachment and subclasses use minimal inheritance

### Dependency Inversion Principle (DIP)
- RequestBuilder depends on IRequestBuilder abstraction (via public implementation)
- RestRequestExtensions depend on RestRequest abstraction, not concrete clients
- Parameter and FileAttachment types decouple builder from RestSharp internals

## Testability Patterns

### Observable Behavior Verification
**Pattern**: Tests call Create() and inspect RestRequest properties
`csharp
var request = builder
    .AddQueryParameter("page", 1)
    .AddHeader("Authorization", "Bearer token")
    .Create();

Assert.AreEqual("page", request.Parameters[0].Name);
Assert.AreEqual("Bearer token", request.Headers[0].Value);
`

### Immutable State Separation
**Pattern**: Tests can call Create() multiple times safely
`csharp
var request1 = builder.AddHeader("X-Custom", "value1").Create();
var request2 = builder.AddHeader("X-Custom", "value2").Create();
// Both requests are independent
`

## Code Quality Indicators

- **No Code Duplication**: Bulk operations delegate to single-item operations
- **Early Validation**: All public methods validate arguments before use
- **Culture-Independent**: InvariantCulture used for string conversions
- **Case-Insensitive Semantics**: Headers, parameters, and cookies respect HTTP conventions
- **Clear Naming**: Methods use verb phrases (AddHeader, WithBearerToken, RemoveParameters)
- **Comprehensive Documentation**: XML docs on all public members

---

**Last Updated**: December 2025
**Focus**: Patterns for building fluent, maintainable request builders
