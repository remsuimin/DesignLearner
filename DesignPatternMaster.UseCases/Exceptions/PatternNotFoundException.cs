namespace DesignPatternMaster.UseCases.Exceptions;

/// <summary>指定 ID のデザインパターンが存在しない場合の例外。</summary>
public sealed class PatternNotFoundException : Exception
{
    public string PatternId { get; }

    public PatternNotFoundException(string patternId)
        : base($"Design pattern '{patternId}' was not found.")
    {
        PatternId = patternId;
    }
}
