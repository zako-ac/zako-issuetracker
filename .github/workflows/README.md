# GitHub Actions Workflows

This directory contains CI/CD workflows for the Zako IssueTracker project.

## Workflows

### 1. Unit Tests (`test.yml`)
**Triggers:** On every push and pull request to any branch

**Purpose:** Quick feedback workflow that runs unit tests

**Steps:**
- ✅ Checkout code
- ✅ Setup .NET 9
- ✅ Restore dependencies
- ✅ Build solution
- ✅ Run all 26 unit tests
- ✅ Generate test summary

**Use Case:** Fast validation for every commit and PR

---

### 2. .NET Build and Test (`dotnet.yml`)
**Triggers:** On push/PR to `master` or `main` branches

**Purpose:** Standard .NET build and test workflow with detailed reporting

**Steps:**
- ✅ Checkout code
- ✅ Setup .NET 9
- ✅ Restore dependencies
- ✅ Build in Release mode
- ✅ Run tests with TRX logger
- ✅ Publish test results

**Use Case:** Main branch validation with test reporting

---

### 3. CI/CD (`ci-cd.yml`)
**Triggers:** On push/PR to `master` or `main` branches

**Purpose:** Comprehensive CI/CD with code quality checks and coverage

**Jobs:**

#### Job 1: Build and Test
- ✅ Build solution in Release mode
- ✅ Run tests with code coverage
- ✅ Generate coverage report (HTML)
- ✅ Upload test results as artifacts
- ✅ Upload coverage report as artifacts
- ✅ Generate test summary

#### Job 2: Code Quality Checks
- ✅ Check code formatting
- ✅ Build with warnings as errors
- ✅ Verify code quality standards

**Use Case:** Complete validation for main branches with quality gates

---

## Workflow Features

### ✨ Test Reporting
- Test results are saved as artifacts (7-day retention)
- TRX format for detailed test analysis
- Test summaries in GitHub Actions UI

### ✨ Code Coverage
- XPlat code coverage collection
- HTML coverage reports generated
- Coverage artifacts uploaded for review

### ✨ Code Quality
- Formatting verification with `dotnet format`
- Build with warnings treated as errors
- Quality gates for PR validation

---

## Environment Variables

All workflows use:
- `DOTNET_VERSION: 9.0.x` - .NET SDK version
- `DOTNET_SKIP_FIRST_TIME_EXPERIENCE: true` - Skip first-run experience
- `DOTNET_CLI_TELEMETRY_OPTOUT: true` - Disable telemetry

---

## Test Suite

Current test coverage: **26 tests**

### Test Projects
- **IssueSystemTests**: 18 tests for database operations
- **AdminToolTests**: 4 tests for admin validation
- **EnvLoaderTests**: 4 tests for environment configuration

### Test Execution
All tests run in isolated environments with:
- Temporary SQLite databases
- Sequential execution to prevent conflicts
- Automatic cleanup after completion

---

## Artifacts

### Test Results
- **Name:** `test-results`
- **Location:** `./coverage/**/*.trx`
- **Retention:** 7 days
- **Format:** TRX (MSTest format)

### Coverage Report
- **Name:** `coverage-report`
- **Location:** `./coveragereport`
- **Retention:** 7 days
- **Format:** HTML with detailed coverage metrics

---

## Branch Protection

Recommended GitHub branch protection rules for `master`/`main`:

```yaml
Required status checks:
  - Unit Tests (test.yml)
  - Build and Test (dotnet.yml)
  - Build and Test / build-and-test (ci-cd.yml)
  - Code Quality Checks / code-quality (ci-cd.yml)

Require branches to be up to date: ✅
Require linear history: ✅
```

---

## Local Testing

Before pushing, run the same checks locally:

```bash
# Run tests
dotnet test --verbosity normal

# Check formatting
dotnet format --verify-no-changes

# Build with Release configuration
dotnet build --configuration Release

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

---

## Troubleshooting

### Tests failing in CI but passing locally?
- Check .NET version matches (9.0.x)
- Verify all dependencies are restored
- Check for environment-specific issues

### Coverage report not generating?
- Ensure `dotnet-reportgenerator-globaltool` is installed
- Verify coverage.cobertura.xml files exist
- Check coverage path in workflow

### Workflow not triggering?
- Verify branch names match trigger patterns
- Check workflow file YAML syntax
- Ensure workflows are enabled in repository settings

---

## Future Enhancements

Planned improvements:
- [ ] Add Docker build and push
- [ ] Implement deployment workflows
- [ ] Add performance benchmarking
- [ ] Integrate with code quality services (SonarQube, CodeQL)
- [ ] Add dependency scanning
- [ ] Implement automated releases

---

## References

- [GitHub Actions Documentation](https://docs.github.com/en/actions)
- [.NET GitHub Actions](https://docs.github.com/en/actions/automating-builds-and-tests/building-and-testing-net)
- [dotnet test Documentation](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-test)
