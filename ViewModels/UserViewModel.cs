using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PlantsInformationWeb.ViewModels
{
    public class UserViewModel
    {
        internal readonly bool IsActive;

        [Required]
        public string? Username { get; set; }

        [Required, EmailAddress]
        public string? Email { get; set; }

        [Required]
        public string? Role { get; set; }

        [Required]
        public string? Password { get; set; }
        public object? CreatedAt { get; internal set; }
    }
}