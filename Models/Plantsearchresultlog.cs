using System;
using System.Collections.Generic;

namespace PlantsInformationWeb.Models;

public partial class Plantsearchresultlog
{
    public int ResultId { get; set; }

    public int? SearchId { get; set; }

    public int? PlantId { get; set; }

    public virtual Plant? Plant { get; set; }

    public virtual Plantsearchlog? Search { get; set; }
}
