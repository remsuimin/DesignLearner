using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace DesignPatternMaster.UI.Helpers;

public static class SyntaxHighlighter
{
    private static readonly TimeSpan MatchTimeout = TimeSpan.FromSeconds(2);

    private static readonly Regex KeywordRegex = new(
        @"\b(abstract|as|async|await|base|bool|break|byte|case|catch|char|checked|class|const|continue|decimal|default|do|double|else|enum|event|explicit|extern|false|file|finally|fixed|float|for|foreach|get|global|goto|if|implicit|in|init|int|interface|internal|is|lock|long|namespace|new|null|object|operator|out|override|params|private|protected|public|readonly|record|ref|required|return|sbyte|sealed|set|short|sizeof|stackalloc|static|string|struct|switch|this|throw|true|try|typeof|uint|ulong|unchecked|unsafe|ushort|using|var|virtual|void|volatile|while)\b",
        RegexOptions.Compiled,
        MatchTimeout);

    private static readonly Regex StringRegex = new(
        @"@\$?""(?:""""|[^""])*""|\$?""(?:[^""\\]|\\.)*""|'(?:[^'\\]|\\.)*'",
        RegexOptions.Compiled,
        MatchTimeout);

    private static readonly Regex CommentRegex = new(
        @"//.*|/\*[\s\S]*?\*/",
        RegexOptions.Compiled,
        MatchTimeout);

    private static readonly SolidColorBrush CommentBrush;
    private static readonly SolidColorBrush StringBrush;
    private static readonly SolidColorBrush KeywordBrush;
    private static readonly SolidColorBrush DefaultBrush;

    static SyntaxHighlighter()
    {
        if (SystemParameters.HighContrast)
        {
            CommentBrush = (SolidColorBrush)SystemColors.GrayTextBrush;
            StringBrush = (SolidColorBrush)SystemColors.HighlightBrush;
            KeywordBrush = (SolidColorBrush)SystemColors.HotTrackBrush;
            DefaultBrush = (SolidColorBrush)SystemColors.WindowTextBrush;
        }
        else
        {
            CommentBrush = new SolidColorBrush(Colors.Green);
            StringBrush = new SolidColorBrush(Colors.Orange);
            KeywordBrush = new SolidColorBrush(Colors.DeepSkyBlue);
            DefaultBrush = new SolidColorBrush(Color.FromRgb(220, 220, 220));
        }

        CommentBrush.Freeze();
        StringBrush.Freeze();
        KeywordBrush.Freeze();
        DefaultBrush.Freeze();
    }

    public static readonly DependencyProperty CodeProperty =
        DependencyProperty.RegisterAttached(
            "Code",
            typeof(string),
            typeof(SyntaxHighlighter),
            new PropertyMetadata(null, OnCodeChanged));

    public static string? GetCode(DependencyObject obj)
    {
        return (string?)obj.GetValue(CodeProperty);
    }

    public static void SetCode(DependencyObject obj, string? value)
    {
        obj.SetValue(CodeProperty, value);
    }

    public static readonly DependencyProperty FontSizeProperty =
        DependencyProperty.RegisterAttached(
            "FontSize",
            typeof(double),
            typeof(SyntaxHighlighter),
            new PropertyMetadata(13.0, OnCodeChanged));

    public static double GetFontSize(DependencyObject obj)
    {
        return (double)obj.GetValue(FontSizeProperty);
    }

    public static void SetFontSize(DependencyObject obj, double value)
    {
        obj.SetValue(FontSizeProperty, value);
    }

    private static void OnCodeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is RichTextBox richTextBox)
        {
            var code = GetCode(d);
            if (string.IsNullOrEmpty(code))
            {
                richTextBox.Document.Blocks.Clear();
                return;
            }

            richTextBox.Document = FormatCode(code, GetFontSize(d));
        }
    }

    internal static FlowDocument FormatCode(string code, double fontSize)
    {
        var document = new FlowDocument();
        var paragraph = new Paragraph
        {
            FontFamily = new FontFamily("Consolas, Courier New"),
            FontSize = fontSize,
            Foreground = DefaultBrush
        };

        // Priority: comments > strings/chars > keywords. Code samples are small,
        // so the three-pass scan with overlap checks is sufficient.
        var tokens = new List<(int Index, int Length, Brush Color)>();

        foreach (Match match in CommentRegex.Matches(code))
        {
            tokens.Add((match.Index, match.Length, CommentBrush));
        }

        foreach (Match match in StringRegex.Matches(code))
        {
            if (!IsOverlapped(tokens, match.Index, match.Length))
                tokens.Add((match.Index, match.Length, StringBrush));
        }

        foreach (Match match in KeywordRegex.Matches(code))
        {
            if (!IsOverlapped(tokens, match.Index, match.Length))
                tokens.Add((match.Index, match.Length, KeywordBrush));
        }

        tokens.Sort((a, b) => a.Index.CompareTo(b.Index));

        var currentIndex = 0;
        foreach (var token in tokens)
        {
            if (token.Index > currentIndex)
                paragraph.Inlines.Add(new Run(code.Substring(currentIndex, token.Index - currentIndex)));

            var run = new Run(code.Substring(token.Index, token.Length))
            {
                Foreground = token.Color
            };
            paragraph.Inlines.Add(run);

            currentIndex = token.Index + token.Length;
        }

        if (currentIndex < code.Length)
            paragraph.Inlines.Add(new Run(code.Substring(currentIndex)));

        document.Blocks.Add(paragraph);
        return document;
    }

    private static bool IsOverlapped(List<(int Index, int Length, Brush Color)> tokens, int index, int length)
    {
        foreach (var token in tokens)
        {
            var tokenEnd = token.Index + token.Length;
            var reqEnd = index + length;

            if (index < tokenEnd && reqEnd > token.Index)
                return true;
        }

        return false;
    }
}
