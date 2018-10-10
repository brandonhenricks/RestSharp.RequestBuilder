# RestSharp.RequestBuilder
RestRequest Builder for RestSharp. Uses Fluid Syntax.

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
