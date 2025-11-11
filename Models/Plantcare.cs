using System;
using System.Collections.Generic;

namespace PlantsInformationWeb.Models;

public partial class Plantcare
{
    public int CareId { get; set; }

    public int? PlantId { get; set; }

    public string Season { get; set; } = null!;

    public string Watering { get; set; } = null!;

    public string Fertilizing { get; set; } = null!;

    public string? Harvesting { get; set; }

    public string? OtherTips { get; set; }

    public virtual Plant? Plant { get; set; }
}
