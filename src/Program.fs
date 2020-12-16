open System
open System.IO
open System.Windows
open System.Windows.Controls

type Grid with
    member this.Children
        with set elements = elements |> Seq.iter (this.Children.Add >> ignore)

[<EntryPoint>]
[<STAThread>]
let main argv =

    let wpfApp = new Application()
    let text = File.ReadAllText argv.[0]

    
    Window(
        Title = "Highlighter Demo",
        Content = ScrollViewer(
            Content = FSharp.Highlighting.Highlighter(SourceText = text),
            Padding = Thickness 10.,
            HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto
        ),
        Width = 500.,
        Height = 500.,
        FontSize = 16.
    )
    |> wpfApp.Run
