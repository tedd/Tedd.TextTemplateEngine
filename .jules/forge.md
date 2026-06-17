## 2026-06-02 - Target Framework and Package Metadata Drift

**Observation:** The main library project `Tedd.TextTemplateEngine.csproj` targets `net8.0` but does not multi-target to include older platforms (such as `netstandard2.0`) to ensure broad compatibility. It is also missing NuGet package metadata. The test project has outdated testing packages (`xunit` 2.4.0, `Microsoft.NET.Test.Sdk` 16.2.0, `coverlet.collector` 1.0.1, `Tedd.RandomUtils` 1.0.2).

**Strategic Action:** Implement multi-targeting (`netstandard2.0;net8.0`) in the main project to preserve broad compatibility while supporting modern enhancements. Add essential NuGet metadata to `Tedd.TextTemplateEngine.csproj`. Update test dependencies to latest stable versions.
