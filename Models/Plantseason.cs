using System;
using System.Collections.Generic;

namespace PlantsInformationWeb.Models;

public partial class Plantseason
{
    public int SeasonId { get; set; }

    public int? PlantId { get; set; }

    public string PlantingSeason { get; set; } = null!;

    public string HarvestingSeason { get; set; } = null!;

    public string? Notes { get; set; }

    public virtual Plant? Plant { get; set; }
}
