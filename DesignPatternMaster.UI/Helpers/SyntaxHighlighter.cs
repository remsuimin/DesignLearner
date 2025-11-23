using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace DesignPatternMaster.UI.Helpers
{
    public static class SyntaxHighlighter
    {
        public static readonly DependencyProperty CodeProperty =
            DependencyProperty.RegisterAttached(
                "Code",
                typeof(string),
                typeof(SyntaxHighlighter),
                new PropertyMetadata(null, OnCodeChanged));

        public static string GetCode(DependencyObject obj)
        {
            return (string)obj.GetValue(CodeProperty);
        }

        public static void SetCode(DependencyObject obj, string value)
        {
            obj.SetValue(CodeProperty, value);
        }

        private static void OnCodeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is RichTextBox richTextBox)
            {
                var code = e.NewValue as string;
                if (string.IsNullOrEmpty(code))
                {
                    richTextBox.Document.Blocks.Clear();
                    return;
                }

                richTextBox.Document = FormatCode(code);
            }
        }

        private static FlowDocument FormatCode(string code)
        {
            var document = new FlowDocument();
            var paragraph = new Paragraph();
            
            // Default style
            paragraph.FontFamily = new FontFamily("Consolas, Courier New");
            paragraph.FontSize = 13;
            paragraph.Foreground = new SolidColorBrush(Color.FromRgb(220, 220, 220)); // Light Gray

            // Split by lines to handle comments easier (optional, but regex can handle it)
            // For simplicity, we'll use a single run and colorize ranges, but RichTextBox needs distinct Runs for colors.
            // A simple approach: Tokenize the string and create Runs.
            
            // Regex definitions
            var keywords = @"\b(public|private|protected|internal|class|interface|abstract|override|virtual|static|readonly|void|int|string|bool|var|new|return|if|else|for|foreach|in|while|using|namespace|get|set|this|base|null|true|false)\b";
            var strings = "\"[^\"]*\"";
            var comments = @"//.*|/\*[\s\S]*?\*/";

            // We need to parse the text and identify matches.
            // Since matches can overlap (e.g. string inside comment? no, comment wins usually), order matters.
            // A simple tokenizer loop is safer than replacing text.

            // Let's use a list of tokens
            var tokens = new List<(int Index, int Length, Brush Color)>();

            // 1. Find Comments (Highest priority)
            foreach (Match match in Regex.Matches(code, comments))
            {
                tokens.Add((match.Index, match.Length, Brushes.Green));
            }

            // 2. Find Strings (High priority, check overlap)
            foreach (Match match in Regex.Matches(code, strings))
            {
                if (!IsOverlapped(tokens, match.Index, match.Length))
                {
                    tokens.Add((match.Index, match.Length, Brushes.Orange));
                }
            }

            // 3. Find Keywords
            foreach (Match match in Regex.Matches(code, keywords))
            {
                if (!IsOverlapped(tokens, match.Index, match.Length))
                {
                    tokens.Add((match.Index, match.Length, Brushes.DeepSkyBlue));
                }
            }

            // Sort tokens by index
            tokens.Sort((a, b) => a.Index.CompareTo(b.Index));

            int currentIndex = 0;
            foreach (var token in tokens)
            {
                // Add non-colored text before token
                if (token.Index > currentIndex)
                {
                    paragraph.Inlines.Add(new Run(code.Substring(currentIndex, token.Index - currentIndex)));
                }

                // Add colored token
                var run = new Run(code.Substring(token.Index, token.Length));
                run.Foreground = token.Color;
                paragraph.Inlines.Add(run);

                currentIndex = token.Index + token.Length;
            }

            // Add remaining text
            if (currentIndex < code.Length)
            {
                paragraph.Inlines.Add(new Run(code.Substring(currentIndex)));
            }

            document.Blocks.Add(paragraph);
            return document;
        }

        private static bool IsOverlapped(List<(int Index, int Length, Brush Color)> tokens, int index, int length)
        {
            foreach (var token in tokens)
            {
                int tokenEnd = token.Index + token.Length;
                int reqEnd = index + length;

                if (index < tokenEnd && reqEnd > token.Index)
                    return true;
            }
            return false;
        }
    }
}
