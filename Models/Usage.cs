using System;
using System.Collections.Generic;

namespace PlantsInformationWeb.Models;

public partial class Usage
{
    public int UsageId { get; set; }

    public int? PlantId { get; set; }

    public string UsageType { get; set; } = null!;

    public string Details { get; set; } = null!;

    public virtual Plant? Plant { get; set; }
}
