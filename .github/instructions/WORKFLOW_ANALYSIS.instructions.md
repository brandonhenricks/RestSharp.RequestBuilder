# Workflow Analysis Instructions

## Overview

This document describes developer workflows, CI/CD pipelines, build processes, and release management procedures for RestSharp.RequestBuilder. It provides clear steps for common tasks and identifies automation opportunities and bottlenecks.

## Developer Workflows

### Setup Workflow

#### Prerequisites
1. .NET 8.0 SDK or later (verify with: dotnet --version)
2. Git for version control
3. Visual Studio 2022 or VS Code with C# extensions
4. Optional: JetBrains Rider

#### Initial Setup
`powershell
# Clone repository
git clone https://github.com/brandonhenricks/RestSharp.RequestBuilder.git
cd RestSharp.RequestBuilder

# Restore packages
dotnet restore

# Verify SDK version
dotnet --version  # Should match global.json constraint

# Open in IDE
Start-Process "RestSharp.RequestBuilder.sln"  # Visual Studio
# OR code .  # VS Code
`

### Development Workflow

#### Feature Development

1. **Create Feature Branch**
   `powershell
   git checkout -b feature/add-custom-header-method
   `

2. **Build Locally**
   `powershell
   dotnet build RestSharp.RequestBuilder.sln
   # Runs analyzers (CSharpAnalyzers, SonarAnalyzer) during build
   # Generates package if GeneratePackageOnBuild=true
   `

3. **Code Implementation**
   - Edit source files in src/RestSharp.RequestBuilder/
   - Follow CODING_STANDARDS.instructions.md
   - Maintain IRequestBuilder contract

4. **Unit Testing**
   `powershell
   dotnet test tests/RestSharp.RequestBuilder.UnitTests/
   # Runs 100+ MSTest test cases
   # Targets net8.0 and netcoreapp2.1
   `

5. **Local Package Testing (Optional)**
   `powershell
   dotnet pack src/RestSharp.RequestBuilder/ -c Release
   # Outputs: bin/Release/RestSharp.RequestBuilder.1.0.X.nupkg
   # Can be installed locally in test projects
   `

6. **Commit and Push**
   `powershell
   git add .
   git commit -m "Add custom header method with fluent syntax"
   git push origin feature/add-custom-header-method
   `

#### Build and Test Validation

**Check for Analyzer Warnings**
- CSharpAnalyzers issues appear as warnings
- SonarAnalyzer.CSharp issues appear as warnings
- Treat as errors in Release builds

**Verify All Tests Pass**
- MSTest must report 0 failures
- All test categories must pass (see UNIT_TESTS.instructions.md)

### Code Review Workflow

#### Pull Request Process
1. **Create PR** on GitHub
2. **PR Description**: Explain feature, link to issues
3. **Automated Checks**:
   - Build pipeline runs (dotnet build)
   - Tests run (dotnet test)
   - Analyzers report (CSharpAnalyzers, SonarAnalyzer)
4. **Manual Review**: Code review team verifies:
   - Adherence to CODING_STANDARDS.instructions.md
   - Test coverage (new methods must have tests)
   - API design consistency with IRequestBuilder
5. **Merge**: Squash or rebase to master

#### PR Validation Checklist
- [ ] All tests pass locally and in CI
- [ ] New public methods have [TestMethod] coverage
- [ ] XML documentation complete
- [ ] No analyzer warnings
- [ ] Follows CODING_STANDARDS.instructions.md
- [ ] Fluent chaining preserved (IRequestBuilder return)
- [ ] Case-insensitive semantics respected (if applicable)
- [ ] No breaking changes to IRequestBuilder

### Bug Fix Workflow

Similar to feature development, but:
1. Create branch: git checkout -b fix/parameter-deduplication-bug
2. Add test case that reproduces bug
3. Implement fix in RequestBuilder.cs
4. Verify test passes
5. Create PR with "Fixes #123" in description

## CI/CD Pipeline

### Build Pipeline Stages

#### Stage 1: Source Restore
- **Command**: dotnet restore
- **Duration**: ~30 seconds
- **Output**: .nuget/ cache populated
- **Artifacts**: None

#### Stage 2: Build Solution
- **Command**: dotnet build RestSharp.RequestBuilder.sln -c Debug
- **Duration**: ~60 seconds
- **Analyzers**: CSharpAnalyzers and SonarAnalyzer run
- **Artifacts**:
  - bin/Debug/netstandard2.0/RestSharp.RequestBuilder.dll
  - bin/Debug/net8.0/, net9.0/ (test framework builds)
  - *.xml documentation files
- **Failure Handling**: Stop if compilation errors or analyzer warnings as errors

#### Stage 3: Unit Tests
- **Command**: dotnet test tests/RestSharp.RequestBuilder.UnitTests/
- **Duration**: ~45 seconds
- **Test Count**: 100+ MSTest methods
- **Reporting**: Test results to CI logs
- **Coverage**: Optional code coverage report
- **Failure Handling**: Stop if any test fails

