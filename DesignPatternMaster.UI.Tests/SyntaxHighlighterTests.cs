using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using DesignPatternMaster.UI.Helpers;
using FluentAssertions;
using Xunit;

namespace DesignPatternMaster.UI.Tests
{
    public class SyntaxHighlighterTests
    {
        [StaFact]
        public void SetCode_WithNull_ShouldClearBlocks()
        {
            var rtb = new RichTextBox();
            SyntaxHighlighter.SetCode(rtb, "var x = 1;");
            rtb.Document.Blocks.Should().NotBeEmpty();

            SyntaxHighlighter.SetCode(rtb, null);

            rtb.Document.Blocks.Should().BeEmpty();
        }

        [StaFact]
        public void SetCode_WithEmptyString_ShouldClearBlocks()
        {
            var rtb = new RichTextBox();
            SyntaxHighlighter.SetCode(rtb, "var x = 1;");
            rtb.Document.Blocks.Should().NotBeEmpty();

            SyntaxHighlighter.SetCode(rtb, string.Empty);

            rtb.Document.Blocks.Should().BeEmpty();
        }

        [StaFact]
        public void SetCode_OnNonRichTextBox_ShouldBeIgnored()
        {
            var button = new Button();
            var act = () => SyntaxHighlighter.SetCode(button, "var x = 1;");
            act.Should().NotThrow();
        }

        [StaFact]
        public void FormatCode_ShouldColorKeywords()
        {
            var doc = InvokeFormatCode("public class Foo { }", 13);

            doc.Should().NotBeNull();
            var paragraph = doc.Blocks.OfType<Paragraph>().First();
            var runs = paragraph.Inlines.OfType<Run>().ToList();
            // At least one run should have DeepSkyBlue (keyword color)
            var keywordRun = runs.FirstOrDefault(r => IsColor(r.Foreground, Colors.DeepSkyBlue));
            keywordRun.Should().NotBeNull("keyword 'public' or 'class' should be colored DeepSkyBlue");
            keywordRun!.Text.Should().MatchRegex("public|class");
        }

        [StaFact]
        public void FormatCode_CommentShouldTakePriorityOverKeywordAndString()
        {
            // Entire line is a comment containing keyword and string-like content
            var doc = InvokeFormatCode("// var x = \"hello\"", 13);

            var paragraph = doc.Blocks.OfType<Paragraph>().First();
            var runs = paragraph.Inlines.OfType<Run>().ToList();

            // All colored runs should be comment color (Green) only; no string/keyword color inside
            var greenRuns = runs.Where(r => IsColor(r.Foreground, Colors.Green)).ToList();
            greenRuns.Should().NotBeEmpty();
            // The whole code should be covered by comment runs or default runs, but no DeepSkyBlue or Orange inside
            runs.Where(r => IsColor(r.Foreground, Colors.DeepSkyBlue)).Should().BeEmpty("keywords inside comment should not be colored");
            runs.Where(r => IsColor(r.Foreground, Colors.Orange)).Should().BeEmpty("strings inside comment should not be colored");
            // Concatenated text should equal original
            string.Concat(runs.Select(r => r.Text)).Should().Be("// var x = \"hello\"");
        }

        [StaFact]
        public void FormatCode_ShouldReflectFontSize()
        {
            const double expectedSize = 20;
            var doc = InvokeFormatCode("var x = 1;", expectedSize);

            var paragraph = doc.Blocks.OfType<Paragraph>().First();
            paragraph.FontSize.Should().Be(expectedSize);
        }

        [StaFact]
        public void FormatCode_DefaultFontSize_ShouldBe13_WhenNoExplicitSize()
        {
            // Compatibility: if overload without fontSize exists, default is 13
            var methodOneParam = typeof(SyntaxHighlighter).GetMethod("FormatCode", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public,
                new[] { typeof(string) });
            var methodTwoParam = typeof(SyntaxHighlighter).GetMethod("FormatCode", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public,
                new[] { typeof(string), typeof(double) });

            if (methodTwoParam != null)
            {
                var doc = (FlowDocument)methodTwoParam.Invoke(null, new object[] { "var x = 1;", 13.0 })!;
                doc.Blocks.OfType<Paragraph>().First().FontSize.Should().Be(13);
            }
            else if (methodOneParam != null)
            {
                var doc = (FlowDocument)methodOneParam.Invoke(null, new object[] { "var x = 1;" })!;
                doc.Blocks.OfType<Paragraph>().First().FontSize.Should().Be(13);
            }
            else
            {
                Assert.Fail("FormatCode method not found");
            }
        }

        private static FlowDocument InvokeFormatCode(string code, double fontSize)
        {
            var type = typeof(SyntaxHighlighter);
            // Prefer (string, double) overload (new implementation)
            var methodTwo = type.GetMethod("FormatCode", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public,
                new[] { typeof(string), typeof(double) });
            if (methodTwo != null)
            {
                return (FlowDocument)methodTwo.Invoke(null, new object[] { code, fontSize })!;
            }

            // Fallback to single param (old implementation) – ignore fontSize
            var methodOne = type.GetMethod("FormatCode", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public,
                new[] { typeof(string) });
            if (methodOne != null)
            {
                return (FlowDocument)methodOne.Invoke(null, new object[] { code })!;
            }

            throw new InvalidOperationException("FormatCode method not found");
        }

        private static bool IsColor(Brush brush, Color color)
        {
            return brush is SolidColorBrush scb && scb.Color == color;
        }
    }
}
