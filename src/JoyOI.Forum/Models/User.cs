using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace JoyOI.Forum.Models
{
    public class User : IdentityUser<Guid>
    {
        public DateTime RegisteryTime { get; set; }

        [MaxLength(512)]
        public string Motto { get; set; }

        [MaxLength(64)]
        public string Nickname { get; set; }

        [MaxLength(64)]
        public string AccessToken { get; set; }

        public DateTime ExpireTime { get; set; }

        public Guid OpenId { get; set; }

        [MaxLength(256)]
        public string AvatarUrl { get; set; }
    }
}
