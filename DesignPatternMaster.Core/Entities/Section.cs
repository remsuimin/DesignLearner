namespace DesignPatternMaster.Core.Entities
{
    public class Section
    {
        public required string Title { get; set; }
        public required string Content { get; set; } // Markdown supported
        public CodeSample? CodeSample { get; set; }
        public string? ImagePath { get; set; }
    }
}
