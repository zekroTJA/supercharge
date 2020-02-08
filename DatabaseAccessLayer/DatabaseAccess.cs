using DatabaseAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatabaseAccessLayer
{
    public class DatabaseAccess
    {
        private readonly DatabaseContext ctx;

        public DatabaseAccess(DatabaseContext ctx)
        {
            this.ctx = ctx;
        }

        public async Task<ICollection<UserModel>> GetUsersAsync()
        {
            return await ctx.Users.ToArrayAsync();
        }

        public async Task<UserModel> GetUserByNameAsync(string server, string userName)
        {
            return await ctx.Users.FirstOrDefaultAsync(u => u.Server == server && u.Username == userName);
        }

        public async Task<UserModel> GetUserBySummonerIdAsync(string summonerId)
        {
            return await ctx.Users.FirstOrDefaultAsync(u => u.SummonerId == summonerId);
        }

        public async Task<ICollection<PointsModel>> GetPointsAsync(
            Guid userId,
            IEnumerable<int> championIds = null)
        {
            return await ctx.Points
                .Where(p => p.User.Id == userId && ((championIds == null || championIds.Count() < 1) || championIds.Contains(p.ChampionId)))
                .OrderByDescending(p => p.ChampionPoints)
                .ToArrayAsync();
        }

        public async Task<ICollection<PointsViewModel>> GetPointsViewAsync(
            Guid userId = default,
            IEnumerable<int> championIds = null,
            int limit = int.MaxValue,
            int skip = 0)
        {
            return await ctx.Points
                .Where(p => (userId == default || p.User.Id == userId) && ((championIds == null || championIds.Count() < 1) || championIds.Contains(p.ChampionId)))
                .OrderByDescending(p => p.ChampionPoints)
                .Skip(skip)
                .Take(limit)
                .Select(p => new PointsViewModel(p))
                .ToArrayAsync();
        }

        public async Task<ICollection<PointsLogModel>> GetPointsLogAsync(
            Guid userId = default,
            IEnumerable<int> championIds = null,
            DateTime from = default,
            DateTime to = default,
            int limit = int.MaxValue,
            int skip = 0)
        {
            if (from == default)
                from = DateTime.Now.Subtract(TimeSpan.FromDays(30));

            if (to == default)
                to = DateTime.Now;

            return await ctx.PointsLog
                .Where(p => (userId == default || p.User.Id == userId) && ((championIds == null || championIds.Count() < 1) || championIds.Contains(p.ChampionId)))
                .Where(p => p.Timestamp >= from && p.Timestamp <= to)
                .OrderByDescending(p => p.Timestamp)
                .Skip(skip)
                .Take(limit)
                .ToArrayAsync();
        }

        public async Task<ICollection<PointsLogViewModel>> GetPointsLogViewAsync(
            Guid userId,
            IEnumerable<int> championIds = null,
            DateTime from = default,
            DateTime to = default)
        {
            if (from == default)
                from = DateTime.Now.Subtract(TimeSpan.FromDays(30));

            if (to == default)
                to = DateTime.Now;

            return await ctx.PointsLog
                .Where(p => p.User.Id == userId && ((championIds == null || championIds.Count() < 1) || championIds.Contains(p.ChampionId)))
                .Where(p => p.Timestamp >= from && p.Timestamp <= to)
                .OrderByDescending(p => p.Timestamp)
                .Select(p => new PointsLogViewModel(p))
                .ToArrayAsync();
        }

        public long GetUsersCount(Func<UserModel, bool> preticate) => 
            ctx.Users.Where(preticate).LongCount();

        public long GetPointsCount(Func<PointsModel, bool> preticate) =>
            ctx.Points.Where(preticate).LongCount();

        public long GetPointsLogCount(Func<PointsLogModel, bool> preticate) =>
            ctx.PointsLog.Where(preticate).LongCount();

        public void DeletePoints(Guid fromUserId)
        {
            var elements = ctx.Points.Where(p => p.User.Id == fromUserId);
            ctx.RemoveRange(elements);
        }

        public void DeletePointsLog(Guid fromUserId)
        {
            var elements = ctx.PointsLog.Where(p => p.User.Id == fromUserId);
            ctx.RemoveRange(elements);
        }

        public void DeleteUser(UserModel user) =>
            ctx.Remove(user);

        public void Update<T>(T d)
        {
            ctx.Update(d);
        }

        public void Add<T>(T d)
        {
            ctx.Add(d);
        }

        public async Task CommitChangesAsync()
        {
            await ctx.SaveChangesAsync();
        }
    }
}
