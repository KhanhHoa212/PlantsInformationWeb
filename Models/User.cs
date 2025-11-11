using System;
using System.Collections.Generic;

namespace PlantsInformationWeb.Models;

public partial class User
{
    public int UserId { get; set; }

    public string? Email { get; set; }

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string? Role { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Chatmessage> Chatmessages { get; set; } = new List<Chatmessage>();

    public virtual ICollection<Chatsession> Chatsessions { get; set; } = new List<Chatsession>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<Passwordresetrequest> Passwordresetrequests { get; set; } = new List<Passwordresetrequest>();

    public virtual ICollection<Plantcomment> Plantcomments { get; set; } = new List<Plantcomment>();

    public virtual ICollection<Plantsearchlog> Plantsearchlogs { get; set; } = new List<Plantsearchlog>();

    public virtual ICollection<Plantviewlog> Plantviewlogs { get; set; } = new List<Plantviewlog>();
}
