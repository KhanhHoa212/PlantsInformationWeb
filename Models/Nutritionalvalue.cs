using System;
using System.Collections.Generic;

namespace PlantsInformationWeb.Models;

public partial class Nutritionalvalue
{
    public int NutritionId { get; set; }

    public int? PlantId { get; set; }

    public string NutrientName { get; set; } = null!;

    public string Amount { get; set; } = null!;

    public string? Benefit { get; set; }

    public virtual Plant? Plant { get; set; }
}
