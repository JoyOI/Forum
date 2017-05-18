using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Pomelo.Marked;

namespace JoyOI.Forum.Models
{
    public class Post
    {
        public Guid Id { get; set; }

        [ForeignKey("Thread")]
        public long ThreadId { get; set; }
        
        public virtual Thread Thread { get; set; }

        public string Content { get; set; }

        [NotMapped]
        public string FiltedContent
        {
            get
            {
                var ret = Helpers.RemoveHtml.Remove(Instance.Parse(Content));
                if (ret.Length > 100)
                    ret = ret.Substring(0, 100);
                return ret;
            }
        }

        public DateTime Time { get; set; }

        [ForeignKey("Parent")]
        public Guid? ParentId { get; set; }

        public virtual Post Parent { get; set; }

        public virtual ICollection<Post> SubPosts { get; set; } = new List<Post>();

        [ForeignKey("User")]
        public Guid? UserId { get; set; }

        public virtual User User { get; set; }
    }
}
