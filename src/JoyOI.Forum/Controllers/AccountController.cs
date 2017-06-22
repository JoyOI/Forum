using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using JoyOI.Forum.Models;
using JoyOI.UserCenter.SDK;

namespace JoyOI.Forum.Controllers
{
    public class AccountController : BaseController
    {
        private static Random _random = new Random();

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(
            string username, 
            string password, 
            bool remember, 
            [FromHeader] string Referer,
            [FromServices] JoyOIUC UC)
        {
            var authorizeResult = await UC.TrustedAuthorizeAsync(username, password);
            if (authorizeResult.succeeded)
            {
                var profileResult = await UC.GetUserProfileAsync(authorizeResult.data.open_id, authorizeResult.data.access_token);

                User user = await UserManager.FindByNameAsync(username);

                if (user == null)
                {
                    user = new User
                    {
                        Id = authorizeResult.data.open_id,
                        UserName = username,
                        Email = profileResult.data.email,
                        PhoneNumber = profileResult.data.phone,
                        AccessToken = authorizeResult.data.access_token,
                        ExpireTime = authorizeResult.data.expire_time,
                        OpenId = authorizeResult.data.open_id,
                        AvatarUrl = UC.GetAvatarUrl(authorizeResult.data.open_id)
                    };

                    await UserManager.CreateAsync(user, password);
                }

                var roles = await UserManager.GetRolesAsync(user);

                if (authorizeResult.data.is_root)
                {
                    if (!roles.Any(x => x == "Root"))
                        await UserManager.AddToRoleAsync(user, "Root");
                }
                else
                {
                    if (roles.Any(x => x == "Root"))
                        await UserManager.RemoveFromRoleAsync(user, "Root");
                }
                
                await SignInManager.SignInAsync(user, true);
                user.LastLoginTime = DateTime.Now;
                DB.SaveChanges();

                return string.IsNullOrEmpty(Referer) ? (IActionResult)RedirectToAction("Index", "Home") : (IActionResult)Redirect(Referer);
            }
            else
            {
                return Prompt(x =>
                {
                    x.Title = "登录失败";
                    x.Details = authorizeResult.msg;
                    x.StatusCode = authorizeResult.code;
                });
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            return Redirect(Configuration["JoyOI:RegisterUrl"]);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await SignInManager.SignOutAsync();
            return Prompt(x =>
            {
                x.Title = "您已注销";
                x.Details = "您已成功注销了登录状态。";
                x.RedirectText = "重新登录";
                x.RedirectUrl = Url.Action("Login", "Account");
                x.HideBack = true;
            });
        }
        
        public async Task<IActionResult> Profile(string id)
        {
            var user = DB.Users.Where(x => x.UserName == id).SingleOrDefault();
            if (user == null)
                return Prompt(x =>
                {
                    x.Title = "资源没有找到";
                    x.Details = "您请求的资源没有找到，请返回重试！";
                    x.StatusCode = 404;
                });

            ViewBag.ImageId = _random.Next() % 21 + 1;
            ViewBag.Role = (await User.Manager.GetRolesAsync(user)).FirstOrDefault() ?? "Member";
            ViewBag.TotalThreads = DB.Threads.Count(x => x.UserId == user.Id);
            ViewBag.TotalPosts = DB.Posts.Count(x => x.UserId == user.Id);
            return View(user);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit(string id)
        {
            var user = DB.Users.Where(x => x.UserName == id).SingleOrDefault();
            if (user == null)
                return Prompt(x =>
                {
                    x.Title = "资源没有找到";
                    x.Details = "您请求的资源没有找到，请返回重试！";
                    x.StatusCode = 404;
                });
            var roles = await UserManager.GetRolesAsync(user);
            if (roles.Contains("Root") && !User.IsInRole("Root"))
                return Prompt(x =>
                {
                    x.Title = "没有权限";
                    x.Details = "您的权限不足以编辑该用户，请使用更高权限帐号执行本操作。";
                    x.StatusCode = 403;
                });
            if (User.Current.UserName != id && !User.AnyRoles("Root, Master"))
                return Prompt(x =>
                {
                    x.Title = "没有权限";
                    x.Details = "您的权限不足以编辑该用户，请使用更高权限帐号执行本操作。";
                    x.StatusCode = 403;
                });
            return View(user);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, IFormFile avatar, User Model)
        {
            var user = DB.Users
                .Where(x => x.UserName == id).SingleOrDefault();
            if (user == null)
                return Prompt(x =>
                {
                    x.Title = "资源没有找到";
                    x.Details = "您请求的资源没有找到，请返回重试！";
                    x.StatusCode = 404;
                });
            var roles = await UserManager.GetRolesAsync(user);
            if (roles.Contains("Root") && !User.IsInRole("Root"))
                return Prompt(x =>
                {
                    x.Title = "没有权限";
                    x.Details = "您的权限不足以编辑该用户，请使用更高权限帐号执行本操作。";
                    x.StatusCode = 403;
                });
            if (User.Current.UserName != id && !User.AnyRoles("Root, Master"))
                return Prompt(x =>
                {
                    x.Title = "没有权限";
                    x.Details = "您的权限不足以编辑该用户，请使用更高权限帐号执行本操作。";
                    x.StatusCode = 403;
                });
            if (avatar != null)
            {
                // TODO: Call user center change avatar api.
            }
            user.Motto = Model.Motto;
            DB.SaveChanges();
            return Prompt(x =>
            {
                x.Title = "修改成功";
                x.Details = "用户资料已经保存成功！";
            });
        }

        [HttpGet]
        [AnyRoles("Root, Master")]
        public async Task<IActionResult> Role(long id)
        {
            var user = await UserManager.FindByIdAsync(id.ToString());
            if (User.AnyRoles("Master") && await UserManager.IsInAnyRolesAsync(user, "Root, Master"))
            {
                return Prompt(x =>
                {
                    x.Title = "权限不足";
                    x.Details = "您无权编辑这个用户的角色！";
                });
            }
            else
            {
                ViewBag.Roles = DB.Roles
                    .Select(x => x.Name)
                    .ToList();
                ViewBag.CurrentRole = (await UserManager.GetRolesAsync(user)).First();
                return View(user);
            }
        }

        [HttpPost]
        [AnyRoles("Root")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Role(Guid id, string Role)
        {
            var user = await UserManager.FindByIdAsync(id.ToString());
            if (!User.IsInRole("Root") || await User.Manager.IsInRoleAsync(user, "Root"))
                return Prompt(x =>
                {
                    x.Title = "权限不足";
                    x.Details = "您无权编辑这个用户的角色！";
                    x.StatusCode = 500;
                });
            await UserManager.RemoveFromRolesAsync(user, await UserManager.GetRolesAsync(user));
            await UserManager.AddToRoleAsync(user, Role);
            return Prompt(x =>
            {
                x.Title = "修改成功";
                x.Details = $"用户{user.UserName}已经成为了{Role}！";
            });
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Password(long id)
        {
            var user = await UserManager.FindByIdAsync(id.ToString());
            if (User.AnyRoles("Master") && await UserManager.IsInAnyRolesAsync(user, "Root, Master") && User.Current.Id != user.Id)
                return Prompt(x =>
                {
                    x.Title = "权限不足";
                    x.Details = "您无权修改这个用户的密码！";
                    x.StatusCode = 500;
                });
            return View(user);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Password(long id, string old, string password, string confirm)
        {
            if (confirm != password)
                return Prompt(x =>
                {
                    x.Title = "修改失败";
                    x.Details = "两次密码输入不一致，请返回重试！";
                    x.StatusCode = 400;
                });
            var user = await UserManager.FindByIdAsync(id.ToString());
            if (User.AnyRoles("Master") && await UserManager.IsInAnyRolesAsync(user, "Root, Master") && User.Current.Id != user.Id)
                return Prompt(x =>
                {
                    x.Title = "权限不足";
                    x.Details = "您无权修改这个用户的密码！";
                    x.StatusCode = 500;
                });
            if (!User.AnyRoles("Root, Master") && !await UserManager.CheckPasswordAsync(user, old))
                return Prompt(x =>
                {
                    x.Title = "修改失败";
                    x.Details = "旧密码不正确，请返回重试！";
                    x.StatusCode = 400;
                });
            var token = await UserManager.GeneratePasswordResetTokenAsync(user);
            await UserManager.ResetPasswordAsync(user, token, password);
            return Prompt(x =>
            {
                x.Title = "修改成功";
                x.Details = "新密码已生效！";
            });
        }
    }
}
