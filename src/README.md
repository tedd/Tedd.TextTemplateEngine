# Tedd.TextTemplateEngine

## Architectural Execution Flow

The `TextTemplateEngine` operates on a linear, high-performance `ReadOnlySpan<char>` parsing paradigm. It minimizes memory allocations by slicing strings natively without creating excessive intermediate `System.String` allocations.

The replacement logic utilizes a dual-pointer system (managing `appendPos` and `startPos`) to traverse the template.
- When an escape character is encountered, the literal segment up to that character is appended to an internal `StringBuilder`.
- When a `StartChar` and subsequent `EndChar` are located, the substring is extracted as a keyword. The engine then converts that candidate keyword to a `string` and performs a dictionary-based lookup against the provided `Dictionary<string, string>`.
- Valid keyword matches dynamically append their corresponding dictionary value. If the keyword is absent, the parsing state resets without modifying the original template string, ensuring deterministic execution.

## Contemporary Implementation Example

```csharp
using System;
using System.Collections.Generic;
using Tedd;

class Program
{
    static void Main()
    {
        string template = "System metrics for [Hostname] indicate operational status. Use \\[Escape\\] to emit raw delimiters.";

        Dictionary<string, string> lookupContext = new()
        {
            { "Hostname", "Nexus-Core-01" }
        };

        // Execution of string interpolation via ReadOnlySpan slicing
        string renderedOutput = TextTemplateEngine.Replace(
            templateString: template,
            lookup: lookupContext,
            startChar: '[',
            endChar: ']',
            escapeChar: '\\'
        );

        Console.WriteLine(renderedOutput);
        // Output: System metrics for Nexus-Core-01 indicate operational status. Use [Escape] to emit raw delimiters.
    }
}
```

## Future Roadmap (Hypotheses)
The current framework restricts evaluation to static `Dictionary` keys. Future enhancements hypothesize introducing:
* Hierarchical Data Binding Contexts
* Routed Event Infrastructure for dynamically resolving parameters at runtime.

*These features are strictly theoretical hypotheses and are not yet materialized within the execution pathways of the engine.*
