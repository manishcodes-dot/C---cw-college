using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SQLite;
using YourAppName.Models;

namespace YourAppName.Services
{
    public class JournalService
    {
        private readonly DatabaseService _databaseService;

        public JournalService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        private async Task<SQLiteAsyncConnection> GetDatabase() => await _databaseService.GetConnectionAsync();

        public async Task<List<JournalEntry>> GetAllEntries()
        {
            var db = await GetDatabase();
            return await db.Table<JournalEntry>().OrderByDescending(e => e.Date).ToListAsync();
        }

        public async Task<JournalEntry?> GetEntryById(Guid id)
        {
            var db = await GetDatabase();
            return await db.Table<JournalEntry>().FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<JournalEntry?> GetEntryByDate(DateTime date)
        {
            var db = await GetDatabase();
            var startOfDay = date.Date;
            var endOfDay = startOfDay.AddDays(1);
            
            // Note: Since SQLite stores dates as ticks or strings, we fetch and filter in memory for precision 
            // unless we want to use specific SQL functions. For a journal, fetching one day's worth is cheap.
            var entries = await db.Table<JournalEntry>().ToListAsync();
            return entries.FirstOrDefault(e => e.Date.Date == date.Date);
        }

        public async Task SaveEntry(JournalEntry entry)
        {
            var db = await GetDatabase();
            entry.UpdatedAt = DateTime.Now;

            // Check if entry exists by ID first
            var existingById = await GetEntryById(entry.Id);
            if (existingById != null)
            {
                await db.UpdateAsync(entry);
                return;
            }

            // Fallback: check if an entry exists for this specific date (business rule)
            var existingByDate = await GetEntryByDate(entry.Date);
            if (existingByDate != null)
            {
                entry.Id = existingByDate.Id; // Keep the same ID
                await db.UpdateAsync(entry);
            }
            else
            {
                if (entry.Id == Guid.Empty) entry.Id = Guid.NewGuid();
                await db.InsertAsync(entry);
            }
        }

        public async Task DeleteEntry(Guid id)
        {
            var db = await GetDatabase();
            await db.DeleteAsync<JournalEntry>(id);
        }

        public async Task<List<JournalEntry>> Search(string query, string? moodFilter = null, string? tagFilter = null)
        {
            var allEntries = await GetAllEntries();
            var filtered = allEntries.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(query))
            {
                var lowerQuery = query.ToLower();
                bool isDate = DateTime.TryParse(query, out var searchDate);

                filtered = filtered.Where(e =>
                    (isDate && e.Date.Date == searchDate.Date) ||
                    (e.Title?.ToLower().Contains(lowerQuery) ?? false) ||
                    (e.Content?.ToLower().Contains(lowerQuery) ?? false) ||
                    (e.Tags.Any(t => t.ToLower().Contains(lowerQuery))) ||
                    (e.Date.ToString("MMMM dd, yyyy").ToLower().Contains(lowerQuery)) ||
                    (e.Date.ToString("yyyy-MM-dd").ToLower().Contains(lowerQuery))
                );
            }

            if (!string.IsNullOrWhiteSpace(moodFilter))
            {
                filtered = filtered.Where(e => 
                    e.PrimaryMood?.Name == moodFilter || 
                    e.SecondaryMoods.Any(m => m.Name == moodFilter)
                );
            }

            if (!string.IsNullOrWhiteSpace(tagFilter))
            {
                filtered = filtered.Where(e => e.Tags.Contains(tagFilter, StringComparer.OrdinalIgnoreCase));
            }

            return filtered.ToList();
        }

        public async Task<int> GetCurrentStreak()
        {
            var entries = await GetAllEntries();
            int streak = 0;
            DateTime date = DateTime.Today;
            
            // Check if there's an entry for today. If not, check if there was one yesterday to keep streak alive.
            if (!entries.Any(e => e.Date.Date == date.Date))
            {
                date = date.AddDays(-1);
            }

            while (entries.Any(e => e.Date.Date == date.Date))
            {
                streak++;
                date = date.AddDays(-1);
            }
            return streak;
        }

        public async Task<int> GetLongestStreak()
        {
            var entries = await GetAllEntries();
            if (!entries.Any()) return 0;
            
            var dates = entries.Select(e => e.Date.Date).Distinct().OrderByDescending(d => d).ToList();
            int longest = 0;
            int current = 0;
            
            for (int i = 0; i < dates.Count; i++)
            {
                current = 1;
                while (i + 1 < dates.Count && (dates[i] - dates[i+1]).TotalDays <= 1.5) // Using 1.5 to handle slight date offsets
                {
                    if ((dates[i] - dates[i+1]).TotalDays >= 0.5) // Only count if it's a different day
                    {
                        current++;
                    }
                    i++;
                }
                if (current > longest) longest = current;
            }
            return longest;
        }

        public async Task<List<DateTime>> GetMissedDays(DateTime start, DateTime end)
        {
            var entries = await GetAllEntries();
            var entryDates = new HashSet<DateTime>(entries.Select(e => e.Date.Date));
            var missed = new List<DateTime>();
            
            for (var d = start.Date; d <= end.Date; d = d.AddDays(1))
            {
                if (!entryDates.Contains(d))
                {
                    missed.Add(d);
                }
            }
            return missed;
        }
    }
}
