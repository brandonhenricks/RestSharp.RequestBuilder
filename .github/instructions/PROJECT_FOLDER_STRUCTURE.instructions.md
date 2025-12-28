# Project Folder Structure Instructions

## Directory Layout

`
RestSharp.RequestBuilder/
├── .github/
│   ├── chatmodes/
│   │   └── custom_instructions.chatmode.md      # AI Toolkit chat mode
│   ├── instructions/
│   │   ├── ARCHITECTURE.instructions.md         # This file set
│   │   ├── CODE_EXEMPLARS.instructions.md
│   │   ├── CODING_STANDARDS.instructions.md
│   │   ├── PROJECT_FOLDER_STRUCTURE.instructions.md
│   │   ├── TECHNOLOGY_STACK.instructions.md
│   │   ├── UNIT_TESTS.instructions.md
│   │   ├── WORKFLOW_ANALYSIS.instructions.md
│   │   └── microsoft-learn.instructions.md      # External resource guidelines
│   ├── workflows/
│   │   └── CI/CD pipeline definitions
│   └── copilot-instructions.md                  # Copilot development guide
│
├── src/
│   └── RestSharp.RequestBuilder/
│       ├── RequestBuilder.cs                     # Core builder class (743 lines)
│       │   - Private fields: headers, parameters, cookies, files, body, format, method, timeout
│       │   - 5 constructor overloads
│       │   - 40+ public methods across 10 functional categories
│       │   - Pattern-matched Create() for file assembly
│       │
│       ├── Extensions/
│       │   └── RestRequestExtensions.cs          # Extension methods on RestRequest
│       │       - WithBuilder() overloads for string, Uri, with/without Method
│       │       - Entry point for fluent API from client code
│       │
│       ├── Interfaces/
│       │   └── IRequestBuilder.cs                # Public contract (338 lines)
│       │       - HeaderCount property
│       │       - 50+ method signatures
│       │       - Comprehensive XML documentation
│       │
│       ├── Models/
│       │   ├── CookieValue.cs                    # Immutable cookie POCO
│       │   ├── CookieValueComparer.cs            # Case-insensitive equality
│       │   └── FileAttachment.cs                 # Polymorphic file abstraction
│       │       - Abstract base class
│       │       - PathFileAttachment subclass
│       │       - ByteFileAttachment subclass
│       │       - StreamFileAttachment subclass
│       │
│       ├── RestSharp.RequestBuilder.csproj       # Project file
│       │   - TargetFramework: netstandard2.0
│       │   - GeneratePackageOnBuild: true
│       │   - PackageReference: RestSharp, Analyzers, SourceLink
│       │
│       ├── bin/                                  # Build output (generated)
│       │   └── Debug/, Release/
│       │       └── net8.0/, net9.0/, netstandard2.0/
│       │
│       └── obj/                                  # Intermediate build artifacts (generated)
│           └── Restored packages and metadata
│
├── tests/
│   └── RestSharp.RequestBuilder.UnitTests/
│       ├── RequestBuilderTests.cs                # Main test suite (2219 lines)
│       │   - [TestClass] RequestBuilderTests
│       │   - 100+ [TestMethod] test cases
│       │   - Categories:
│       │     * Constructors (null guards, resource parsing)
│       │     * Body operations (AddBody, AddJsonBody, etc.)
│       │     * File operations (AddFile, AddFileBytes, AddFileStream)
│       │     * Header operations (AddHeader, RemoveHeader, etc.)
│       │     * Parameter operations (case-insensitive deduplication)
│       │     * Cookie operations
│       │     * Authentication (Bearer, Basic, API Key, OAuth2)
│       │     * Content negotiation (Accept, ContentType)
│       │     * Mixed scenarios (multiple file sources, complex builds)
│       │   - Test helper: TestUser class for serialization tests
│       │
│       ├── RestSharp.RequestBuilder.UnitTests.csproj
│       │   - TargetFramework: net8.0 (primary), netcoreapp2.1
│       │   - Test Framework: MSTest (Microsoft.VisualStudio.TestTools.UnitTesting)
│       │   - References: RestSharp.RequestBuilder project
│       │
│       ├── bin/                                  # Test build output (generated)
│       └── obj/                                  # Test build artifacts (generated)
│
├── .gitignore
├── Directory.Build.props                         # Shared build properties
├── Directory.Packages.props                      # Centralized NuGet package versions
├── global.json                                   # Global .NET version constraints
├── nuget.config                                  # NuGet source configuration
├── RestSharp.RequestBuilder.sln                  # Solution file (2 projects)
├── README.md                                     # User-facing documentation
│   - Installation instructions
│   - Feature overview
│   - Usage examples (basic, query params, authentication, files)
│   - NuGet badge
│
├── LICENSE                                       # Repository license
└── images/                                       # Documentation images

TestResults/ (generated)
├── Deploy_bhenricks 20251213T155215_62064/      # Test run artifacts
└── Deploy_bhenricks 20251213T155307_45816/
`

## Key Directories

