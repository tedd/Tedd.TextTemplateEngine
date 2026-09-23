# Tedd.TextTemplateEngine

## Architectural Execution Flow

The `TextTemplateEngine` operates on a linear, high-performance `ReadOnlySpan<char>` parsing paradigm. It minimizes memory allocations by natively slicing strings without generating excessive intermediate `System.String` allocations. The framework incorporates a `TemplateMatchType` taxonomy, currently optimizing for the `FastNaive` execution strategy, while structurally anticipating more complex `Regex`-based string resolution paradigms.

The core replacement logic employs a dual-pointer system (coordinating the `appendPos` and `startPos` indices) to evaluate the template structure:
- Upon encountering an escape character, the preceding literal sequence is committed to an internal `StringBuilder` construct.
- Identification of a `StartChar` followed by an `EndChar` triggers the extraction of the enclosed substring as a target keyword. The engine subsequently casts this candidate to a `System.String` representation and executes a dictionary-based resolution against the bound `Dictionary<string, string>` context.
- Verified keyword matches dynamically integrate their respective dictionary values into the output buffer. If a keyword fails resolution, the parsing state automatically resets without mutating the original template string, thereby guaranteeing deterministic execution.

## Contemporary Implementation Example

```csharp
using System;
using System.Collections.Generic;
using Tedd;

string template = "System metrics for [Hostname] indicate operational status. Use \\[Escape\\] to emit raw delimiters.";

Dictionary<string, string> lookupContext = new()
{
    ["Hostname"] = "Nexus-Core-01"
};

// Execute template token replacement via ReadOnlySpan slicing
string renderedOutput = TextTemplateEngine.Replace(
    templateString: template,
    lookup: lookupContext,
    startChar: '[',
    endChar: ']',
    escapeChar: '\\'
);

Console.WriteLine(renderedOutput);
// Output: System metrics for Nexus-Core-01 indicate operational status. Use [Escape] to emit raw delimiters.
```

## Future Roadmap (Hypotheses)

The current framework evaluation restricts data resolution to static `Dictionary` keys. Future enhancements hypothesize introducing advanced data resolution mechanics:
* **Hierarchical Data Binding Contexts:** The theoretical integration of nested object graphs, enabling deep property path resolution (e.g., `[Server.Metrics.CPU]`) to supersede flat dictionary lookups.
* **Routed Event Infrastructure:** The anticipated deployment of an event-driven architecture, permitting dynamic parameter resolution at runtime via targeted callback execution rather than static pre-initialization.

*These features are strictly theoretical hypotheses and are not yet materialized within the execution pathways of the engine.*
