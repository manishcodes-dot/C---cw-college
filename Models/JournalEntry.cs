using System;
using System.Collections.Generic;

namespace YourAppName.Models
{
    public enum MoodCategory
    {
        Positive,
        Neutral,
        Negative
    }

    public class Mood
    {
        public string Name { get; set; }
        public MoodCategory Category { get; set; }
        public string Emoji { get; set; }
    }

    public class JournalEntry
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; }
        public string Content { get; set; } // Supports HTML/Markdown
        public DateTime Date { get; set; } = DateTime.Today;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public Mood PrimaryMood { get; set; }
        public List<Mood> SecondaryMoods { get; set; } = new List<Mood>();
        
        public string Category { get; set; }
        public List<string> Tags { get; set; } = new List<string>();

        public int WordCount => string.IsNullOrWhiteSpace(Content) ? 0 : Content.Split(new[] { ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Length;
    }
}
