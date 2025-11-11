using System;
using System.Collections.Generic;

namespace PlantsInformationWeb.Models;

public partial class Climate
{
    public int ClimateId { get; set; }

    public string ClimateName { get; set; } = null!;

    public string? TemperatureRange { get; set; }

    public string? RainfallRange { get; set; }

    public string? HumidityRange { get; set; }

    public string? Description { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Plant> Plants { get; set; } = new List<Plant>();
}
