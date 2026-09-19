namespace DesignPatternMaster.Core.Entities;

/// <summary>動作するコード例。Language はシンタックスハイライト用トークン（例: "csharp"）。</summary>
public sealed class CodeSample
{
    public string Language { get; }
    public string Code { get; }
    public string Description { get; }

    public CodeSample(string language, string code, string? description = "")
    {
        if (string.IsNullOrWhiteSpace(language))
            throw new ArgumentException("Language must not be empty.", nameof(language));
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Code must not be empty.", nameof(code));

        Language = language.Trim();
        Code = code;
        Description = description ?? string.Empty;
    }
}
