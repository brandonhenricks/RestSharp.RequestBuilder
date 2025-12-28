# Technology Stack Instructions

## Overview

This document specifies all technologies, frameworks, libraries, and tools used in the RestSharp.RequestBuilder project, including versions, constraints, and integration points.

## Core Technologies

### .NET Runtime

#### Target Framework
- **Primary**: .NET Standard 2.0 (netstandard2.0)
- **Rationale**: Enables compatibility with .NET Framework 4.6.1+, .NET Core 2.0+, and .NET 5+
- **Compatibility**: Broadest reach for NuGet consumers
- **Location**: src/RestSharp.RequestBuilder/RestSharp.RequestBuilder.csproj

#### Build/Test Frameworks
- **.NET 8.0** (net8.0): Primary development and test target
- **.NET 9.0** (net9.0): Forward-compatibility testing
- **.NET Core 2.1** (netcoreapp2.1): Legacy test compatibility
- **Location**: RestSharp.RequestBuilder.UnitTests.csproj (multi-target)

### Runtime Dependencies

#### RestSharp
- **Purpose**: HTTP client library for REST requests
- **Version**: Managed by Directory.Packages.props (centralized)
- **Type**: Private asset (IncludeAssets=none for consumers)
- **Usage**: RequestBuilder wraps RestSharp.RestRequest
- **Integration**:
  - RequestBuilder.Create() returns RestRequest
  - RestRequestExtensions provide WithBuilder() entry point
  - Parameter types (QueryParameter, UrlSegmentParameter) from RestSharp
  - File handling via RestSharp.AddFile() overloads

#### System.* Framework Packages
- **System.Text.Json**: JSON serialization (from .NET Standard)
- **System.Collections.Generic**: Generics support (Dictionary, List, HashSet)
- **System.Linq**: LINQ query support
- **System.Globalization**: CultureInfo.InvariantCulture for string conversion
- **System.IO**: Stream support for file attachments
- **No External Serialization**: Builder is format-agnostic; RestSharp handles serialization

## Development Dependencies

### Test Framework

#### MSTest (Microsoft.VisualStudio.TestTools.UnitTesting)
- **Purpose**: Unit testing framework
- **Version**: Stable (latest compatible with .NET 8/9)
- **Attributes**: [TestClass], [TestMethod], [TestInitialize], [TestCleanup]
- **Project**: tests/RestSharp.RequestBuilder.UnitTests/RestSharp.RequestBuilder.UnitTests.csproj
- **Test Count**: 100+ test methods in RequestBuilderTests.cs
- **Pattern**: AAA (Arrange, Act, Assert)

### Static Analysis

#### CSharpAnalyzers
- **Purpose**: C# code quality analysis
- **Type**: PrivateAssets (build-only dependency)
- **IncludeAssets**: runtime; build; native; contentfiles; analyzers
- **Enforces**:
  - Naming conventions
  - Code organization
  - Error handling patterns
  - Style consistency

#### SonarAnalyzer.CSharp
- **Purpose**: Static code analysis for reliability and maintainability
- **Type**: PrivateAssets (build-only dependency)
- **IncludeAssets**: runtime; build; native; contentfiles; analyzers
- **Enforces**:
  - SOLID principles
  - Code smells detection
  - Security vulnerabilities
  - Cognitive complexity limits

#### Microsoft.SourceLink.GitHub
- **Purpose**: Embed source code links in NuGet package
- **Type**: PrivateAssets (build-only dependency)
- **Integration**: PDB files contain GitHub URLs for debugging
- **Benefit**: Debuggers can fetch source code from repository

### Package Management

#### Directory.Packages.props
- **Central Package Management (CPM)**: Unified version control
- **Location**: Repository root
- **Scope**: All NuGet dependencies defined once
- **Inheritance**: All .csproj files reference centrally-managed versions
- **Transitive Dependency Control**: Version constraints for indirect dependencies

#### nuget.config
- **Purpose**: NuGet source configuration
- **Location**: Repository root
- **Scope**: Specifies NuGet.org and other feed sources
- **Security**: API key management for package publishing

