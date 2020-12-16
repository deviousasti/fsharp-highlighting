# FSharp.Highlighting
A WPF control which displays F# Source Code with syntax highlighting, for embedding within tools and extensions - so that you don't have to rely on heavyweight editor controls to display some source. That was a run-on sentence. 

## Usage

Embed `Highlighter.fs`.

In code, you can create a control instance with:

```fsharp
new FSharp.Highlighting.Highlighter(
	SourceText = "let x = 42"
)
```

or in XAML

```xaml
<highlighting:Highlighter SourceText = "let x = 42" />
```

## What it looks like

![img](https://pbs.twimg.com/media/EpX_EZqUcAAEEmn?format=png&name=small)

## Dependencies

- WindowsBase
- FCS 38