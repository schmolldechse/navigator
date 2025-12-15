using System.ComponentModel.DataAnnotations;

namespace Navigator.Data.Models.Timetable;

public class RisBoardRequest
{
    public required int EvaNumber { get; set; }
    public required DateTime TimeStart { get; set; } = DateTime.Now;

    [Range(0, 720)]
    public int Duration { get; set; } = 60;
}
