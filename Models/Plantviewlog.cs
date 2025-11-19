using System;
using System.Collections.Generic;

namespace PlantsInformationWeb.Models;

public partial class Plantviewlog
{
    public int ViewId { get; set; }

    public int? PlantId { get; set; }

    public int? UserId { get; set; }

    public DateTime? ViewedAt { get; set; }

    public virtual Plant? Plant { get; set; }
}
