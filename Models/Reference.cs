using System;
using System.Collections.Generic;

namespace PlantsInformationWeb.Models;

public partial class Reference
{
    public int ReferenceId { get; set; }

    public int? PlantId { get; set; }

    public string SourceName { get; set; } = null!;

    public string Url { get; set; } = null!;

    public virtual Plant? Plant { get; set; }
}
