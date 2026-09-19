namespace DesignPatternMaster.Core.Entities;

/// <summary>パターン解説の1節。Markdown 本文＋任意のコード例・画像。</summary>
public sealed class Section
{
    public string Title { get; }
    public string Content { get; }
    public CodeSample? CodeSample { get; }
    public string? ImagePath { get; }

    public Section(string title, string content, CodeSample? codeSample = null, string? imagePath = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title must not be empty.", nameof(title));
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Content must not be empty.", nameof(content));

        Title = title.Trim();
        Content = content.Trim();
        CodeSample = codeSample;
        ImagePath = string.IsNullOrWhiteSpace(imagePath) ? null : imagePath.Trim();
    }
}
