# Coding Standards Instructions

## Overview

This document specifies coding conventions, style guidelines, and best practices for RestSharp.RequestBuilder development. All code must adhere to these standards to maintain consistency, readability, and testability.

## Naming Conventions

### Classes
- **RequestBuilder**: Sealed implementation of IRequestBuilder
- **Suffixes**: Use descriptive suffixes (Comparer, Attachment, Value, Extensions)
- **PascalCase**: All class names use PascalCase
- **No Generic Prefixes**: Avoid "Abstract" prefix; use abstract keyword in definition

### Methods
- **Verb Phrases**: All public methods start with action verbs (Add, Remove, Set, With)
- **PascalCase**: Method names use PascalCase
- **Prefix Conventions**:
  - Add*: Adds item(s) to a collection
  - Remove*: Removes item(s) from a collection
  - Set*: Sets a configuration value
  - With*: Adds authentication or HTTP headers with fluent syntax
- **Consistency**: Mirror method names across singular/bulk operations
  - AddHeader / AddHeaders
  - AddParameter / AddParameters
  - RemoveHeader / RemoveHeaders

### Parameters
- **camelCase**: All parameter names use camelCase
- **Descriptive**: Use full words (name, value, path, fileName, contentType)
- **Abbreviation Avoidance**: Avoid abbreviations (use dataFormat, not fmt)

### Fields
- **Private Prefix Underscore**: All private fields prefixed with underscore (_headers, _parameters)
- **Readonly Where Possible**: Use readonly for immutable collections
- **Clear Intent**: Field names indicate their purpose (Dictionary<string, string> _headers)

### Properties
- **PascalCase**: Property names use PascalCase
- **Public Only When Needed**: Properties are sparse; most state is private
- **Documented**: All public properties have XML docs

## Code Organization

### File Structure
`
namespace RestSharp.RequestBuilder
{
    public sealed class RequestBuilder : IRequestBuilder
    {
        #region Private Properties
        // Private fields and internal properties

        #endregion

        #region Public Properties
        // Public properties only

        #endregion

        #region Public Constructors
        // All public constructors

        #endregion

        #region Public Methods
        // All public method implementations
        // Organized by functional category (Body, File, Header, Parameter)

        #endregion
    }
}
`

### Method Organization
Methods are organized by functional category within the Public Methods region:
1. **Body Methods**: AddBody, AddBody<T>, AddJsonBody, AddXmlBody, AddFormUrlEncodedBody
2. **File Methods**: AddFile, AddFiles, AddFileBytes, AddFileStream
3. **Header Methods**: AddHeader, AddHeaders, RemoveHeader, RemoveHeaders
4. **Parameter Methods**: AddParameter, AddParameters, AddQueryParameter, AddQueryParameters, AddUrlSegment, RemoveParameters, RemoveParameter
5. **Cookie Methods**: AddCookie, RemoveCookies
6. **Configuration Methods**: SetFormat, SetMethod, SetTimeout
7. **Authentication Methods**: WithBearerToken, WithBasicAuth, WithApiKey, WithOAuth2
8. **Content Negotiation**: WithAccept, WithAcceptJson, WithAcceptXml, WithContentType
9. **HTTP Headers**: WithUserAgent, WithAuthorization, WithIfMatch, WithIfNoneMatch, WithReferer, WithOrigin
10. **Factory Method**: Create()

## Documentation Standards

### XML Documentation Comments
All public classes, methods, and properties require XML documentation:

`csharp
/// <summary>
/// Adds a header to the request.
/// </summary>
/// <param name="name">The name of the header.</param>
/// <param name="value">The value of the header.</param>
/// <returns>The current instance of <see cref="IRequestBuilder"/>.</returns>
public IRequestBuilder AddHeader(string name, string value)
{
    // Implementation
}
`

### Guidelines
- **Summary**: One-line description of what the member does
- **Params**: Describe each parameter, including constraints (null handling, expected formats)
- **Returns**: Describe the return value; include note about fluent chaining
- **Remarks**: Optional; use for complex behavior or edge cases
- **Examples**: Use in README.md instead of XML docs
- **See Also**: Link related methods with <see cref="..."/>

