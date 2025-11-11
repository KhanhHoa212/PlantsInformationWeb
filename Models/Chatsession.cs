using System;
using System.Collections.Generic;

namespace PlantsInformationWeb.Models;

public partial class Chatsession
{
    public int SessionId { get; set; }

    public int? UserId { get; set; }

    public DateTime? StartedAt { get; set; }

    public DateTime? EndedAt { get; set; }

    public virtual ICollection<Chatmessage> Chatmessages { get; set; } = new List<Chatmessage>();

    public virtual User? User { get; set; }
}
