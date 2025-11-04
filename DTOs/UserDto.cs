using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlantsInformationWeb.DTOs
{
    public class UserDto
    {
        public int UserId { get; set; }

        public string? Email { get; set; }

        public string Username { get; set; } = null!;

        public string? Role { get; set; }

        public DateTime? CreatedAt { get; set; }

        public bool? Isactive { get; set; }
    }
}