using System.Collections.Generic;

namespace DesignPatternMaster.Core.Entities
{
    public class DesignPattern
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public required string Summary { get; set; }
        public required string Category { get; set; } // e.g., Creational, Structural, Behavioral
        public required string Difficulty { get; set; } // e.g., Beginner, Intermediate, Advanced
        public bool IsAntiPattern { get; set; } // Considered an anti-pattern?
        public required string ModernRelevance { get; set; } // Explanation of its status today
        public string? IconPath { get; set; }
        public List<Section> Sections { get; set; } = new();
        public List<string> Tags { get; set; } = new();
    }
}
