using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using JoyOI.Forum.Models;

namespace JoyOI.Forum.Controllers
{
    public class AdminController : BaseController
    {
        [AnyRoles("Root")]
        public IActionResult Forum()
        {
            var ret = DB.Forums
                .Include(x => x.SubForums)
                .Where(x => x.ParentId == null)
                .OrderBy(x => x.PRI)
                .ToList();
            return View(ret);
        }

        [HttpGet]
        [AnyRoles("Root")]
        public IActionResult EditForum(string id)
        {
            return View(DB.Forums.Single(x => x.Id == id));
        }

        [HttpPost]
        [AnyRoles("Root")]
        [ValidateAntiForgeryToken]
        public IActionResult EditForum(string id, int pri, string title)
        {
            var forum = DB.Forums.Single(x => x.Id == id);
            forum.Title = title;
            forum.PRI = pri;
            DB.SaveChanges();
            return Prompt(x =>
            {
                x.Title = "修改成功";
                x.Details = "论坛板块信息保存成功！";
                x.RedirectText = "返回论坛管理";
                x.RedirectUrl = Url.Action("Forum", "Admin");
            });
        }

        [HttpGet]
        [AnyRoles("Root")]
        public IActionResult CreateForum()
        {
            return View();
        }

        [HttpPost]
        [AnyRoles("Root")]
        [ValidateAntiForgeryToken]
        public IActionResult CreateForum(string parentid, string title, int pri, string id)
        {
            if (DB.Forums.SingleOrDefault(x => x.Id == id) != null)
                return Prompt(x =>
                {
                    x.Title = "创建失败";
                    x.Details = "板块标识已经存在，请返回修改后重试！";
                });
            var forum = new Models.Forum { ParentId = parentid, Title = title, PRI = pri, Id = id };
            DB.Forums.Add(forum);
            DB.SaveChanges();
            return Prompt(x =>
            {
                x.Title = "创建成功";
                x.Details = "新的论坛版块已经创建成功";
                x.RedirectText = "返回论坛版块管理";
                x.RedirectUrl = Url.Action("Forum", "Admin");
            });
        }

        [AnyRoles("Root")]
        public IActionResult DeleteForum(string id)
        {
            var forum = DB.Forums.Where(x => x.Id == id).Single();
            DB.Forums.Remove(forum);
            DB.SaveChanges();
            return Prompt(x =>
            {
                x.Title = "删除成功";
                x.Details = "板块已经成功删除";
                x.HideBack = true;
                x.RedirectText = "返回论坛版块管理";
                x.RedirectUrl = Url.Action("Forum", "Admin");
            });
        }
    }
}
