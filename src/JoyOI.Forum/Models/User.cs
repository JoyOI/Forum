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
    }
}
