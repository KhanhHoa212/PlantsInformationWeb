using System;
using System.Collections.Generic;

namespace PlantsInformationWeb.Models;

public partial class Plant
{
    public int PlantId { get; set; }

    public int? CategoryId { get; set; }

    public int? ClimateId { get; set; }

    public string PlantName { get; set; } = null!;

    public string? ScientificName { get; set; }

    public string? OtherNames { get; set; }

    public string? Origin { get; set; }

    public string GrowthCycle { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string Status { get; set; } = null!;

    public virtual Plantcategory? Category { get; set; }

    public virtual Climate? Climate { get; set; }

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<Nutritionalvalue> Nutritionalvalues { get; set; } = new List<Nutritionalvalue>();

    public virtual ICollection<Plantcare> Plantcares { get; set; } = new List<Plantcare>();

    public virtual ICollection<Plantcomment> Plantcomments { get; set; } = new List<Plantcomment>();

    public virtual ICollection<Plantimage> Plantimages { get; set; } = new List<Plantimage>();

    public virtual ICollection<Plantsearchresultlog> Plantsearchresultlogs { get; set; } = new List<Plantsearchresultlog>();

    public virtual ICollection<Plantseason> Plantseasons { get; set; } = new List<Plantseason>();

    public virtual ICollection<Plantviewlog> Plantviewlogs { get; set; } = new List<Plantviewlog>();

    public virtual ICollection<Reference> References { get; set; } = new List<Reference>();

    public virtual ICollection<Usage> Usages { get; set; } = new List<Usage>();

    public virtual ICollection<Disease> Diseases { get; set; } = new List<Disease>();

    public virtual ICollection<Region> Regions { get; set; } = new List<Region>();

    public virtual ICollection<Soiltype> Soils { get; set; } = new List<Soiltype>();
}
