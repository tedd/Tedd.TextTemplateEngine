## 2026-06-02 - Target Framework and Package Metadata Drift

**Observation:** The main library project `Tedd.TextTemplateEngine.csproj` targets `net8.0` but does not multi-target to include older platforms (such as `netstandard2.0`) to ensure broad compatibility. It is also missing NuGet package metadata. The test project has outdated testing packages (`xunit` 2.4.0, `Microsoft.NET.Test.Sdk` 16.2.0, `coverlet.collector` 1.0.1, `Tedd.RandomUtils` 1.0.2).

**Strategic Action:** Implement multi-targeting (`netstandard2.0;net8.0`) in the main project to preserve broad compatibility while supporting modern enhancements. Add essential NuGet metadata to `Tedd.TextTemplateEngine.csproj`. Update test dependencies to latest stable versions.

## 2026-10-02 - Dependency Modernization and Package Metadata Updates

**Observation:** Test project dependencies (`xunit`, `xunit.runner.visualstudio`, `coverlet.collector`, `coverlet.msbuild`, `Microsoft.NET.Test.Sdk`, `Tedd.RandomUtils`) and `System.Memory` package reference needed to be updated to latest stable versions. NuGet package metadata (e.g. `PackageTags`, `PackageProjectUrl`) was missing.

**Strategic Action:** Updated test dependencies to latest stable versions in `Tedd.TextTemplateEngine.Tests.csproj` and `System.Memory` to `4.6.3` in `Tedd.TextTemplateEngine.csproj`. Appended missing NuGet metadata `PackageTags` and `PackageProjectUrl` to ensure validation conformance.