#### Stage 4: Package Generation (Release Config)
- **Command**: dotnet pack src/RestSharp.RequestBuilder/ -c Release
- **Duration**: ~30 seconds
- **Condition**: Only on Release builds or main branch
- **Output**: bin/Release/RestSharp.RequestBuilder.1.0.X.nupkg
- **Artifacts**: NuGet package ready for publishing

#### Stage 5: Publishing (Optional)
- **Command**: dotnet nuget push bin/Release/*.nupkg -s https://api.nuget.org/v3/index.json
- **Condition**: Only on tagged releases or manual trigger
- **Authentication**: API key from secure vault
- **Success**: Package available on NuGet.org

### Pipeline Triggers

1. **Pull Request**: Runs full pipeline (restore, build, test)
   - Block merge on failure
   - Require analyzer pass

2. **Commit to Master**: Runs full pipeline + packaging
   - Creates NuGet package as artifact
   - Manual approval for NuGet.org publish

3. **Semantic Version Tag**: Triggers pipeline + publish
   - Tag format: v1.0.0
   - Auto-publishes to NuGet.org

### Pipeline Configuration

**Location**: .github/workflows/ (GitHub Actions assumed)

**Workflow File Example** (github-ci.yml):
`yaml
name: Build and Test
on: [pull_request, push]
jobs:
  build:
    runs-on: windows-latest
    steps:
      - uses: actions/checkout@v3
      - uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '8.0.x'
      - run: dotnet restore
      - run: dotnet build -c Release
      - run: dotnet test
      - run: dotnet pack -c Release
`

## Build Commands Reference

### Common Build Commands

`powershell
# Debug Build (default)
dotnet build RestSharp.RequestBuilder.sln
# Output: bin/Debug/netstandard2.0/RestSharp.RequestBuilder.dll

# Release Build (optimized, for publishing)
dotnet build RestSharp.RequestBuilder.sln -c Release
# Output: bin/Release/netstandard2.0/RestSharp.RequestBuilder.dll

# Build Specific Project
dotnet build src/RestSharp.RequestBuilder/RestSharp.RequestBuilder.csproj

# Build Tests Only
dotnet build tests/RestSharp.RequestBuilder.UnitTests/

# Clean Build (remove bin, obj)
dotnet clean
dotnet build
`

### Test Commands

`powershell
# Run All Tests
dotnet test tests/RestSharp.RequestBuilder.UnitTests/

# Run Specific Test Method
dotnet test --filter "AddHeader_ValidInput_ReturnsBuilder"

# Run Tests with Verbose Output
dotnet test -v detailed

# Run Tests with Code Coverage
dotnet test /p:CollectCoverage=true /p:CoverageFormat=cobertura

# Run Tests for Specific Framework
dotnet test -f net8.0
`

### Package Commands

`powershell
# Pack for NuGet (local)
dotnet pack src/RestSharp.RequestBuilder/ -c Release
# Output: bin/Release/RestSharp.RequestBuilder.1.0.2.nupkg

# Install Locally
dotnet nuget add source ./bin/Release --name local
dotnet add package RestSharp.RequestBuilder --version 1.0.2 --source local

# Publish to NuGet.org
dotnet nuget push bin/Release/RestSharp.RequestBuilder.1.0.2.nupkg -s https://api.nuget.org/v3/index.json -k <API_KEY>
`

## Release Management

### Versioning Strategy

- **Format**: Semantic Versioning (Major.Minor.Patch)
- **Example**: 1.0.2
- **Location**: src/RestSharp.RequestBuilder/RestSharp.RequestBuilder.csproj
  - Property: <AssemblyVersion>1.0.2</AssemblyVersion>
  - Property: <Version>1.0.2</Version>

### Release Process

1. **Feature Merge**: Feature branch merged to master
2. **Version Bump**: Update AssemblyVersion in csproj
3. **Changelog**: Update CHANGELOG.md (optional but recommended)
4. **Git Tag**: Create tag (v1.0.2)
   `powershell
   git tag v1.0.2
   git push origin v1.0.2
   `
5. **CI Trigger**: Tag triggers release pipeline
6. **Package Generation**: NuGet package auto-generated
7. **Publish**: Manual or auto-publish to NuGet.org
8. **GitHub Release**: Create release note on GitHub

### Hotfix Process

For urgent production issues:

1. **Create Hotfix Branch** from master
   `powershell
   git checkout -b hotfix/critical-parameter-bug master
   `
2. **Fix and Test**: Implement fix with tests
3. **Merge Back**: Merge to master
4. **Version Bump**: Increment patch version (1.0.1 -> 1.0.2)
5. **Release**: Follow release process above
6. **Merge to Dev**: (if separate dev branch exists)

## Automation Opportunities

### Current State
- ✅ Automated build and test on PR
- ✅ Automated analyzer checks
- ✅ Automated package generation

### Improvement Opportunities

1. **Code Coverage Reporting**
   - **Tool**: Coverlet + CodeCov integration
   - **Benefit**: Track coverage percentage over time
   - **Action**: Add /p:CollectCoverage=true to test step

2. **Semantic Release**
   - **Tool**: semantic-release or GitVersion
   - **Benefit**: Automatic version bumping based on commit messages
   - **Action**: Parse commit messages (feat:, fix:, BREAKING:) to determine version

3. **Automated NuGet Publishing**
   - **Tool**: GitHub Actions with NuGet credentials
   - **Benefit**: No manual publish steps
   - **Action**: Auto-publish on successful merge to master

4. **Documentation Generation**
   - **Tool**: DocFX or Sandcastle
   - **Benefit**: Auto-generate API docs from XML comments
   - **Action**: Generate docs on release, publish to GitHub Pages

5. **Performance Regression Testing**
   - **Tool**: BenchmarkDotNet
   - **Benefit**: Detect performance regressions before merge
   - **Action**: Benchmark AddParameters() for large arrays

6. **Security Scanning**
   - **Tool**: OWASP Dependency-Check or GitHub Security Scanning
   - **Benefit**: Detect vulnerable transitive dependencies
   - **Action**: Scan nuget.org feed during build

## Bottlenecks and Constraints

### Current Bottlenecks

1. **Manual Version Bumping**
   - **Impact**: Risk of version misalignment
   - **Solution**: Automate with semantic-release or GitVersion

2. **Manual NuGet Publishing**
   - **Impact**: Delayed public availability
   - **Solution**: Automate with GitHub Actions

3. **No API Compatibility Checks**
   - **Impact**: Accidental breaking changes
   - **Solution**: Add tool like APICompat to CI

### Infrastructure Constraints

- **Build Time**: ~3-5 minutes for full pipeline
  - Acceptable for CI; too slow for tight feedback loop
  - Optimization: Parallel test execution, incremental builds

- **Test Framework**: MSTest
  - Adequate; well-integrated with VS and CI
  - Alternative: xUnit (requires migration effort)

- **Analyzer Warnings**: May become noisy as codebase grows
  - Mitigation: Regular review of analyzer settings

## Developer Experience Improvements

### Recommended Enhancements

1. **Local Build Script**
   `powershell
   # build.ps1
   param([string] = "Debug")
   dotnet clean
   dotnet restore
   dotnet build -c 
   dotnet test
   `

2. **Pre-commit Hooks**
   - Run formatters and linters before commit
   - Block commits with analyzer warnings

3. **IDE Templates**
   - VS Code snippet for new test method (TestMethod)
   - Rider live template for fluent builder pattern

4. **Documentation Automation**
   - Generate README examples from integration tests
   - Auto-link to relevant issues/PRs

## Deployment Environments

### Development
- **Target**: Local machine and CI agents
- **Configuration**: Debug build
- **Testing**: Full MSTest suite
- **Artifacts**: bin/Debug/

### Staging
- **Target**: NuGet.org preview feed (optional)
- **Configuration**: Release build
- **Testing**: Integration tests with real RestSharp
- **Artifacts**: Pre-release NuGet package

### Production
- **Target**: NuGet.org
- **Configuration**: Release build
- **Testing**: Full test suite on release branch
- **Artifacts**: Official NuGet package
- **Verification**: SourceLink validation, symbol upload

## Troubleshooting Common Issues

### Build Failures

**Issue**: dotnet build fails with analyzer warnings
- **Solution**: Check analyzer settings in Directory.Build.props
- **Action**: dotnet build -p:TreatWarningsAsErrors=false (temporary)

**Issue**: Test failures on specific framework
- **Solution**: Run specific framework test: dotnet test -f net8.0
- **Action**: Check .NET version on machine matches global.json

**Issue**: Package generation fails
- **Solution**: Verify version format in csproj (must be SemVer)
- **Action**: Check for duplicate <Version> properties

### CI/CD Failures

**Issue**: Pipeline timeout
- **Solution**: Increase timeout in workflow file
- **Action**: Investigate slow stage (often test execution)

**Issue**: Package publish fails
- **Solution**: Verify API key in vault
- **Action**: Re-authenticate with nuget.org

## Monitoring and Metrics

### Build Metrics to Track

1. **Build Time**: Should stay < 5 minutes
2. **Test Pass Rate**: Should stay at 100%
3. **Code Coverage**: Target > 80%
4. **Analyzer Warnings**: Trend downward

### Tools

- **GitHub Actions Dashboard**: Monitor pipeline runs
- **NuGet.org Stats**: Download count, version history
- **Dependabot**: Track dependency updates

---

**Last Updated**: December 2025
**Maintainer**: RestSharp.RequestBuilder Team
**Assumption**: GitHub Actions for CI/CD (can adapt for other platforms)
