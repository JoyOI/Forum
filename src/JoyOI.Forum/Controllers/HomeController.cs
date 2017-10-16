using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JoyOI.Forum.Models;
using JoyOI.UserCenter.SDK;

namespace JoyOI.Forum.Controllers
{
    public class HomeController : BaseController
    {
        public IActionResult Index()
        {
            return RedirectToAction("Index", "Forum");
        }

        public async Task<IActionResult> Summary(
            [FromServices] JoyOIUC UC,
            System.Threading.CancellationToken token)
        {
            var threads = await DB.Threads
                .Include(x => x.Forum)
                .Include(x => x.User)
                .Include(x => x.Posts)
                .Where(x => x.IsAnnouncement)
                .ToListAsync(token);

            IEnumerable<Thread> plainThreads = new List<Thread>();

            if (threads.Count < 10)
            {
                var count = 10 - threads.Count;
                plainThreads = await DB.Threads
                    .Include(x => x.Forum)
                    .Include(x => x.User)
                    .Include(x => x.Posts)
                    .Where(x => !x.IsAnnouncement)
                    .OrderByDescending(x => x.LastReplyTime)
                    .Take(count)
                    .ToListAsync(token);
            }

            var raw = threads.Concat(plainThreads);
            return Json(raw.Select(x => new {
                forumTitle =  x.Forum.Title,
                forumId = x.ForumId,
                threadId = x.Id,
                threadTitle = x.Title,
                time = x.LastReplyTime,
                userId = x.User.OpenId,
                username = x.User.UserName,
                avatarUrl = UC.GetAvatarUrl(x.User.OpenId),
                replyCount = x.Posts.Count
            }));
        }
    }
}
