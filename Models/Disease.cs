using System;
using System.Collections.Generic;

namespace PlantsInformationWeb.Models;

public partial class Disease
{
    public int DiseaseId { get; set; }

    public string DiseaseName { get; set; } = null!;

    public string Symptoms { get; set; } = null!;

    public string Solution { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Plant> Plants { get; set; } = new List<Plant>();
}
