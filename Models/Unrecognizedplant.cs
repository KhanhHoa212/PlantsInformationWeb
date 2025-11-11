using System;
using System.Collections.Generic;

namespace PlantsInformationWeb.Models;

public partial class Unrecognizedplant
{
    public int UnrecognizedId { get; set; }

    public string Plantname { get; set; } = null!;

    public string Usermessage { get; set; } = null!;

    public DateTime? Createdat { get; set; }
}
