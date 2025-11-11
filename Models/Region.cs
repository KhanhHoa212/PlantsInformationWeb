using System;
using System.Collections.Generic;

namespace PlantsInformationWeb.Models;

public partial class Region
{
    public int RegionId { get; set; }

    public string RegionName { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Plant> Plants { get; set; } = new List<Plant>();
}
