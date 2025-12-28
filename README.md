# RestSharp.RequestBuilder

[![NuGet Status](https://img.shields.io/nuget/v/RestSharp.RequestBuilder.svg?style=flat)](https://www.nuget.org/packages/RestSharp.RequestBuilder/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

RestSharp.RequestBuilder is a .NET Standard library that provides a fluent, chainable API for constructing REST requests using RestSharp. It offers a clean, expressive syntax for building complex HTTP requests with headers, authentication, parameters, files, and request bodies—all while maintaining strong type safety and minimal surface area.

## Overview

The library's primary goal is to aid in creating `RestRequest` objects using Fluent Syntax, reducing boilerplate and improving readability. The `RequestBuilder` class acts as a general-purpose wrapper for the `IRestRequest` object. It was created to help with projects that use the RestSharp Client to interact with third-party APIs.

## Technology Stack

- **Target Framework**: .NET Standard 2.0 (compatible with .NET Framework 4.6.1+, .NET Core 2.0+, and .NET 5+)
- **Build/Test Frameworks**: .NET 8.0, .NET 9.0, .NET Core 2.1
- **Test Framework**: MSTest (Microsoft.VisualStudio.TestTools.UnitTesting)
- **HTTP Client**: RestSharp (core dependency)
- **Static Analysis**: CSharpAnalyzers, SonarAnalyzer.CSharp
- **Code Link Embedding**: Microsoft.SourceLink.GitHub

## Installation

Install the RestSharp.RequestBuilder NuGet package from the .NET Core CLI:

```shell
dotnet add package RestSharp.RequestBuilder
```

Or from the NuGet package manager:

```shell
Install-Package RestSharp.RequestBuilder
```

## Getting Started

### Basic Usage

Here are the two primary ways to create a RestRequest with RestSharp.RequestBuilder:

**Using the RequestBuilder constructor:**
```csharp
var builder = new RequestBuilder("user");

var request = builder
    .SetFormat(DataFormat.Json)
    .SetMethod(Method.Get)
    .AddHeader("test-header", "value")
    .Create();
```

**Using the fluent extension method on RestRequest:**
```csharp
var request = new RestRequest().WithBuilder("user")
    .SetFormat(DataFormat.Json)
    .SetMethod(Method.Get)
    .AddHeader("test-header", "value")
    .Create();
```

Both approaches return a fully configured `RestRequest` ready for use with a RestSharp `RestClient`.

### Query Parameters and URL Segments

The library provides convenient fluent methods to add query string parameters and URL segments without manually creating Parameter objects:

```csharp
// Add query parameters individually
var request = new RestRequest().WithBuilder("users")
    .AddQueryParameter("page", 1)
    .AddQueryParameter("limit", 50)
    .AddQueryParameter("sort", "desc")
    .Create();
```

```csharp
// Add multiple query parameters at once
var request = new RestRequest().WithBuilder("users")
    .AddQueryParameters(new Dictionary<string, object>
    {
        { "page", 1 },
        { "limit", 50 },
        { "sort", "desc" }
    })
    .Create();
```

```csharp
// Add URL segments for parameterized routes
var request = new RestRequest().WithBuilder("users/{id}/posts")
    .AddUrlSegment("id", 123)
    .AddQueryParameter("page", 1)
    .Create();
```

**Features:**

- All values are automatically converted to strings using `InvariantCulture`
- Duplicate parameters are replaced (case-insensitive)
- All methods support fluent chaining
- Null values in bulk operations are skipped

### Authentication

The library provides first-class fluent authentication helpers for common HTTP authentication methods:

#### Bearer Token Authentication

```csharp
var request = new RestRequest().WithBuilder("api/me")
    .WithBearerToken("your-bearer-token")
    .Create();
```

#### Basic Authentication

```csharp
var request = new RestRequest().WithBuilder("admin")
    .WithBasicAuth("username", "password")
    .Create();
```

The credentials are automatically Base64-encoded as required by the HTTP Basic authentication standard.

#### API Key Authentication

```csharp
// Using default header name (X-API-Key)
var request = new RestRequest().WithBuilder("secure")
    .WithApiKey("your-api-key")
    .Create();

// Using custom header name
var request = new RestRequest().WithBuilder("secure")
    .WithApiKey("your-api-key", "X-Custom-Api-Key")
    .Create();
```

#### OAuth2 Authentication

```csharp
var request = new RestRequest().WithBuilder("api/protected")
    .WithOAuth2("oauth2-access-token")
    .Create();
```

**Authentication Features:**

- All authentication methods chain with other builder methods
- Setting a new authentication automatically replaces any existing Authorization header
- All methods validate input and throw `ArgumentNullException` for null or empty values
- Full XML documentation for IntelliSense support

### Header Convenience Methods

The library provides fluent convenience methods for common HTTP headers, making it easier to set standard headers without having to remember exact header names or worry about typos:

#### Accept Header

```csharp
// Specify custom Accept header
var request = new RestRequest().WithBuilder("api/data")
    .WithAccept("application/json")
    .Create();

// Convenient shortcuts for common media types
var jsonRequest = new RestRequest().WithBuilder("api/data")
    .WithAcceptJson()  // Sets Accept: application/json
    .Create();

var xmlRequest = new RestRequest().WithBuilder("api/data")
    .WithAcceptXml()  // Sets Accept: application/xml
    .Create();
```

#### Content-Type Header

```csharp
var request = new RestRequest().WithBuilder("api/upload")
    .SetMethod(Method.Post)
    .WithContentType("application/json")
    .AddJsonBody(new { data = "value" })
    .Create();
```

#### User-Agent Header

```csharp
var request = new RestRequest().WithBuilder("api/resource")
    .WithUserAgent("MyCustomClient/1.2.3")
    .Create();
```

#### Custom Authorization Header

```csharp
// For custom authorization schemes beyond Bearer/Basic
var request = new RestRequest().WithBuilder("api/protected")
    .WithAuthorization("Digest", "realm=\"example.com\"")
    .Create();

// You can still use the specific helpers for common schemes
var bearerRequest = new RestRequest().WithBuilder("api/me")
    .WithBearerToken("token123")  // Shortcut for WithAuthorization("Bearer", "token123")
    .Create();
```

#### Conditional Request Headers (ETags)

```csharp
// If-Match for optimistic concurrency control
var updateRequest = new RestRequest().WithBuilder("api/resource/{id}")
    .AddUrlSegment("id", "123")
    .SetMethod(Method.Put)
    .WithIfMatch("\"v1-abc123\"")
    .AddJsonBody(new { name = "Updated Name", version = "v2" })
    .Create();

// If-None-Match for efficient caching
var cacheRequest = new RestRequest().WithBuilder("api/resource/{id}")
    .AddUrlSegment("id", "123")
    .WithIfNoneMatch("\"v1-abc123\"")
    .Create();
```

#### Referer and Origin Headers

```csharp
// Useful for CORS and tracking
var request = new RestRequest().WithBuilder("api/external")
    .WithReferer("https://myapp.com/page")
    .WithOrigin("https://myapp.com")
    .Create();
```

#### Combining Multiple Headers

```csharp
// All header methods chain fluently
var request = new RestRequest().WithBuilder("api/items")
    .WithAcceptJson()
    .WithContentType("application/json")
    .WithUserAgent("MyClient/1.0.0")
    .WithBearerToken("mytoken")
    .WithIfNoneMatch("\"abc1234\"")
    .WithReferer("https://example.com/page")
    .WithOrigin("https://example.com")
    .AddQueryParameter("page", 1)
    .Create();
```

**Header Method Features:**

- All header methods use standard HTTP header casing (e.g., 'Accept', 'Content-Type', 'User-Agent')
- Duplicate headers are automatically replaced when called multiple times
- All methods validate input and throw `ArgumentNullException` for null or empty values
- Seamless integration with other builder methods for flexible request construction
- Full XML documentation for IntelliSense support

### Request Bodies

The library provides convenient shortcut methods for adding request bodies with common content types:

#### JSON Body

```csharp
// Add a JSON body to the request
var request = new RestRequest().WithBuilder("api/users")
    .SetMethod(Method.Post)
    .AddJsonBody(new { name = "John Doe", email = "john@example.com" })
    .Create();
```

The `AddJsonBody` method automatically sets the request format to `DataFormat.Json` and serializes the body object.

#### XML Body

```csharp
// Add an XML body to the request
var request = new RestRequest().WithBuilder("api/users")
    .SetMethod(Method.Post)
    .AddXmlBody(new User { Id = 1, Name = "John Doe" })
    .Create();
```

The `AddXmlBody` method automatically sets the request format to `DataFormat.Xml` and serializes the body object.

#### Form URL Encoded Body

```csharp
// Add form-urlencoded data (commonly used for OAuth token requests)
var request = new RestRequest().WithBuilder("oauth/token")
    .SetMethod(Method.Post)
    .AddFormUrlEncodedBody(new Dictionary<string, string>
    {
        { "grant_type", "password" },
        { "username", "user@example.com" },
        { "password", "secretpassword" }
    })
    .Create();
```

The `AddFormUrlEncodedBody` method adds each key-value pair as a form parameter (`GetOrPost` parameter type).

**Body Method Features:**

- All body methods validate input and throw `ArgumentNullException` for null values
- Body methods chain with other builder methods for flexible request construction
- `AddJsonBody` and `AddXmlBody` automatically set the appropriate request format
- `AddFormUrlEncodedBody` skips null values in the dictionary
- Setting a new body replaces any previously set body
- Full XML documentation for IntelliSense support

**Important:** Do not mix `AddJsonBody`/`AddXmlBody` with `AddFormUrlEncodedBody` on the same request builder. These represent mutually exclusive ways of sending data:

- `AddJsonBody`/`AddXmlBody` set a request body (serialized as JSON or XML)
- `AddFormUrlEncodedBody` adds form parameters (sent as `application/x-www-form-urlencoded`)

Choose one approach based on your API requirements. Mixing both would create a semantically invalid request.

**Note:** These methods work with RestSharp's serialization pipeline. For JSON, ensure you have configured an appropriate JSON serializer (RestSharp uses `System.Text.Json` by default). For XML, RestSharp uses the built-in .NET XML serializer.

## Project Architecture

RestSharp.RequestBuilder implements a **Fluent Builder Pattern** with the following design principles:

### Key Components

- **RequestBuilder.cs**: The sealed core implementation that accumulates REST request configuration (headers, parameters, cookies, files, body, format, method, timeout)
- **IRequestBuilder.cs**: The public contract defining all builder operations with fluent chaining semantics
- **RestRequestExtensions.cs**: Extension methods providing ergonomic entry points via `WithBuilder()` on `RestRequest`
- **Models/**: Supporting types including `CookieValue`, `CookieValueComparer`, and polymorphic `FileAttachment` classes

### Design Patterns

1. **Fluent Builder Pattern**: Every public method returns `IRequestBuilder` to enable method chaining
2. **Single Responsibility**: The builder focuses solely on accumulating configuration without side effects
3. **Sealed Implementation**: `RequestBuilder` is sealed to prevent subclass override and maintain invariants
4. **State Encapsulation**: All configuration is stored in private fields until `Create()` is called, enabling safe repeated calls
5. **Case-Insensitive Deduplication**: Parameters and headers are deduplicated using `StringComparison.InvariantCultureIgnoreCase`

## Project Structure

```
RestSharp.RequestBuilder/
├── src/RestSharp.RequestBuilder/
│   ├── RequestBuilder.cs              # Core builder class (743 lines)
│   ├── Extensions/RestRequestExtensions.cs  # Entry points
│   ├── Interfaces/IRequestBuilder.cs  # Public contract (338 lines)
│   └── Models/
│       ├── CookieValue.cs            # Immutable cookie POCO
│       ├── CookieValueComparer.cs    # Case-insensitive equality
│       └── FileAttachment.cs         # Polymorphic file abstraction
├── tests/RestSharp.RequestBuilder.UnitTests/
│   ├── RequestBuilderTests.cs        # 100+ test cases
│   └── RestSharp.RequestBuilder.UnitTests.csproj
├── .github/
│   ├── instructions/                 # Comprehensive documentation
│   ├── copilot-instructions.md       # AI development guide
│   └── workflows/                    # CI/CD pipeline definitions
├── RestSharp.RequestBuilder.sln      # Solution root
├── Directory.Build.props             # Shared build properties
├── Directory.Packages.props          # Centralized package versions
└── global.json                       # .NET SDK version pinning
```

## Development Setup

### Prerequisites

- **.NET 8.0 SDK** or later (verify with `dotnet --version`)
- Git for version control
- Visual Studio 2022, VS Code, or JetBrains Rider

### Getting Started

```powershell
# Clone the repository
git clone https://github.com/brandonhenricks/RestSharp.RequestBuilder.git
cd RestSharp.RequestBuilder

# Restore packages
dotnet restore

# Build the solution
dotnet build RestSharp.RequestBuilder.sln -c Release

# Run tests
dotnet test RestSharp.RequestBuilder.sln -c Release

# Package locally (optional)
dotnet pack src/RestSharp.RequestBuilder/ -c Release -o ./artifacts
```

## Key Features

- **Fluent API**: Chainable methods for expressive request construction
- **Comprehensive Authentication**: Built-in helpers for Bearer, Basic, API Key, and OAuth2
- **Flexible Parameters**: Query parameters, URL segments, and intelligent deduplication
- **File Handling**: Seamless support for file paths, byte arrays, and streams
- **Content Negotiation**: Convenience methods for Accept, Content-Type, and custom headers
- **Body Serialization**: JSON, XML, and form URL-encoded shortcuts
- **Cookie Management**: Case-insensitive cookie handling via `CookieContainer`
- **Input Validation**: Null checks and type safety throughout
- **Broad Compatibility**: .NET Standard 2.0 support for maximum reach
- **Zero Runtime Dependencies**: Only RestSharp is required

## Coding Standards

The project adheres to strict coding standards:

- **Naming Conventions**: Methods use action verbs (Add*, Remove*, Set*, With*); case-insensitive for parameters/headers
- **Method Organization**: Organized by functional category (Body, File, Header, Parameter, Cookie, Configuration, Authentication)
- **Documentation**: Comprehensive XML comments for IntelliSense
- **Analyzers**: CSharpAnalyzers and SonarAnalyzer.CSharp enforce consistency
- **Test-Driven**: All public methods have associated unit tests covering happy path, null guards, and edge cases

## Testing

The project includes 100+ MSTest unit tests covering:

- **Constructor validation**: Null guards, resource parsing, default values
- **Body operations**: JSON, XML, and form-encoded serialization
- **File operations**: Disk paths, byte arrays, and streams
- **Header operations**: Case-insensitive deduplication and replacement
- **Parameter operations**: Deduplication, URL segments, query parameters
- **Cookie operations**: Case-insensitive handling via `CookieContainer`
- **Authentication**: Bearer, Basic, API Key, OAuth2 methods
- **Content negotiation**: Accept and Content-Type headers
- **Complex scenarios**: Mixed file sources, chained operations

Run tests with:

```powershell
dotnet test RestSharp.RequestBuilder.sln -c Release
```

## Contributing

Contributions are welcome! Please follow these guidelines:

1. **Create a feature branch**: `git checkout -b feature/your-feature-name`
2. **Follow coding standards**: Review [CODING_STANDARDS.instructions.md](.github/instructions/CODING_STANDARDS.instructions.md)
3. **Add tests**: All new public methods must have associated tests
4. **Run the full build**: `dotnet build` and `dotnet test` must pass
5. **Update README**: If adding user-visible features, update this file with examples
6. **Maintain backward compatibility**: The .NET Standard surface is immutable

### Before Submitting a PR

- [ ] `dotnet build RestSharp.RequestBuilder.sln` passes
- [ ] `dotnet test RestSharp.RequestBuilder.sln` passes all tests
- [ ] New public APIs have XML documentation
- [ ] Test coverage for new methods (happy path, null guards, edge cases)
- [ ] README updated if consumer-visible behavior changed
- [ ] No secrets or real tokens in code/tests

## Detailed Documentation

For deeper information, see the [.github/instructions](.github/instructions) directory:

- [ARCHITECTURE.instructions.md](.github/instructions/ARCHITECTURE.instructions.md) — Design decisions and component interactions
- [CODING_STANDARDS.instructions.md](.github/instructions/CODING_STANDARDS.instructions.md) — Naming, style, and documentation conventions
- [PROJECT_FOLDER_STRUCTURE.instructions.md](.github/instructions/PROJECT_FOLDER_STRUCTURE.instructions.md) — Directory organization and key files
- [TECHNOLOGY_STACK.instructions.md](.github/instructions/TECHNOLOGY_STACK.instructions.md) — Dependencies and version management
- [UNIT_TESTS.instructions.md](.github/instructions/UNIT_TESTS.instructions.md) — Testing strategy and coverage
- [WORKFLOW_ANALYSIS.instructions.md](.github/instructions/WORKFLOW_ANALYSIS.instructions.md) — Build, test, and release workflows

See also [AGENTS.md](AGENTS.md) for agent-specific development context and [copilot-instructions.md](.github/copilot-instructions.md) for architectural deep dives.

## License

RestSharp.RequestBuilder is licensed under the [MIT license](LICENSE).
