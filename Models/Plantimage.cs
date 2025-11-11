using System;
using System.Collections.Generic;

namespace PlantsInformationWeb.Models;

public partial class Plantimage
{
    public int ImageId { get; set; }

    public int? PlantId { get; set; }

    public string ImageUrl { get; set; } = null!;

    public string? Description { get; set; }

    public bool? IsPrimary { get; set; }

    public virtual Plant? Plant { get; set; }
}
