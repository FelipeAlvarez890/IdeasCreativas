using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IdeasCreativas.Models
{
    public class Team
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        public int MemberCount { get; set; }

        [Required]
        public string Member1 { get; set; } = string.Empty;

        public string? Member2 { get; set; }

        public List<Idea> Ideas { get; set; } = new List<Idea>();
    }
}
