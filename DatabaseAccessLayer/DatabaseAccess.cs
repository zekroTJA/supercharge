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

        public async Task<ICollection<PointsViewModel>> GetPointsModelAsync(
            Guid userId,
            IEnumerable<int> championIds = null)
        {
            return await ctx.Points
                .Where(p => p.User.Id == userId && ((championIds == null || championIds.Count() < 1) || championIds.Contains(p.ChampionId)))
                .OrderByDescending(p => p.ChampionPoints)
                .Select(p => new PointsViewModel(p))
                .ToArrayAsync();
        }

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
