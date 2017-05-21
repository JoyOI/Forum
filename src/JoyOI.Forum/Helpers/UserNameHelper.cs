using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Html;
using Microsoft.Extensions.DependencyInjection;
using JoyOI.Forum.Models;

namespace Microsoft.AspNetCore.Mvc.Rendering
{
    public static class UserNameHelper
    {
        public static async Task<HtmlString> ColorUserNameAsync(this IHtmlHelper self, User user, string @class = "", string tag = "span")
        {
            var UserManager = self.ViewContext.HttpContext.RequestServices.GetService<UserManager<User>>();
            if (await UserManager.IsInRoleAsync(user, "Root") || await UserManager.IsInRoleAsync(user, "Master"))
                return new HtmlString($"<{tag} class=\"{@class} user-master\">{user.UserName}</{tag}>");
            else
                return new HtmlString($"<{tag} class=\"{@class} user-member\">{user.UserName}</{tag}>");
        }
    }
}