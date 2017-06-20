using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Pomelo.Net.Smtp;
using JoyOI.Forum.Models;
using JoyOI.Forum.Hubs;

namespace JoyOI.Forum.Controllers
{
    public class BaseController : BaseController<ForumContext, User, Guid>
    {
        public override void Prepare()
        {
            base.Prepare();
            ViewBag.Announcements = DB.Threads
                .Where(x => x.IsAnnouncement)
                .OrderByDescending(x => x.CreationTime)
                .Take(5)
                .ToList();

            if (User.Current != null && User.Current.ActiveTime.AddMinutes(1) < DateTime.Now)
            {
                User.Current.ActiveTime = DateTime.Now;
                DB.SaveChanges();
            }
        }

        [Inject]
        public IHubContext<ForumHub> ForumHub { get; set; }

        [Inject]
        public AesCrypto Aes { get; set; }
    }
}
