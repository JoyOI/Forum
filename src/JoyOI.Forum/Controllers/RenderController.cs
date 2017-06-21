using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pomelo.Marked;

namespace JoyOI.Forum.Controllers
{
    public class RenderController : BaseController
    {
        public IActionResult Post(Guid id)
        {
            var post = DB.Posts
                .Include(x => x.Thread)
                .ThenInclude(x => x.User)
                .Include(x => x.User)
                .Include(x => x.SubPosts)
                .ThenInclude(x => x.User)
                .Where(x => x.Id == id)
                .SingleOrDefault();
            if (post == null)
            {
                Response.StatusCode = 404;
                return Content("");
            }
            return View(post);
        }

        public IActionResult PostContent(Guid id)
        {
            var post = DB.Posts
                .Where(x => x.Id == id)
                .Select(x => x.Content)
                .SingleOrDefault();
            return Content(post);
        }

        public IActionResult Thread(long id)
        {
            var thread = DB.Threads
                .Include(x => x.User)
                .Include(x => x.Posts)
                .Where(x => x.Id == id)
                .SingleOrDefault();
            thread.LastPost = DB.Posts
                .Where(x => x.ThreadId == thread.Id)
                .OrderByDescending(x => x.Time)
                .FirstOrDefault();
            if (thread == null)
                return Content("");
            return View(thread);
        }

        public IActionResult ThreadContent(long id)
        {
            var thread = DB.Threads
                .Where(x => x.Id == id)
                .Select(x => x.Content)
                .SingleOrDefault();
            if (thread == null)
                return Content("");
            else
                return Content(Instance.Parse(thread));
        }

        public IActionResult UserThread(Guid id)
        {
            return PagedView(DB.Threads
                .Where(x => x.UserId == id)
                .OrderByDescending(x => x.CreationTime), 10);
        }

        public IActionResult UserPost(Guid id)
        {
            return PagedView(DB.Posts
                .Include(x => x.Thread)
                .Where(x => x.UserId == id)
                .OrderByDescending(x => x.Time), 10);
        }
    }
}
