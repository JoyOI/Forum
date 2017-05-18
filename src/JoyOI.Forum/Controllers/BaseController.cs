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
        }

        [Inject]
        public IHubContext<ForumHub> ForumHub { get; set; }

        [Inject]
        public IEmailSender Mail { get; set; }

        [Inject]
        public AesCrypto Aes { get; set; }
    }
}
