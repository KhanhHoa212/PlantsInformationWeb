using System;
using System.Collections.Generic;

namespace PlantsInformationWeb.Models;

public partial class Notification
{
    public int NotificationId { get; set; }

    public int? UserId { get; set; }

    public int? CommentId { get; set; }

    public string Message { get; set; } = null!;

    public bool? IsRead { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? PlantId { get; set; }

    public virtual Plantcomment? Comment { get; set; }

    public virtual Plant? Plant { get; set; }
}
