# RestSharp.RequestBuilder
RestSharp.RequestBuilder is a .NET Standard library for use with RestSharp.
It's primary goal is to aid with creating RestRequest objects using Fluent Syntax.
The RequestBuilder class acts as a general purpose wrapper for the IRestRequest object. 
It was created to help aid with an internal project that uses the RestSharp Client to interact with a third-party API.

## Installation
[![NuGet Status](https://img.shields.io/nuget/v/RestSharp.RequestBuilder.svg?style=flat)](https://www.nuget.org/packages/RestSharp.RequestBuilder/)

You can install the RestSharp.RequestBuilder NuGet package from the .NET Core CLI using:

```
dotnet add package RestSharp.RequestBuilder
```

or from the NuGet package manager:

```
Install-Package RestSharp.RequestBuilder
```

## Examples
Below is an example of instantiating a RequestBuilder with the resource of "user", and setting the appropriate values to create a RestRequest object that we can later
pass into a RestSharp Client instance.

```csharp
var builder = new RequestBuilder("user");

var request = builder
	.SetFormat(DataFormat.Json)
	.SetMethod(Method.GET)
	.AddHeader("test-header", "value")
	.Create();
```


```csharp
var request = new RestRequest().WithBuilder("user")
	.SetFormat(DataFormat.Json)
	.SetMethod(Method.GET)
	.AddHeader("test-header", "value")
	.Create();
```

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

## License

RestSharp.RequestBuilder is licensed under the [MIT license](LICENSE).
