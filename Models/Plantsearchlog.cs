using System;
using System.Collections.Generic;

namespace PlantsInformationWeb.Models;

public partial class Plantsearchlog
{
    public int SearchId { get; set; }

    public int? UserId { get; set; }

    public string? Keyword { get; set; }

    public DateTime? SearchedAt { get; set; }

    public string? FilterJson { get; set; }

    public virtual ICollection<Plantsearchresultlog> Plantsearchresultlogs { get; set; } = new List<Plantsearchresultlog>();
}
