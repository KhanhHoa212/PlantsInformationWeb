using System;
using System.Collections.Generic;

namespace PlantsInformationWeb.Models;

public partial class Plantcategory
{
    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Plant> Plants { get; set; } = new List<Plant>();
}
