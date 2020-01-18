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

        public async Task<ICollection<PointsModel>> GetPoints(Guid userId, int championId = -1)
        {
            return await ctx.Points
                .Where(p => p.User.Id == userId && (championId == -1 || p.ChampionId == championId))
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
