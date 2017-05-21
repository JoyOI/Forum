using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace JoyOI.Forum.Models
{
    public static class SampleData
    {
        public static async Task InitDB(IServiceProvider services)
        {
            var DB = services.GetRequiredService<ForumContext>();
            var UserManager = services.GetRequiredService<UserManager<User>>();
            var RoleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
            if (DB.Database.EnsureCreated())
            {
                await RoleManager.CreateAsync(new IdentityRole<Guid>("Root"));
                await RoleManager.CreateAsync(new IdentityRole<Guid>("Master"));

                var user = new User { UserName = "root", Email = "someone@somedomain.com" };
                await UserManager.CreateAsync(user, "123456");
                await UserManager.AddToRoleAsync(user, "Root");

                var forum = new Forum { Title = "父板块1", Id = "forum-1" };
                var forum2 = new Forum { Title = "父板块2", Id = "forum-2" };
                var sub1 = new Forum { Title = "子板块1", ParentId = "forum-1", Id = "sub-1" };
                var sub2 = new Forum { Title = "子板块2", ParentId = "forum-1", Id = "sub-2" };
                var sub3 = new Forum { Title = "子板块3", ParentId = "forum-1", Id = "sub-3" };
                var sub4 = new Forum { Title = "子板块4", ParentId = "forum-2", Id = "sub-4" };
                var sub5 = new Forum { Title = "子板块5", ParentId = "forum-2", Id = "sub-5" };
                DB.Forums.Add(forum);
                DB.Forums.Add(forum2);
                DB.Forums.Add(sub1);
                DB.Forums.Add(sub2);
                DB.Forums.Add(sub3);
                DB.Forums.Add(sub4);
                DB.Forums.Add(sub5);
                DB.SaveChanges();
            }
        }
    }
}
