using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlantsInformationWeb.DTOs
{
    public class UnrecognizedplantDto
    {
        public int UnrecognizedId { get; set; }

        public string Plantname { get; set; } = null!;

        public DateTime? Createdat { get; set; }
        public string Usermessage { get; set; } = null!;
    }
}