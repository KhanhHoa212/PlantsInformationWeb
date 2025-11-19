using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace PlantsInformationWeb.Models;

public partial class PlantsInformationContext : DbContext
{
    public PlantsInformationContext()
    {
    }

    public PlantsInformationContext(DbContextOptions<PlantsInformationContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Chatmessage> Chatmessages { get; set; }

    public virtual DbSet<Chatsession> Chatsessions { get; set; }

    public virtual DbSet<Climate> Climates { get; set; }

    public virtual DbSet<Disease> Diseases { get; set; }

    public virtual DbSet<Favoriteplant> Favoriteplants { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Nutritionalvalue> Nutritionalvalues { get; set; }

    public virtual DbSet<Passwordresetrequest> Passwordresetrequests { get; set; }

    public virtual DbSet<Plant> Plants { get; set; }

    public virtual DbSet<Plantcare> Plantcares { get; set; }

    public virtual DbSet<Plantcategory> Plantcategories { get; set; }

    public virtual DbSet<Plantcomment> Plantcomments { get; set; }

    public virtual DbSet<Plantimage> Plantimages { get; set; }

    public virtual DbSet<Plantsearchlog> Plantsearchlogs { get; set; }

    public virtual DbSet<Plantsearchresultlog> Plantsearchresultlogs { get; set; }

    public virtual DbSet<Plantseason> Plantseasons { get; set; }

    public virtual DbSet<Plantviewlog> Plantviewlogs { get; set; }

    public virtual DbSet<Reference> References { get; set; }

    public virtual DbSet<Region> Regions { get; set; }

    public virtual DbSet<Soiltype> Soiltypes { get; set; }

    public virtual DbSet<Unrecognizedplant> Unrecognizedplants { get; set; }

    public virtual DbSet<Usage> Usages { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Database=PlantsInformation;Username=postgres;Password=123");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Chatmessage>(entity =>
        {
            entity.HasKey(e => e.MessageId).HasName("chatmessage_pkey");

            entity.ToTable("chatmessage");

            entity.Property(e => e.MessageId).HasColumnName("message_id");
            entity.Property(e => e.MessageText).HasColumnName("message_text");
            entity.Property(e => e.SenderType)
                .HasMaxLength(20)
                .HasColumnName("sender_type");
            entity.Property(e => e.SentAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("sent_at");
            entity.Property(e => e.SessionId).HasColumnName("session_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Session).WithMany(p => p.Chatmessages)
                .HasForeignKey(d => d.SessionId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("chatmessage_session_id_fkey");
        });

        modelBuilder.Entity<Chatsession>(entity =>
        {
            entity.HasKey(e => e.SessionId).HasName("chatsession_pkey");

            entity.ToTable("chatsession");

            entity.Property(e => e.SessionId).HasColumnName("session_id");
            entity.Property(e => e.EndedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("ended_at");
            entity.Property(e => e.StartedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("started_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");
        });

        modelBuilder.Entity<Climate>(entity =>
        {
            entity.HasKey(e => e.ClimateId).HasName("climate_pkey");

            entity.ToTable("climate");

            entity.Property(e => e.ClimateId).HasColumnName("climate_id");
            entity.Property(e => e.ClimateName)
                .HasMaxLength(100)
                .HasColumnName("climate_name");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.HumidityRange)
                .HasMaxLength(50)
                .HasColumnName("humidity_range");
            entity.Property(e => e.RainfallRange)
                .HasMaxLength(50)
                .HasColumnName("rainfall_range");
            entity.Property(e => e.TemperatureRange)
                .HasMaxLength(50)
                .HasColumnName("temperature_range");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Disease>(entity =>
        {
            entity.HasKey(e => e.DiseaseId).HasName("disease_pkey");

            entity.ToTable("disease");

            entity.Property(e => e.DiseaseId).HasColumnName("disease_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.DiseaseName)
                .HasMaxLength(150)
                .HasColumnName("disease_name");
            entity.Property(e => e.Solution).HasColumnName("solution");
            entity.Property(e => e.Symptoms).HasColumnName("symptoms");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Favoriteplant>(entity =>
        {
            entity.HasKey(e => e.FavoriteId).HasName("favoriteplant_pkey");

            entity.ToTable("favoriteplant");

            entity.Property(e => e.FavoriteId).HasColumnName("favorite_id");
            entity.Property(e => e.FavoritedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("favorited_at");
            entity.Property(e => e.PlantId).HasColumnName("plant_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Plant).WithMany(p => p.Favoriteplants)
                .HasForeignKey(d => d.PlantId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("favoriteplant_plant_id_fkey");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("notification_pkey");

            entity.ToTable("notification");

            entity.Property(e => e.NotificationId).HasColumnName("notification_id");
            entity.Property(e => e.CommentId).HasColumnName("comment_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.IsRead)
                .HasDefaultValueSql("false")
                .HasColumnName("is_read");
            entity.Property(e => e.Message).HasColumnName("message");
            entity.Property(e => e.PlantId).HasColumnName("plant_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Comment).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.CommentId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("notification_comment_id_fkey");

            entity.HasOne(d => d.Plant).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.PlantId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("notification_plant_id_fkey");
        });

        modelBuilder.Entity<Nutritionalvalue>(entity =>
        {
            entity.HasKey(e => e.NutritionId).HasName("nutritionalvalue_pkey");

            entity.ToTable("nutritionalvalue");

            entity.Property(e => e.NutritionId).HasColumnName("nutrition_id");
            entity.Property(e => e.Amount)
                .HasMaxLength(50)
                .HasColumnName("amount");
            entity.Property(e => e.Benefit).HasColumnName("benefit");
            entity.Property(e => e.NutrientName)
                .HasMaxLength(100)
                .HasColumnName("nutrient_name");
            entity.Property(e => e.PlantId).HasColumnName("plant_id");

            entity.HasOne(d => d.Plant).WithMany(p => p.Nutritionalvalues)
                .HasForeignKey(d => d.PlantId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("nutritionalvalue_plant_id_fkey");
        });

        modelBuilder.Entity<Passwordresetrequest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("passwordresetrequests_pkey");

            entity.ToTable("passwordresetrequests");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ExpiresAt).HasColumnName("expires_at");
            entity.Property(e => e.IsUsed)
                .HasDefaultValueSql("false")
                .HasColumnName("is_used");
            entity.Property(e => e.OtpCode)
                .HasMaxLength(10)
                .HasColumnName("otp_code");
            entity.Property(e => e.UserId).HasColumnName("user_id");
        });

        modelBuilder.Entity<Plant>(entity =>
        {
            entity.HasKey(e => e.PlantId).HasName("plant_pkey");

            entity.ToTable("plant");

            entity.Property(e => e.PlantId).HasColumnName("plant_id");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.ClimateId).HasColumnName("climate_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.GrowthCycle)
                .HasMaxLength(100)
                .HasColumnName("growth_cycle");
            entity.Property(e => e.Origin)
                .HasMaxLength(150)
                .HasColumnName("origin");
            entity.Property(e => e.OtherNames).HasColumnName("other_names");
            entity.Property(e => e.PlantName)
                .HasMaxLength(150)
                .HasColumnName("plant_name");
            entity.Property(e => e.ScientificName)
                .HasMaxLength(150)
                .HasColumnName("scientific_name");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValueSql("'Active'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Category).WithMany(p => p.Plants)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("plant_category_id_fkey");

            entity.HasOne(d => d.Climate).WithMany(p => p.Plants)
                .HasForeignKey(d => d.ClimateId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("plant_climate_id_fkey");

            entity.HasMany(d => d.Diseases).WithMany(p => p.Plants)
                .UsingEntity<Dictionary<string, object>>(
                    "Plantdiseaselink",
                    r => r.HasOne<Disease>().WithMany()
                        .HasForeignKey("DiseaseId")
                        .HasConstraintName("plantdiseaselink_disease_id_fkey"),
                    l => l.HasOne<Plant>().WithMany()
                        .HasForeignKey("PlantId")
                        .HasConstraintName("plantdiseaselink_plant_id_fkey"),
                    j =>
                    {
                        j.HasKey("PlantId", "DiseaseId").HasName("plantdiseaselink_pkey");
                        j.ToTable("plantdiseaselink");
                        j.IndexerProperty<int>("PlantId").HasColumnName("plant_id");
                        j.IndexerProperty<int>("DiseaseId").HasColumnName("disease_id");
                    });

            entity.HasMany(d => d.Regions).WithMany(p => p.Plants)
                .UsingEntity<Dictionary<string, object>>(
                    "Plantregion",
                    r => r.HasOne<Region>().WithMany()
                        .HasForeignKey("RegionId")
                        .HasConstraintName("plantregion_region_id_fkey"),
                    l => l.HasOne<Plant>().WithMany()
                        .HasForeignKey("PlantId")
                        .HasConstraintName("plantregion_plant_id_fkey"),
                    j =>
                    {
                        j.HasKey("PlantId", "RegionId").HasName("plantregion_pkey");
                        j.ToTable("plantregion");
                        j.IndexerProperty<int>("PlantId").HasColumnName("plant_id");
                        j.IndexerProperty<int>("RegionId").HasColumnName("region_id");
                    });

            entity.HasMany(d => d.Soils).WithMany(p => p.Plants)
                .UsingEntity<Dictionary<string, object>>(
                    "Plantsoil",
                    r => r.HasOne<Soiltype>().WithMany()
                        .HasForeignKey("SoilId")
                        .HasConstraintName("plantsoil_soil_id_fkey"),
                    l => l.HasOne<Plant>().WithMany()
                        .HasForeignKey("PlantId")
                        .HasConstraintName("plantsoil_plant_id_fkey"),
                    j =>
                    {
                        j.HasKey("PlantId", "SoilId").HasName("plantsoil_pkey");
                        j.ToTable("plantsoil");
                        j.IndexerProperty<int>("PlantId").HasColumnName("plant_id");
                        j.IndexerProperty<int>("SoilId").HasColumnName("soil_id");
                    });
        });

        modelBuilder.Entity<Plantcare>(entity =>
        {
            entity.HasKey(e => e.CareId).HasName("plantcare_pkey");

            entity.ToTable("plantcare");

            entity.Property(e => e.CareId).HasColumnName("care_id");
            entity.Property(e => e.Fertilizing).HasColumnName("fertilizing");
            entity.Property(e => e.Harvesting).HasColumnName("harvesting");
            entity.Property(e => e.OtherTips).HasColumnName("other_tips");
            entity.Property(e => e.PlantId).HasColumnName("plant_id");
            entity.Property(e => e.Season)
                .HasMaxLength(100)
                .HasColumnName("season");
            entity.Property(e => e.Watering).HasColumnName("watering");

            entity.HasOne(d => d.Plant).WithMany(p => p.Plantcares)
                .HasForeignKey(d => d.PlantId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("plantcare_plant_id_fkey");
        });

        modelBuilder.Entity<Plantcategory>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("plantcategory_pkey");

            entity.ToTable("plantcategory");

            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.CategoryName)
                .HasMaxLength(100)
                .HasColumnName("category_name");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Plantcomment>(entity =>
        {
            entity.HasKey(e => e.CommentId).HasName("plantcomment_pkey");

            entity.ToTable("plantcomment");

            entity.Property(e => e.CommentId).HasColumnName("comment_id");
            entity.Property(e => e.CommentText).HasColumnName("comment_text");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("false")
                .HasColumnName("is_deleted");
            entity.Property(e => e.ParentCommentId).HasColumnName("parent_comment_id");
            entity.Property(e => e.PlantId).HasColumnName("plant_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.ParentComment).WithMany(p => p.InverseParentComment)
                .HasForeignKey(d => d.ParentCommentId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("plantcomment_parent_comment_id_fkey");

            entity.HasOne(d => d.Plant).WithMany(p => p.Plantcomments)
                .HasForeignKey(d => d.PlantId)
                .HasConstraintName("plantcomment_plant_id_fkey");
        });

        modelBuilder.Entity<Plantimage>(entity =>
        {
            entity.HasKey(e => e.ImageId).HasName("plantimage_pkey");

            entity.ToTable("plantimage");

            entity.Property(e => e.ImageId).HasColumnName("image_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.ImageUrl).HasColumnName("image_url");
            entity.Property(e => e.IsPrimary)
                .HasDefaultValueSql("false")
                .HasColumnName("is_primary");
            entity.Property(e => e.PlantId).HasColumnName("plant_id");

            entity.HasOne(d => d.Plant).WithMany(p => p.Plantimages)
                .HasForeignKey(d => d.PlantId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("plantimage_plant_id_fkey");
        });

        modelBuilder.Entity<Plantsearchlog>(entity =>
        {
            entity.HasKey(e => e.SearchId).HasName("plantsearchlog_pkey");

            entity.ToTable("plantsearchlog");

            entity.Property(e => e.SearchId).HasColumnName("search_id");
            entity.Property(e => e.FilterJson).HasColumnName("filter_json");
            entity.Property(e => e.Keyword)
                .HasMaxLength(150)
                .HasColumnName("keyword");
            entity.Property(e => e.SearchedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("searched_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");
        });

        modelBuilder.Entity<Plantsearchresultlog>(entity =>
        {
            entity.HasKey(e => e.ResultId).HasName("plantsearchresultlog_pkey");

            entity.ToTable("plantsearchresultlog");

            entity.Property(e => e.ResultId).HasColumnName("result_id");
            entity.Property(e => e.PlantId).HasColumnName("plant_id");
            entity.Property(e => e.SearchId).HasColumnName("search_id");

            entity.HasOne(d => d.Plant).WithMany(p => p.Plantsearchresultlogs)
                .HasForeignKey(d => d.PlantId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("plantsearchresultlog_plant_id_fkey");

            entity.HasOne(d => d.Search).WithMany(p => p.Plantsearchresultlogs)
                .HasForeignKey(d => d.SearchId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("plantsearchresultlog_search_id_fkey");
        });

        modelBuilder.Entity<Plantseason>(entity =>
        {
            entity.HasKey(e => e.SeasonId).HasName("plantseason_pkey");

            entity.ToTable("plantseason");

            entity.Property(e => e.SeasonId).HasColumnName("season_id");
            entity.Property(e => e.HarvestingSeason)
                .HasMaxLength(100)
                .HasColumnName("harvesting_season");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.PlantId).HasColumnName("plant_id");
            entity.Property(e => e.PlantingSeason)
                .HasMaxLength(100)
                .HasColumnName("planting_season");

            entity.HasOne(d => d.Plant).WithMany(p => p.Plantseasons)
                .HasForeignKey(d => d.PlantId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("plantseason_plant_id_fkey");
        });

        modelBuilder.Entity<Plantviewlog>(entity =>
        {
            entity.HasKey(e => e.ViewId).HasName("plantviewlog_pkey");

            entity.ToTable("plantviewlog");

            entity.Property(e => e.ViewId).HasColumnName("view_id");
            entity.Property(e => e.PlantId).HasColumnName("plant_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.ViewedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("viewed_at");

            entity.HasOne(d => d.Plant).WithMany(p => p.Plantviewlogs)
                .HasForeignKey(d => d.PlantId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("plantviewlog_plant_id_fkey");
        });

        modelBuilder.Entity<Reference>(entity =>
        {
            entity.HasKey(e => e.ReferenceId).HasName("reference_pkey");

            entity.ToTable("reference");

            entity.Property(e => e.ReferenceId).HasColumnName("reference_id");
            entity.Property(e => e.PlantId).HasColumnName("plant_id");
            entity.Property(e => e.SourceName)
                .HasMaxLength(200)
                .HasColumnName("source_name");
            entity.Property(e => e.Url).HasColumnName("url");

            entity.HasOne(d => d.Plant).WithMany(p => p.References)
                .HasForeignKey(d => d.PlantId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("reference_plant_id_fkey");
        });

        modelBuilder.Entity<Region>(entity =>
        {
            entity.HasKey(e => e.RegionId).HasName("region_pkey");

            entity.ToTable("region");

            entity.Property(e => e.RegionId).HasColumnName("region_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.RegionName)
                .HasMaxLength(100)
                .HasColumnName("region_name");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Soiltype>(entity =>
        {
            entity.HasKey(e => e.SoilId).HasName("soiltype_pkey");

            entity.ToTable("soiltype");

            entity.Property(e => e.SoilId).HasColumnName("soil_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.FertilityLevel)
                .HasMaxLength(50)
                .HasColumnName("fertility_level");
            entity.Property(e => e.PhRange)
                .HasMaxLength(50)
                .HasColumnName("ph_range");
            entity.Property(e => e.SoilName)
                .HasMaxLength(100)
                .HasColumnName("soil_name");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Unrecognizedplant>(entity =>
        {
            entity.HasKey(e => e.UnrecognizedId).HasName("unrecognizedplants_pkey");

            entity.ToTable("unrecognizedplants");

            entity.Property(e => e.UnrecognizedId).HasColumnName("unrecognized_id");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Plantname)
                .HasMaxLength(200)
                .HasColumnName("plantname");
            entity.Property(e => e.Usermessage).HasColumnName("usermessage");
        });

        modelBuilder.Entity<Usage>(entity =>
        {
            entity.HasKey(e => e.UsageId).HasName("usage_pkey");

            entity.ToTable("usage");

            entity.Property(e => e.UsageId).HasColumnName("usage_id");
            entity.Property(e => e.Details).HasColumnName("details");
            entity.Property(e => e.PlantId).HasColumnName("plant_id");
            entity.Property(e => e.UsageType)
                .HasMaxLength(100)
                .HasColumnName("usage_type");

            entity.HasOne(d => d.Plant).WithMany(p => p.Usages)
                .HasForeignKey(d => d.PlantId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("usage_plant_id_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("users_pkey");

            entity.ToTable("users");

            entity.HasIndex(e => e.Username, "users_username_key").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Isactive)
                .HasDefaultValueSql("true")
                .HasColumnName("isactive");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .HasColumnName("password_hash");
            entity.Property(e => e.Role)
                .HasMaxLength(50)
                .HasDefaultValueSql("'user'::character varying")
                .HasColumnName("role");
            entity.Property(e => e.Username)
                .HasMaxLength(100)
                .HasColumnName("username");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
