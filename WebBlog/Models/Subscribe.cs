﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBlog.Models
{
    public class Subscribe
    {
        public Guid Id { get; set; }
        [Required]
        public User Creator { get; set; }
        [ForeignKey("Creator")]
        public Guid CreatorId { get; set; }
        [Required]
        public User Target { get; set; }
        [ForeignKey("Target")]
        public Guid TargetId { get; set; }
        public Subscribe()
        {

        }
        public Subscribe(User creator, User target)
        {
            Id=Guid.NewGuid();
            Creator=creator;
            Target=target;
        }
    }
}
