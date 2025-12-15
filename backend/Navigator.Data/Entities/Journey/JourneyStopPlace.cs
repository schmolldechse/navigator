using Microsoft.EntityFrameworkCore;
using Navigator.Data.Entities.Journey.Message;
using Navigator.Data.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Navigator.Data.Entities.Journey;

[Table("journey_stop_places", Schema = "core")]
[Index(nameof(JourneyId))]
[Index(nameof(Date))]
[Index(nameof(StationEvaNumber))]
[Index(nameof(StationEvaNumber), nameof(JourneyId))]
[Index(nameof(StationEvaNumber), nameof(Date))]
[Index(nameof(PlannedTimeUtc))]
[Index(nameof(ActualTimeUtc))]
public class JourneyStopPlace
{
    private DateTimeOffset _plannedTime, _actualTime;

    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [Column("journey_id")]
    [MaxLength(82)]
    public required string JourneyId { get; set; }

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

    [NotMapped]
    public required DateTimeOffset PlannedTime
    {
        get => _plannedTime;
        set => _plannedTime = value;
    }

    [NotMapped]
    public required DateTimeOffset ActualTime
    {
        get => _actualTime;
        set => _actualTime = value;
    }

    [Column("time_type")]
    public required TimeType TimeType { get; set; }

    [Column("delay")]
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public int Delay { get; private init; }

    [MaxLength(32)]
    [Column("planned_platform")]
    public string? PlannedPlatform { get; set; }

    [MaxLength(32)]
    [Column("actual_platform")]
    public string? ActualPlatform { get; set; }

    public required virtual ICollection<JourneyStopPlaceMessage> Messages { get; set; } = [];

    /// INTERNAL DATABASE USE
    [Column("planned_time_utc")]
    public DateTime PlannedTimeUtc
    {
        get => _plannedTime.UtcDateTime;
        private set
        {
            var currentOffset = _plannedTime.Offset;
            _plannedTime = new DateTimeOffset(value, TimeSpan.Zero).ToOffset(currentOffset);
        }
    }

    [Column("planned_time_offset_minutes")]
    public short PlannedTimeOffsetMinutes
    {
        get => (short)_plannedTime.Offset.TotalMinutes;
        private set
        {
            var currentUtc = _plannedTime.UtcDateTime;
            _plannedTime = new DateTimeOffset(currentUtc, TimeSpan.Zero).ToOffset(TimeSpan.FromMinutes(value));
        }
    }

    [Column("actual_time_utc")]
    public DateTime ActualTimeUtc
    {
        get => _actualTime.UtcDateTime;
        private set
        {
            var currentOffset = _actualTime.Offset;
            _actualTime = new DateTimeOffset(value, TimeSpan.Zero).ToOffset(currentOffset);
        }
    }

    [Column("actual_time_offset_minutes")]
    public short ActualTimeOffsetMinutes
    {
        get => (short)_actualTime.Offset.TotalMinutes;
        private set
        {
            var currentUtc = _actualTime.UtcDateTime;
            _actualTime = new DateTimeOffset(currentUtc, TimeSpan.Zero).ToOffset(TimeSpan.FromMinutes(value));
        }
    }
}
