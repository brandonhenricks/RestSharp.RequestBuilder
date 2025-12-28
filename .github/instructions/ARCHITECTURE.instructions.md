# Architecture Instructions

## Overview

RestSharp.RequestBuilder implements a **Fluent Builder Pattern** for constructing REST requests with a focus on clean, chainable APIs. The architecture prioritizes simplicity, immutability at the builder level, and strong encapsulation of state.

## Core Architectural Principles

### 1. Fluent Builder Pattern
- Every public method returns IRequestBuilder to enable method chaining
- Enables readable, expressive syntax for building complex REST requests

### 2. Single Responsibility
- RequestBuilder (sealed): The sole implementation of IRequestBuilder
- Responsible for accumulating request configuration
- Delegates creation to RestSharp via Create()
- No inheritance; sealed to prevent extension

### 3. Layered Architecture
- Public API Layer: IRequestBuilder interface
- Implementation Layer: RequestBuilder sealed class + Extension Methods
- Model Layer: Parameter types, FileAttachment, CookieValue
- Dependency Layer: RestSharp.RestRequest

### 4. State Management Strategy
- Private fields store request configuration until Create() is called
- Allows safe, repeatable Create() calls without side effects
- Configuration never mutates underlying RestSharp RestRequest until finalization

## Key Components

### RequestBuilder.cs (743 lines)
- Core Responsibility: Accumulate and finalize REST request configuration
- Headers: Dictionary<string, string> with case-insensitive lookups
- Parameters: List<Parameter> with InvariantCultureIgnoreCase deduplication
- Cookies: HashSet<CookieValue> with CookieValueComparer for identity
- Files: List<FileAttachment> with polymorphic handling (Path, Bytes, Stream)
- Body, Format, Method, Timeout: Private fields set until Create()

### IRequestBuilder.cs (338 lines)
- Responsibility: Define public contract for all builder operations
- All Methods: Return IRequestBuilder for fluent chaining

### RestRequestExtensions.cs
- Responsibility: Provide ergonomic extension methods on RestSharp.RestRequest
- Overloads: WithBuilder() for string, Uri, with/without Method
- Behavior: Returns new RequestBuilder instances

### Models/ Subdirectory
- CookieValue.cs: Immutable POCO for cookie name, value, path, domain
- CookieValueComparer.cs: Case-insensitive equality by name
- FileAttachment.cs: Abstract base with PathFileAttachment, ByteFileAttachment, StreamFileAttachment

## Key Design Decisions

### Case-Insensitive Parameter Deduplication
- Parameters deduplicated by StringComparison.InvariantCultureIgnoreCase
- Prevents accidental duplicate query parameters
- See AddParameters() for two-pass deduplication algorithm

### Default HTTP Configuration
- Default method: Method.Get
- Default format: DataFormat.Json
- Default timeout: TimeSpan.FromSeconds(30)

### Null Argument Validation
- Every public method validates arguments early
- Throws ArgumentNullException or ArgumentOutOfRangeException

### No Inheritance; Sealed Implementation
- RequestBuilder is sealed to prevent subclass override
- Extension via extension methods or wrapper composition

### InvariantCulture String Conversion
- Query and URL segment parameter values use InvariantCulture
- Ensures consistent, culture-independent behavior

## Dependencies
- RestSharp: Underlying HTTP request library
- Analyzers: CSharpAnalyzers, SonarAnalyzer.CSharp
- .NET Standard 2.0: Target framework for broad compatibility

## Architectural Risks

1. **Stream Lifecycle Management**: Callers must keep streams alive until RestSharp executes
2. **Parameter Type Explosivity**: RestSharp defines many types; builder exposes subset
3. **No Async Support**: Builder is synchronous; all I/O in RestSharp

## Improvement Opportunities

1. **Partial Parameter Replacement**: Add append-only semantics option
2. **Request Interception**: Middleware-style request transformation hooks
3. **Schema Validation**: Optional parameter name/header format validation
4. **Template URL Support**: Advanced URI template RFC 6570 support

## Backward Compatibility
- netstandard2.0 is immutable; new helpers must not break sealed surface
- New methods must return IRequestBuilder to preserve fluent chaining
- Parameter and header deduplication semantics are part of the contract
