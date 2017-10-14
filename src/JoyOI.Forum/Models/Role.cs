using System;
using Microsoft.AspNetCore.Identity;

namespace JoyOI.Forum.Models
{
    public class Role : IdentityRole<Guid>
    {
        public Role() { }
        public Role(string role) : base(role) { }
    }
}
