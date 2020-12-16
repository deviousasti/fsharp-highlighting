# Fsharp.Highlighting
An WPF control which displays F# Source Code with syntax highlighting, for embedding within tools and extensions so that you don't have to rely on heavyweight UI controls. 

Usage:

```fsharp
new FSharp.Highlighting.Highlighter(
	SourceText = "let x = 42"
)
```

## Dependencies

- WindowsBase
- FCS 38