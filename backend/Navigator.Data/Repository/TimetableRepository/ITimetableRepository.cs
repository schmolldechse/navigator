using Navigator.Data.Models.Ris;
using Navigator.Data.Models.Timetable;

namespace Navigator.Data.Repository.TimetableRepository;

public interface ITimetableRepository
{
    Task<RisBoards.BoardPublicArrival> GetArrivalsAsync(RisBoardRequest request);
    Task<RisBoards.BoardPublicDeparture> GetDeparturesAsync(RisBoardRequest request);
}
