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

## License

RestSharp.RequestBuilder is licensed under the [MIT license](LICENSE).