## Build and Compilation

### Project Properties

#### RestSharp.RequestBuilder.csproj
`xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>netstandard2.0</TargetFramework>
    <GeneratePackageOnBuild>true</GeneratePackageOnBuild>
    <Authors>Brandon Henricks</Authors>
    <Company>Brandon Henricks</Company>
    <Description>RestSharp wrapper using Fluent Syntax</Description>
    <Copyright>2025</Copyright>
    <PackageProjectUrl>https://github.com/brandonhenricks/RestSharp.RequestBuilder/</PackageProjectUrl>
    <RepositoryUrl>https://github.com/brandonhenricks/RestSharp.RequestBuilder/</RepositoryUrl>
    <PackageTags>csharp restsharp</PackageTags>
  </PropertyGroup>
</Project>
`

#### GeneratePackageOnBuild
- **Purpose**: Automatic NuGet package generation during build
- **Trigger**: dotnet build or CI/CD pipeline
- **Output**: .nupkg file in bin/Release/
- **Version**: Extracted from csproj AssemblyVersion

### Build Targets and Outputs

#### Debug Build
- **Command**: dotnet build RestSharp.RequestBuilder.sln -c Debug
- **Output**:
  - bin/Debug/netstandard2.0/RestSharp.RequestBuilder.dll
  - bin/Debug/net8.0/, net9.0/ (multi-target test builds)
  - RestSharp.RequestBuilder.xml (API documentation)
  - Symbol files for debugging

#### Release Build
- **Command**: dotnet build RestSharp.RequestBuilder.sln -c Release
- **Output**:
  - bin/Release/netstandard2.0/RestSharp.RequestBuilder.dll
  - Optimized IL with minimal symbols
  - Ready for NuGet packaging

#### NuGet Pack
- **Command**: dotnet pack src/RestSharp.RequestBuilder/RestSharp.RequestBuilder.csproj -c Release
- **Output**: RestSharp.RequestBuilder.1.0.2.nupkg
- **Contents**:
  - lib/netstandard2.0/RestSharp.RequestBuilder.dll
  - XML documentation
  - Metadata from csproj

## Testing Infrastructure

### Test Execution

#### Primary Command
`powershell
dotnet test tests/RestSharp.RequestBuilder.UnitTests/RestSharp.RequestBuilder.UnitTests.csproj
`

#### Test Frameworks
- **Runner**: dotnet test (built-in)
- **Framework**: MSTest
- **Coverage**: 100+ test cases across all public methods
- **Platforms**: .NET 8.0, .NET Core 2.1

#### Test Categories
- **Constructor Tests**: Null guards, resource parsing, method/format initialization
- **Body Tests**: AddBody, AddJsonBody, AddXmlBody, AddFormUrlEncodedBody
- **File Tests**: AddFile, AddFileBytes, AddFileStream, mixed sources
- **Header Tests**: AddHeader, AddHeaders, RemoveHeader, deduplication
- **Parameter Tests**: Case-insensitive deduplication, AddQueryParameters, AddUrlSegment
- **Cookie Tests**: AddCookie, RemoveCookies, HashSet-based deduplication
- **Authentication Tests**: WithBearerToken, WithBasicAuth, WithApiKey, WithOAuth2
- **Content Negotiation Tests**: WithAccept, WithAcceptJson, WithContentType
- **Integration Tests**: Complex request building with multiple components

### Test Patterns

#### Arrange-Act-Assert (AAA)
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

#### Observable Behavior Testing
Tests inspect Create() output, not private fields:
`csharp
var request = builder
    .AddQueryParameter("page", 1)
    .AddHeader("Authorization", "Bearer token")
    .Create();

Assert.AreEqual("page", request.Parameters[0].Name);
`

## Version Management

### Global .NET Version
- **File**: global.json
- **Purpose**: Constrain SDK version for consistent builds
- **Example**: Ensures all developers use .NET 8.0 SDK or compatible

### Assembly Versioning
- **Property**: <AssemblyVersion> in csproj
- **NuGet Version**: Extracted from assembly version
- **Format**: SemanticVersioning (Major.Minor.Patch)

