using System;
using System.Collections.Generic;

namespace PlantsInformationWeb.Models;

public partial class Passwordresetrequest
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public string? OtpCode { get; set; }

    public DateTime? ExpiresAt { get; set; }

    public bool? IsUsed { get; set; }

    public virtual User? User { get; set; }
}
