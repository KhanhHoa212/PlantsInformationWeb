using System;
using System.Collections.Generic;

namespace PlantsInformationWeb.Models;

public partial class Favoriteplant
{
    public int FavoriteId { get; set; }

    public int? UserId { get; set; }

    public int? PlantId { get; set; }

    public DateTime? FavoritedAt { get; set; }

    public virtual Plant? Plant { get; set; }
}
