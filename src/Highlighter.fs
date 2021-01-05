namespace FSharp.Highlighting

open System.Windows
open System.Windows.Controls
open FSharp.Compiler.SourceCodeServices
open System.Windows.Media

type CharKind = FSharpTokenCharKind
type ColorKind = FSharpTokenColorKind

module internal Tokenizer =

    [<Struct>]
    type Word =
        {
            Text: string
            Color: ColorKind
            Kind: CharKind
        }

    [<Struct>]
    type TextInline =
        | Span of Word
        | LineBoundary of line:int

    let rec scan (tokenizer: FSharpLineTokenizer) (line: string) state acc =
        match tokenizer.ScanToken(state) with
        | Some token, state ->
            let text =
                line.[token.LeftColumn..token.RightColumn]

            let word =
                {
                    Text = text
                    Color = token.ColorClass
                    Kind = token.CharClass
                }

            match acc with
            | [] -> [ word ]
            | { Kind = CharKind.WhiteSpace
                Text = text } :: tail -> { word with Text = text + word.Text } :: tail
            | prev :: tail when prev.Kind = word.Kind ->
                { prev with
                    Text = prev.Text + word.Text
                }
                :: tail
            | tail -> word :: tail
            |> scan tokenizer line state
        | None, state -> state, (List.rev acc)

    let scanLines lines (stokenize: FSharpSourceTokenizer) =
        lines
        |> Seq.scan
            (fun (state, acc) line -> scan (stokenize.CreateLineTokenizer line) line state [])
            (FSharpTokenizerLexState.Initial, [])
        |> Seq.map snd

    let scanText =
        let sourceTok =
            FSharpSourceTokenizer([], Some "Program.fsx")

        fun text ->
            let stext =
                FSharp.Compiler.Text.SourceText.ofString text

            let lines =
                seq { 0 .. stext.GetLineCount() - 1 }
                |> Seq.map stext.GetLineString

            sourceTok 
            |> scanLines lines
            |> Seq.indexed
            |> Seq.collect (fun (lineNum,wlist) -> seq { 
                yield! Seq.map Span wlist
                yield LineBoundary lineNum 
            })

module Theme =

    let Default =
        function
        | ColorKind.Default -> Brushes.White
        | ColorKind.Comment -> Brushes.DarkGreen
        | ColorKind.InactiveCode -> Brushes.Gray
        | ColorKind.Keyword -> Brushes.Blue
        | ColorKind.Identifier 
        | ColorKind.Operator
        | ColorKind.Punctuation -> Brushes.Black
        | ColorKind.String -> Brushes.Maroon
        | ColorKind.Number -> Brushes.DarkSlateBlue
        | ColorKind.UpperIdentifier -> Brushes.DarkCyan
        | ColorKind.PreprocessorKeyword -> Brushes.DarkBlue
        | _ -> Brushes.Black

type Highlighter() =
    inherit TextBlock(TextWrapping = TextWrapping.NoWrap, FontFamily = FontFamily("Consolas"))

    static let sourceTextProp =
        DependencyProperty.Register(
            "SourceText",
            typeof<string>,
            typeof<Highlighter>,
            new FrameworkPropertyMetadata(
                "",
                fun dobj _ ->
                    match dobj with
                    | :? Highlighter as h -> h.OnSourceChanged()
                    | _ -> ()
            )
        )

    let inlines = base.Inlines

    let mutable theme = Theme.Default
    
    static member SourceTextProperty = sourceTextProp
    member this.SourceText
        with get (): string = (string (this.GetValue sourceTextProp))
        and set (value: string) = this.SetValue(sourceTextProp, box value)

    member this.OnSourceChanged() =
        inlines.Clear()
        Tokenizer.scanText this.SourceText 
        |> Seq.map(function 
            | Tokenizer.Span word -> 
                Documents.Run(Text = word.Text, Foreground = theme word.Color) :> Documents.Inline
            | Tokenizer.LineBoundary _ ->                
                Documents.LineBreak() :> Documents.Inline
        )
        |> inlines.AddRange
    
    member this.Theme
           with get () = theme
           and set v = 
            theme <- v
            this.OnSourceChanged()