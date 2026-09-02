## 2026-06-02 - Target Framework and Package Metadata Drift

**Observation:** The main library project `Tedd.TextTemplateEngine.csproj` targets `net8.0` but does not multi-target to include older platforms (such as `netstandard2.0`) to ensure broad compatibility. It is also missing NuGet package metadata. The test project has outdated testing packages (`xunit` 2.4.0, `Microsoft.NET.Test.Sdk` 16.2.0, `coverlet.collector` 1.0.1, `Tedd.RandomUtils` 1.0.2).

**Strategic Action:** Implement multi-targeting (`netstandard2.0;net8.0`) in the main project to preserve broad compatibility while supporting modern enhancements. Add essential NuGet metadata to `Tedd.TextTemplateEngine.csproj`. Update test dependencies to latest stable versions.
## 2026-09-02 - Target Framework and Package Metadata Drift

**Observation:** The main library project `Tedd.TextTemplateEngine.csproj` lacked the latest dependencies for `netstandard2.0`. The test project `Tedd.TextTemplateEngine.Tests.csproj` was using significantly outdated testing frameworks (xUnit 2.7.0, coverlet.collector 6.0.2, Microsoft.NET.Test.Sdk 17.9.0) which are deprecated or vulnerable.

**Strategic Action:** Updated `System.Memory` to 4.6.3 in the library. In the test project, upgraded `xunit` to 2.9.3, `xunit.runner.visualstudio` to 3.0.2, and `Microsoft.NET.Test.Sdk` to 17.13.0. Verified build, tests, and packing complete successfully without regression.
