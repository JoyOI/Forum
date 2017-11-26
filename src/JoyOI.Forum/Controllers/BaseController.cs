using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Pomelo.Net.Smtp;
using JoyOI.Forum.Models;
using JoyOI.Forum.Hubs;
using JoyOI.UserCenter.SDK;

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

            if (User.Current != null)
            {
                ViewBag.ChatUrl = UC.GenerateChatWindowUrl(User.Current.OpenId);
            }
        }

        [Inject]
        public JoyOIUC UC { get; set; }

        [Inject]
        public IHubContext<ForumHub> ForumHub { get; set; }

        [Inject]
        public AesCrypto Aes { get; set; }
    }
}
