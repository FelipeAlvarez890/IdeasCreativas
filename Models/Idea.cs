using System;
using System.ComponentModel.DataAnnotations;

namespace IdeasCreativas.Models
{
    public class Idea
    {
        public int Id { get; set; }

        [Required]
        public string Text { get; set; } = string.Empty;

        public DateTime PostDate { get; set; }

        public bool IsCreative { get; set; }
        public bool IsWellFormulated { get; set; }
        public bool IsApproved { get; set; }
        public bool IsRejected { get; set; }
        public string? RejectionReason { get; set; }

        public int TeamId { get; set; }
        public Team? Team { get; set; }
    }
}
