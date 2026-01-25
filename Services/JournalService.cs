using System;
using System.Collections.Generic;
using System.Linq;
using YourAppName.Models;

namespace YourAppName.Services
{
    public class JournalService
    {
        private List<JournalEntry> _entries = new List<JournalEntry>();

        public JournalService()
        {
            AddSampleData();
        }

        public List<JournalEntry> GetAllEntries() => _entries.OrderByDescending(e => e.Date).ToList();

        public JournalEntry GetEntryByDate(DateTime date) => _entries.FirstOrDefault(e => e.Date.Date == date.Date);

        public void SaveEntry(JournalEntry entry)
        {
            var existing = _entries.FirstOrDefault(e => e.Date.Date == entry.Date.Date);
            if (existing != null)
            {
                existing.Title = entry.Title;
                existing.Content = entry.Content;
                existing.PrimaryMood = entry.PrimaryMood;
                existing.SecondaryMoods = entry.SecondaryMoods;
                existing.Category = entry.Category;
                existing.Tags = entry.Tags;
                existing.UpdatedAt = DateTime.Now;
            }
            else
            {
                _entries.Add(entry);
            }
        }

        public void DeleteEntry(Guid id)
        {
            var entry = _entries.FirstOrDefault(e => e.Id == id);
            if (entry != null)
            {
                _entries.Remove(entry);
            }
        }

        public List<JournalEntry> Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return GetAllEntries();
            query = query.ToLower();
            return _entries.Where(e => 
                (e.Title?.ToLower().Contains(query) ?? false) || 
                (e.Content?.ToLower().Contains(query) ?? false) ||
                (e.Tags.Any(t => t.ToLower().Contains(query)))
            ).ToList();
        }

        private void AddSampleData()
        {
            var positiveMoods = new List<Mood> {
                new Mood { Name = "Happy", Category = MoodCategory.Positive, Emoji = "😊" },
                new Mood { Name = "Grateful", Category = MoodCategory.Positive, Emoji = "🙏" }
            };

            _entries.Add(new JournalEntry
            {
                Title = "A Productive Day",
                Content = "I finished all my tasks and went for a run. Feeling great!",
                Date = DateTime.Today.AddDays(-1),
                PrimaryMood = positiveMoods[0],
                Category = "Work",
                Tags = new List<string> { "Exercise", "Work" }
            });

            _entries.Add(new JournalEntry
            {
                Title = "Relaxing Sunday",
                Content = "Spent the day reading and meditation. Very peaceful morning.",
                Date = DateTime.Today.AddDays(-2),
                PrimaryMood = new Mood { Name = "Calm", Category = MoodCategory.Neutral, Emoji = "😐" },
                Category = "Self-care",
                Tags = new List<string> { "Reading", "Meditation" }
            });
        }

        public int GetCurrentStreak()
        {
            int streak = 0;
            DateTime date = DateTime.Today;
            while (_entries.Any(e => e.Date.Date == date.Date))
            {
                streak++;
                date = date.AddDays(-1);
            }
            return streak;
        }

        public int GetLongestStreak()
        {
            if (!_entries.Any()) return 0;
            var dates = _entries.Select(e => e.Date.Date).Distinct().OrderByDescending(d => d).ToList();
            int longest = 0;
            int current = 0;
            
            for (int i = 0; i < dates.Count; i++)
            {
                current = 1;
                while (i + 1 < dates.Count && (dates[i] - dates[i+1]).Days == 1)
                {
                    current++;
                    i++;
                }
                if (current > longest) longest = current;
            }
            return longest;
        }

        public List<DateTime> GetMissedDays(DateTime start, DateTime end)
        {
            var missed = new List<DateTime>();
            for (var d = start.Date; d <= end.Date; d = d.AddDays(1))
            {
                if (!_entries.Any(e => e.Date.Date == d))
                {
                    missed.Add(d);
                }
            }
            return missed;
        }
    }
}
