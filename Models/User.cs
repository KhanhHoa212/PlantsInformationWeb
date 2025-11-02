using System;
using System.Collections.Generic;

namespace PlantsInformationWeb.Models;

public partial class User
{
    public int UserId { get; set; }

    public string? Email { get; set; }

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string? Role { get; set; }

    public DateTime? CreatedAt { get; set; }

    public bool? Isactive { get; set; }

    public virtual ICollection<Passwordresetrequest> Passwordresetrequests { get; set; } = new List<Passwordresetrequest>();
}
