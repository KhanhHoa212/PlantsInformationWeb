using System;
using System.Collections.Generic;

namespace PlantsInformationWeb.Models;

public partial class Soiltype
{
    public int SoilId { get; set; }

    public string SoilName { get; set; } = null!;

    public string? PhRange { get; set; }

    public string? FertilityLevel { get; set; }

    public string? Description { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Plant> Plants { get; set; } = new List<Plant>();
}
