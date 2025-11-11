using System;
using System.Collections.Generic;

namespace PlantsInformationWeb.Models;

public partial class Chatmessage
{
    public int MessageId { get; set; }

    public int? SessionId { get; set; }

    public int? UserId { get; set; }

    public string SenderType { get; set; } = null!;

    public string MessageText { get; set; } = null!;

    public DateTime? SentAt { get; set; }

    public virtual Chatsession? Session { get; set; }

    public virtual User? User { get; set; }
}
