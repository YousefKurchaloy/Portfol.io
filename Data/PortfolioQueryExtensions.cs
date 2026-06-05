using System.Linq;
using Portfolio.Models;

namespace Portfolio.Data
{
    public static class PortfolioQueryExtensions
    {
        public static IQueryable<Skill> GetSortedSkillsForUser(this IQueryable<Skill> query, int userId)
        {
            return query
                .Where(s => s.ApplicationUserId == userId)
                .OrderBy(s => s.Category)
                .ThenBy(s => s.DisplayOrder);
        }

        public static IQueryable<Project> GetSortedProjectsForUser(this IQueryable<Project> query, int userId)
        {
            return query
                .Where(p => p.ApplicationUserId == userId)
                .OrderBy(p => p.DisplayOrder)
                .ThenByDescending(p => p.CompletionDate);
        }

        public static IQueryable<Credential> GetSortedCredentialsForUser(this IQueryable<Credential> query, int userId)
        {
            return query
                .Where(c => c.ApplicationUserId == userId)
                .OrderByDescending(c => c.IssueDate);
        }

        public static IQueryable<PlatformProfile> GetSortedPlatformProfilesForUser(this IQueryable<PlatformProfile> query, int userId)
        {
            return query
                .Where(p => p.ApplicationUserId == userId)
                .OrderBy(p => p.DisplayOrder);
        }

        public static IQueryable<TimelineEvent> GetSortedTimelineEventsForUser(this IQueryable<TimelineEvent> query, int userId)
        {
            return query
                .Where(e => e.ApplicationUserId == userId)
                .OrderByDescending(e => e.StartDate);
        }
    }
}
