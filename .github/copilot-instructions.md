# RestSharp.RequestBuilder — Copilot Instructions

## Big picture architecture
- The sealed fluent helper in [src/RestSharp.RequestBuilder/RequestBuilder.cs](src/RestSharp.RequestBuilder/RequestBuilder.cs) is the canonical place for assembling a `RestSharp.RestRequest`; it keeps headers, parameters, a body, cookies, file metadata, format, method, and timeout before emitting the final `Create()` result.
- The behavior surface is defined by [src/RestSharp.RequestBuilder/Interfaces/IRequestBuilder.cs](src/RestSharp.RequestBuilder/Interfaces/IRequestBuilder.cs); follow its fluent chaining style and the fact that every mutator returns `this` so new helpers can slot straight into the same pipeline.
- Entry points always go through [src/RestSharp.RequestBuilder/Extensions/RestRequestExtensions.cs](src/RestSharp.RequestBuilder/Extensions/RestRequestExtensions.cs); `RestRequest.WithBuilder(...)` is the documented shortcut mentioned in [README.md](README.md) and mirrors the constructors in `RequestBuilder`.
- Cookies live inside `HashSet<CookieValue>(new CookieValueComparer())`, so add/remove helpers work through [src/RestSharp.RequestBuilder/Models/CookieValue.cs](src/RestSharp.RequestBuilder/Models/CookieValue.cs) and `CookieValueComparer`; any new identity logic needs to respect the case-insensitive equality baked into that pair.
- Parameters are stored in `List<Parameter>` and deduplicated case-insensitively by name. The library offers both singular helpers like `AddQueryParameter(name, value)` and bulk operations like `AddQueryParameters(dictionary)`, with authentication wrappers (e.g., `WithBearerToken`, `WithBasicAuth`, `WithApiKey`) for common header patterns.
- The library targets `netstandard2.0` and ships as a NuGet package with `GeneratePackageOnBuild` plus metadata in [src/RestSharp.RequestBuilder/RestSharp.RequestBuilder.csproj](src/RestSharp.RequestBuilder/RestSharp.RequestBuilder.csproj), so preserve that project file when adding dependencies, analyzers (CSharpAnalyzers/SonarAnalyzer/SourceLink), or metadata updates.

## Key workflows & commands
- Run `dotnet build RestSharp.RequestBuilder.sln` (or `dotnet build src/RestSharp.RequestBuilder/RestSharp.RequestBuilder.csproj`) to trigger the analyzers and package-generation hooks defined in the project file.
- Run `dotnet test tests/RestSharp.RequestBuilder.UnitTests/RestSharp.RequestBuilder.UnitTests.csproj` to exercise the MSTest coverage, especially the parameter/cookie/header behavior captured in [tests/RestSharp.RequestBuilder.UnitTests/RequestBuilderTests.cs](tests/RestSharp.RequestBuilder.UnitTests/RequestBuilderTests.cs).
- Use `RestRequest.WithBuilder(...)` from client code to verify new builder APIs when writing tests; `Create()` returns the real `RestRequest`, so inspect `request.Parameters`, `request.Headers`, and `request.CookieContainer` just like the existing tests do.
- Package locally with `dotnet pack src/RestSharp.RequestBuilder/RestSharp.RequestBuilder.csproj -c Release` for manual NuGet distribution.
- Note: `RestRequestExtensions.WithBuilder(...)` ignores the original `RestRequest` instance and returns a fresh builder – the pattern exists for fluent ergonomics, not request mutation.

## Conventions & patterns to respect
- Headers stay in a single dictionary; `AddHeader` only replaces a value if it actually changes, and `AddHeaders` loops the same logic with case-insensitive lookups – avoid reintroducing multiple storage structures or new header collections.
- Parameter deduplication now happens in `AddParameters(Parameter[] parameters)` with a single `Dictionary<string, int>` keyed by `StringComparer.InvariantCultureIgnoreCase`. This O(n+m) strategy builds a lookup of existing parameter indices, then replaces/adds each new parameter while tracking duplicates within the input array itself. Keep this pattern whenever touching parameter merging so the behavior in [tests/RestSharp.RequestBuilder.UnitTests/RequestBuilderTests.cs](tests/RestSharp.RequestBuilder.UnitTests/RequestBuilderTests.cs) continues to pass.
- `AddParameter(Parameter)` follows the same case-insensitive replacement path via `FindIndex`; keep the `StringComparison.InvariantCultureIgnoreCase` comparisons intact when expanding the API or adding new helpers such as `AddQuery` or `AddBodyProperty`.
- Bulk parameter helpers like `AddQueryParameters(IDictionary<string, object>)` delegate to `AddParameter(Parameter)` for each entry, ensuring consistent deduplication logic. All values are automatically converted to strings using `InvariantCulture`.
- Optional pieces (timeout, file uploads, cookie headers) are stored in private fields and only forwarded during `Create()`. That makes the builder safe to call repeatedly, so tests reconstruct the request once per scenario.
- Authentication helpers (`WithBearerToken`, `WithBasicAuth`, `WithApiKey`, `WithOAuth2`) replace the `Authorization` header directly; each automatically formats the value per HTTP standards (e.g., Base64-encoding for Basic auth).
- Content negotiation helpers (`WithAccept`, `WithAcceptJson`, `WithAcceptXml`, `WithContentType`) and other header convenience methods (`WithUserAgent`, `WithIfMatch`, `WithIfNoneMatch`, `WithReferer`, `WithOrigin`, `WithAuthorization`) use standard HTTP header casing and throw `ArgumentNullException` on null/empty values.

## Testing guidance
- Add new unit tests beside [tests/RestSharp.RequestBuilder.UnitTests/RequestBuilderTests.cs](tests/RestSharp.RequestBuilder.UnitTests/RequestBuilderTests.cs) and follow the MSTest `[TestClass]`/`[TestMethod]` pattern; the existing suite already checks null guards, header counts, parameter replacement, and insertion order.
- When you need to assert cookies, reuse the `CookieContainer` assertions already present; the builder adds cookies through `CookieValue` so new cookie helpers should surface there rather than reaching into `CookieContainer` directly.
- Instrument new helpers by calling `Create()` and inspecting the returned `RestRequest`. That keeps the tests focused on observable behaviors (request format, method, parameters, headers, file attachments) rather than internal fields.
- Tests follow a `[TestInitialize]`/`[TestCleanup]` pattern with a shared `_builder` instance for common scenarios, but each test calls `Create()` independently to avoid side effects between tests.

## Packaging & analyzer expectations
- The project ships as netstandard 2.0, references `RestSharp`, and enables `CSharpAnalyzers`, `SonarAnalyzer.CSharp`, and `Microsoft.SourceLink.GitHub` per [src/RestSharp.RequestBuilder/RestSharp.RequestBuilder.csproj](src/RestSharp.RequestBuilder/RestSharp.RequestBuilder.csproj); don’t add runtime dependencies that would break the lightweight surface or disable the analyzers.
- `GeneratePackageOnBuild` is already set, so CI builds run `dotnet build` and get a NuGet package; if you need to publish locally, `dotnet pack src/RestSharp.RequestBuilder/RestSharp.RequestBuilder.csproj -c Release` is the manual path.

## Need feedback?
- If any part of this document is unclear, incomplete, or misses a critical workflow (build/test/pack/release), update `.github/copilot-instructions.md` and ask the team for clarification before proceeding with feature work.