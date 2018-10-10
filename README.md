# RestSharp.RequestBuilder
RestRequest Builder for RestSharp. Uses Fluid Syntax.

## Example Usage:

```csharp
	var builder = new RequestBuilder("user");
	
	var request = builder
		.SetFormat(DataFormat.Json)
		.SetMethod(Method.GET)
		.AddHeader("test-header", "value")
		.Create();
```