### /.github/instructions/
**Purpose**: Developer-facing instruction documents
**Contents**:
- ARCHITECTURE.instructions.md - System design and layering
- CODE_EXEMPLARS.instructions.md - Patterns to follow
- CODING_STANDARDS.instructions.md - Style and conventions
- PROJECT_FOLDER_STRUCTURE.instructions.md - This file
- TECHNOLOGY_STACK.instructions.md - Dependencies and versions
- UNIT_TESTS.instructions.md - Testing strategy and coverage
- WORKFLOW_ANALYSIS.instructions.md - Build, test, release pipelines
- microsoft-learn.instructions.md - External resource guidelines

### /src/RestSharp.RequestBuilder/
**Purpose**: Core library code
**Organization**:
- Root level: RequestBuilder.cs (main implementation)
- Extensions/: Entry point methods via extension methods
- Interfaces/: Public contract (IRequestBuilder)
- Models/: Data types (CookieValue, FileAttachment and subclasses)

### /tests/RequestBuilderTests.cs
**Purpose**: Comprehensive unit test coverage
**Scope**: 100+ tests covering all public methods, error cases, and edge cases
**Pattern**: MSTest with [TestClass], [TestMethod], [TestInitialize], [TestCleanup]

## Build Output Structure

### Debug/
`
bin/Debug/
├── net8.0/                  # .NET 8 debug build
├── net9.0/                  # .NET 9 debug build
└── netstandard2.0/          # .NET Standard 2.0 debug build
    ├── RestSharp.RequestBuilder.dll
    ├── RestSharp.RequestBuilder.xml           (API documentation)
    ├── RestSharp.dll                          (dependency)
    └── System.*.xml                           (framework docs)
`

### obj/
`
obj/
├── Debug/
│   ├── netstandard2.0/                        # Primary build
│   │   ├── RestSharp.RequestBuilder.AssemblyInfo.cs
│   │   ├── RestSharp.RequestBuilder.GeneratedMSBuildEditorConfig.editorconfig
│   │   └── RestSharp.RequestBuilder.GlobalUsings.g.cs
│   └── net8.0/, net9.0/                       # Multi-target builds
├── project.assets.json                         # Package restore metadata
└── RestSharp.RequestBuilder.csproj.nuget.*.props/targets
`

## NuGet Package Structure

When packed via dotnet pack, generates:
`
RestSharp.RequestBuilder.1.0.2.nupkg
├── lib/
│   ├── netstandard2.0/
│   │   └── RestSharp.RequestBuilder.dll
│   │   └── RestSharp.RequestBuilder.xml
│   ├── net8.0/
│   └── net9.0/
├── _rels/
├── package/
├── RestSharp.RequestBuilder.nuspec
├── [Content_Types].xml
└── ... (metadata)
`

## File Naming Conventions

### Source Code
- **Classes**: PascalCase, one class per file
- **Extensions**: Suffix is "Extensions" (RestRequestExtensions.cs)
- **Comparers**: Suffix is "Comparer" (CookieValueComparer.cs)
- **Models**: Descriptive names (CookieValue.cs, FileAttachment.cs)
- **Interfaces**: Prefix is "I" (IRequestBuilder.cs)

### Documentation
- **Instructions**: Suffix is ".instructions.md" (ARCHITECTURE.instructions.md)
- **README**: Named "README.md" at repository root
- **Changelogs**: Named "CHANGELOG.md" (if present)

## Configuration Files

### RestSharp.RequestBuilder.csproj
- TargetFramework: netstandard2.0
- GeneratePackageOnBuild: true
- Dependencies: RestSharp (private asset)
- Analyzers: CSharpAnalyzers, SonarAnalyzer.CSharp
- SourceLink: Microsoft.SourceLink.GitHub

### RestSharp.RequestBuilder.UnitTests.csproj
- TargetFramework: net8.0 (primary), netcoreapp2.1 (legacy)
- TestFramework: MSTest
- ProjectReference: RestSharp.RequestBuilder

### Directory.Build.props
- Shared build properties across all projects
- Version, copyright, package metadata
- Analyzer settings, warnings-as-errors

### Directory.Packages.props
- Centralized NuGet package version management
- Transitive dependency control
- Central package version (CPM) feature

### global.json
- Constrains .NET SDK version used for builds
- Ensures consistent build environment across developers

## Key Build Artifacts (Generated, Do Not Commit)

- bin/ - Compiled binaries and documentation
- obj/ - Intermediate build artifacts
- TestResults/ - Test run outputs and reports
- *.nupkg - Generated NuGet packages (from bin/Release)

## Summary

The folder structure follows standard .NET project conventions:
- Clear separation between src/, tests/, and .github/
- Logical grouping within src/ (Extensions, Interfaces, Models)
- Comprehensive documentation in .github/instructions/
- All source code in a single library project (RestSharp.RequestBuilder)
- Automated build outputs in bin/ and obj/
- Test projects parallel to source projects

---

**Last Updated**: December 2025
**Maintainer**: RestSharp.RequestBuilder Team
