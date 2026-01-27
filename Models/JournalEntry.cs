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
        public string Pin { get; set; } // Optional PIN for security

        public int WordCount => string.IsNullOrWhiteSpace(Content) ? 0 : PlainTextContent.Split(new[] { ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Length;

        public string PlainTextContent 
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Content)) return string.Empty;
                // Simple regex to strip HTML tags for preview
                return System.Text.RegularExpressions.Regex.Replace(Content, "<.*?>", string.Empty);
            }
        }
    }
}