### Comment Style
- **No Inline Comments**: Code should be self-documenting
- **Exceptional Cases**: Comments for non-obvious logic (e.g., deduplication algorithm)
- **Region Comments**: Only for section markers (#region, #endregion)

## Formatting and Style

### Indentation
- **Spaces Only**: 4 spaces per indentation level, never tabs
- **Braces**: Allman style (opening brace on new line for classes/methods)
- **Line Length**: Prefer ≤120 characters; break long lines after operators

### Code Blocks
`csharp
public IRequestBuilder AddHeader(string name, string value)
{
    if (string.IsNullOrEmpty(name))
    {
        throw new ArgumentNullException(nameof(name));
    }

    _headers[name] = value;
    return this;
}
`

### Spacing
- **Method Calls**: No space before parentheses (AddHeader(...), not AddHeader (...))
- **Control Structures**: Space before opening brace (if (...) {, not if(...){)
- **Type Casting**: No space after cast ((string)value, not (string) value)
- **Generic Types**: No space in angle brackets (Dictionary<string, string>, not Dictionary< string, string >)

## Argument Validation

### Validation Pattern
All public methods validate inputs early:

`csharp
public IRequestBuilder AddQueryParameter(string name, object value)
{
    if (string.IsNullOrEmpty(name))
    {
        throw new ArgumentNullException(nameof(name));
    }

    if (value is null)
    {
        throw new ArgumentNullException(nameof(value));
    }

    // Safe to proceed
}
`

### Validation Hierarchy
1. **Null Checks**: Check for null before length/content checks
2. **Empty String Checks**: Use string.IsNullOrEmpty() or string.IsNullOrWhiteSpace()
3. **Length/Size Checks**: Validate array/collection length
4. **Value Checks**: Validate acceptable value ranges

### Exception Types
- **ArgumentNullException**: For null reference types
- **ArgumentException**: For invalid string/value content
- **ArgumentOutOfRangeException**: For numeric or TimeSpan bounds

## Method Implementation Patterns

### Fluent Chaining Pattern
Every mutator must return 	his:
`csharp
public IRequestBuilder AddHeader(string name, string value)
{
    // Validate and mutate
    _headers[name] = value;
    
    // Always return for chaining
    return this;
}
`

### Deduplication Pattern
Bulk operations delegate to single-item operations:
`csharp
public IRequestBuilder AddHeaders(IDictionary<string, string> headers)
{
    foreach (var header in headers)
    {
        AddHeader(header.Key, header.Value);  // Consistent deduplication
    }
    return this;
}
`

### Case-Insensitive Comparison
Use StringComparison.InvariantCultureIgnoreCase for parameter/header identity:
`csharp
var existingIndex = _parameters.FindIndex(p =>
    string.Equals(p.Name, parameter.Name, StringComparison.InvariantCultureIgnoreCase));
`

### Culture-Independent String Conversion
Use InvariantCulture for query parameter values:
`csharp
var stringValue = Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture);
`

### Null Coalescing in Collections
Use foreach with null checks:
`csharp
foreach (var kvp in parameters.Where(kvp => kvp.Value != null))
{
    // Process non-null value
}
`

## Testing Conventions

### Test Organization
- **Test File**: RequestBuilderTests.cs (MSTest)
- **Test Class**: RequestBuilderTests with [TestClass] attribute
- **Per-Method Tests**: Group tests by public method name
- **Setup/Cleanup**: Use [TestInitialize] and [TestCleanup]

### Test Naming
- **Format**: MethodName_Condition_ExpectedOutcome
- **Examples**:
  - AddHeader_ValidInput_ReturnsBuilder
  - AddHeader_NullName_Throws
  - AddParameters_WithDuplicates_ReplacesPrevious

### Test Pattern (AAA - Arrange, Act, Assert)
`csharp
[TestMethod]
public void AddHeader_ValidInput_ReturnsBuilder()
{
    // Arrange
    var builder = new RequestBuilder("resource");
    
    // Act
    var result = builder.AddHeader("X-Custom", "value");
    
    // Assert
    Assert.AreSame(builder, result);
}
`

### Observable Behavior Testing
Test through public API only; inspect Create() result:
`csharp
[TestMethod]
public void AddHeader_AddsToRequest()
{
    var builder = new RequestBuilder("resource");
    var request = builder.AddHeader("X-Custom", "value").Create();
    
    Assert.AreEqual(1, request.Headers.Count);
    Assert.AreEqual("value", request.Headers[0].Value);
}
`

## Error Handling

### Exception Throwing
- **Throw Early**: Validate at public API boundary
- **Descriptive Messages**: Use nameof() for parameter names
- **Specific Types**: Use ArgumentNullException, not Exception

### No Swallowing
Never silently ignore errors:
`csharp
// BAD: Silently ignores null
if (value != null)
{
    AddParameter(new QueryParameter(name, value));
}

// GOOD: Throws if null not allowed
if (value is null)
    throw new ArgumentNullException(nameof(value));
AddParameter(new QueryParameter(name, value));
`

## Dependencies and Imports

### Using Statements
- **Order**: System, then System.Collections, then RestSharp, then RestSharp.RequestBuilder
- **No Aliases**: Avoid using aliases (using List = System.Collections.Generic.List)
- **Minimal**: Include only necessary namespaces

### Version Constraints
- **RestSharp**: Private asset; versions managed by Directory.Packages.props
- **Analyzers**: Enabled by default; don't disable without justification
- **.NET Standard 2.0**: Minimum target; no .NET 5+ specific features

## Consistency Checklist

Before submitting code, verify:
- [ ] All public members have XML documentation
- [ ] Method names follow verb-phrase convention (Add, Remove, Set, With)
- [ ] All parameters are validated before use
- [ ] All mutations return 	his for fluent chaining
- [ ] Private fields prefixed with underscore
- [ ] Case-insensitive comparisons for parameter/header/cookie identity
- [ ] InvariantCulture used for string conversions
- [ ] No code duplication; bulk operations delegate to single-item operations
- [ ] Tests follow AAA pattern and use descriptive names
- [ ] Tests inspect Create() result, not private fields
- [ ] No empty catch blocks; no silent error swallowing

---

**Last Updated**: December 2025
**Standards Maintainer**: RestSharp.RequestBuilder Team
**Tool Enforcement**: CSharpAnalyzers, SonarAnalyzer.CSharp, StyleCop
