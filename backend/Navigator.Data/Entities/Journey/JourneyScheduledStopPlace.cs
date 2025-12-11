using Microsoft.EntityFrameworkCore;
using Navigator.Data.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Navigator.Data.Entities.Journey;

[Table("journey_scheduled_stop_places", Schema = "core")]
[Index(nameof(Date))]
[Index(nameof(StationEvaNumber))]
[Index(nameof(StationEvaNumber), nameof(JourneyId))]
[Index(nameof(StationEvaNumber), nameof(Date))]
public class JourneyScheduledStopPlace
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [Column("journey_id")]
    public required Guid JourneyId { get; set; }

    [ForeignKey(nameof(JourneyId))]
    public virtual Journey? Journey { get; set; }

    [Column("date")]
    public required DateOnly Date { get; set; }

    [Column("schedule_type")]
    public required ScheduleType ScheduleType { get; set; }

    [MaxLength(1024)]
    [Column("station_name")]
    public required string StationName { get; set; }

    [Column("station_eva_number")]
    public required int StationEvaNumber { get; set; }

    [Column("cancelled")]
    public required bool Cancelled { get; set; }

    [Column("additional")]
    public required bool Additional { get; set; }

    [Column("demand")]
    public required bool Demand { get; set; }

    [Column("no_passenger_change")]
    public required bool NoPassengerChange { get; set; }

    [Column("planned_time")]
    public required DateTimeOffset PlannedTime { get; set; }

    [Column("actual_time")]
    public required DateTimeOffset ActualTime { get; set; }

    [Column("delay")]
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public int Delay { get; private init; }

    [MaxLength(32)]
    [Column("planned_platform")]
    public string? PlannedPlatform { get; set; }

    [MaxLength(32)]
    [Column("actual_platform")]
    public string? ActualPlatform { get; set; }

    public virtual ICollection<JourneyStopPlaceInformation> Informations { get; set; } = [];
}