### Transitive Dependencies
- **Managed By**: Directory.Packages.props
- **Control**: Explicit version pins prevent version conflicts
- **Updates**: Manual review before updating dependencies

## Continuous Integration

### Build Pipeline
- **Trigger**: Pull requests, commits to master
- **Steps**:
  1. Restore NuGet packages
  2. Build solution (dotnet build)
  3. Run unit tests (dotnet test)
  4. Generate NuGet package (dotnet pack) - on Release config
  5. Optional: Code quality scan (SonarAnalyzer integration)

### Analysis Tools
- **Analyzers**: Run during build (CSharpAnalyzers, SonarAnalyzer)
- **Errors as Warnings**: Configurable in build
- **Reporting**: MSBuild output to CI logs

## Documentation Generation

### XML Documentation
- **Generated**: During build from XML doc comments
- **Output**: RestSharp.RequestBuilder.xml
- **Consumption**: IDE tooltips, external tools (Sandcastle, DocFX)
- **Location**: Embedded in NuGet package

### SourceLink Integration
- **Generated**: During build
- **Content**: GitHub repository URLs for each source file
- **Benefit**: Debuggers can fetch source code on demand
- **Requirement**: GitHub repository must be public

## External Tools and Extensions

### Development Tools
- **Visual Studio 2022**: Primary IDE
- **VS Code**: Alternative with C# extensions
- **Rider**: JetBrains C# IDE option

### Code Quality Tools
- **StyleCop Analyzers**: Code style enforcement
- **Resharper**: Code inspection and refactoring
- **CodeMetrics**: Complexity analysis

### Documentation Tools
- **Sandcastle Help File Builder**: Generate CHM help files (optional)
- **DocFX**: Generate web-based API documentation (optional)

## Security and Compliance

### SourceLink
- **Security**: Public repository only; no credentials in URLs
- **Verification**: PDB checksums protect against tampering
- **Benefit**: Debugger can verify source authenticity

### NuGet Package Signing
- **Optional**: Can sign packages for verification
- **Tool**: dotnet nuget sign command
- **Certificate**: Code signing certificate required

## Dependency Tree

`
RestSharp.RequestBuilder
├── RestSharp (runtime)
│   ├── System.Text.Json
│   ├── System.Net.Http
│   └── ...
├── CSharpAnalyzers (build-only)
├── SonarAnalyzer.CSharp (build-only)
└── Microsoft.SourceLink.GitHub (build-only)

RestSharp.RequestBuilder.UnitTests
├── RestSharp.RequestBuilder (project reference)
├── MSTest.TestAdapter
└── MSTest.TestFramework
`

## Version Constraints

### Runtime
- **.NET Standard 2.0**: No upper bound; forward-compatible
- **RestSharp**: Version constraint in Directory.Packages.props

### Build
- **.NET 8.0 SDK**: Minimum version in global.json
- **Analyzers**: Latest stable, checked for compatibility

### Test
- **.NET 8.0, 9.0, 2.1**: Multi-target for broadest coverage

## Technology Decisions

### Why Fluent Builder?
- **Readability**: Method chaining creates natural, readable API
- **Flexibility**: Can add new methods without breaking existing code
- **Testability**: Each method can be tested independently
- **Type Safety**: Compile-time verification of valid request states

### Why .NET Standard 2.0?
- **Compatibility**: Broadest reach across .NET ecosystems
- **Maturity**: Well-supported, stable API surface
- **Deprecation**: Still widely used; good support timeline

### Why MSTest?
- **Microsoft Official**: First-party support and documentation
- **Integration**: Built-in to Visual Studio
- **Simplicity**: Lightweight, minimal setup required

### Why Sealed Class?
- **Invariant Protection**: Prevents subclass violation of builder semantics
- **Performance**: No virtual method dispatch overhead
- **Intent**: Signals that extension is via extension methods, not inheritance

---

**Last Updated**: December 2025
**Maintainer**: RestSharp.RequestBuilder Team
**Notes**: Versions in Directory.Packages.props are source of truth for actual versions
