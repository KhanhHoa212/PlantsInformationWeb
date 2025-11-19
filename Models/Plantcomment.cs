using System;
using System.Collections.Generic;

namespace PlantsInformationWeb.Models;

public partial class Plantcomment
{
    public int CommentId { get; set; }

    public int PlantId { get; set; }

    public int? UserId { get; set; }

    public int? ParentCommentId { get; set; }

    public string CommentText { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual ICollection<Plantcomment> InverseParentComment { get; set; } = new List<Plantcomment>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual Plantcomment? ParentComment { get; set; }

    public virtual Plant Plant { get; set; } = null!;

    public virtual User? User {get; set;}
}
