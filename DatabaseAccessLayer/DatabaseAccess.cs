using DatabaseAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
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

        public void UpdateUser(UserModel user)
        {
            ctx.Users.Update(user);
        }

        public async Task CommitChangesAsync()
        {
            await ctx.SaveChangesAsync();
        }
    }
}
