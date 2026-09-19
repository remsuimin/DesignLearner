using DesignPatternMaster.Core.Enums;

namespace DesignPatternMaster.Core.Entities;

/// <summary>GoFデザインパターンの集約ルート。不変条件はコンストラクタで保証する。</summary>
public sealed class DesignPattern
{
    private readonly List<Section> _sections = new();
    private readonly List<string> _tags = new();

    public string Id { get; }
    public string Name { get; }
    public string Summary { get; }
    public PatternCategory Category { get; }
    public DifficultyLevel Difficulty { get; }
    public bool IsAntiPattern { get; }
    public bool IsModern { get; }
    public string ModernRelevance { get; }
    public string? IconPath { get; }
    public IReadOnlyList<Section> Sections => _sections;
    public IReadOnlyList<string> Tags => _tags;

    public DesignPattern(
        string id,
        string name,
        string summary,
        PatternCategory category,
        DifficultyLevel difficulty,
        bool isAntiPattern = false,
        bool isModern = false,
        string? modernRelevance = "",
        string? iconPath = null,
        IReadOnlyList<Section>? sections = null,
        IReadOnlyList<string>? tags = null)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Id must not be empty.", nameof(id));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name must not be empty.", nameof(name));
        if (string.IsNullOrWhiteSpace(summary))
            throw new ArgumentException("Summary must not be empty.", nameof(summary));

        Id = id.Trim();
        Name = name.Trim();
        Summary = summary.Trim();
        Category = category;
        Difficulty = difficulty;
        IsAntiPattern = isAntiPattern;
        IsModern = isModern;
        ModernRelevance = modernRelevance ?? string.Empty;
        IconPath = string.IsNullOrWhiteSpace(iconPath) ? null : iconPath.Trim();

        if (sections is not null)
        {
            foreach (var section in sections)
            {
                AddSection(section);
            }
        }

        if (tags is not null)
        {
            foreach (var tag in tags)
            {
                AddTag(tag);
            }
        }
    }

    public void AddSection(Section section)
    {
        ArgumentNullException.ThrowIfNull(section);
        _sections.Add(section);
    }

    public void AddTag(string tag)
    {
        if (string.IsNullOrWhiteSpace(tag))
            throw new ArgumentException("Tag must not be empty.", nameof(tag));

        var normalized = tag.Trim();
        if (_tags.Contains(normalized, StringComparer.Ordinal))
            return;

        _tags.Add(normalized);
    }
}
