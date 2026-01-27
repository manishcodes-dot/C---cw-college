using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using SQLite;

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
        public string Name { get; set; } = string.Empty;
        public MoodCategory Category { get; set; }
        public string Emoji { get; set; } = string.Empty;
    }

    public class JournalEntry
    {
        [PrimaryKey]
        public Guid Id { get; set; } = Guid.NewGuid();
        
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty; // Supports HTML/Markdown
        
        public DateTime Date { get; set; } = DateTime.Today;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // SQLite doesn't support objects/lists, so we use string properties for DB storage
        [Ignore]
        public Mood? PrimaryMood { get; set; }
        
        public string PrimaryMoodJson 
        { 
            get => JsonSerializer.Serialize(PrimaryMood);
            set => PrimaryMood = string.IsNullOrEmpty(value) ? null : JsonSerializer.Deserialize<Mood>(value);
        }

        [Ignore]
        public List<Mood> SecondaryMoods { get; set; } = new List<Mood>();
        
        public string SecondaryMoodsJson 
        { 
            get => JsonSerializer.Serialize(SecondaryMoods);
            set => SecondaryMoods = string.IsNullOrEmpty(value) ? new List<Mood>() : JsonSerializer.Deserialize<List<Mood>>(value) ?? new List<Mood>();
        }
        
        public string Category { get; set; } = string.Empty;

        [Ignore]
        public List<string> Tags { get; set; } = new List<string>();

        public string TagsJson 
        { 
            get => JsonSerializer.Serialize(Tags);
            set => Tags = string.IsNullOrEmpty(value) ? new List<string>() : JsonSerializer.Deserialize<List<string>>(value) ?? new List<string>();
        }

        public string Pin { get; set; } = string.Empty; // Optional PIN for security

        [Ignore]
        public string PlainTextContent 
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Content)) return string.Empty;
                // Improved regex to strip HTML tags and handle line breaks
                var stripped = System.Text.RegularExpressions.Regex.Replace(Content, "<.*?>", " ");
                return System.Net.WebUtility.HtmlDecode(stripped).Trim();
            }
        }

        [Ignore]
        public int WordCount 
        {
            get
            {
                var text = PlainTextContent;
                if (string.IsNullOrWhiteSpace(text)) return 0;
                return text.Split(new[] { ' ', '\r', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries).Length;
            }
        }
    }
}
