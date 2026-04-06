using Navigator.Data.Entities.Journey.Message;
using Navigator.Data.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Navigator.Data.Entities.Journey;

public class JourneyStopPlace
{
    [Column("id")]
    public virtual Guid Id { get; set; }

    [Column("journey_id")]
    public required string JourneyId { get; set; }

    public virtual Journey? Journey { get; set; }

    [Column("date")]
    public required DateOnly Date { get; set; }

    [Column("schedule_type")]
    public required ScheduleType ScheduleType { get; set; }

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
    public required DateTime PlannedTime { get; set; }

    [Column("actual_time")]
    public required DateTime ActualTime { get; set; }

    [Column("time_type")]
    public required TimeType TimeType { get; set; }

    [Column("delay")]
    public int Delay { get; private init; }

    [Column("planned_platform")]
    public string? PlannedPlatform { get; set; }

    [Column("actual_platform")]
    public string? ActualPlatform { get; set; }

    public required virtual ICollection<JourneyStopPlaceMessage> Messages { get; set; } = [];
}
