using Navigator.Data.Entities;

namespace Navigator.Data.RisIds;

public interface IRisIdReactivationHoldService
{
    Task MarkReactivatedAsync(
        IReadOnlyCollection<RisId> reactivatedRisIds,
        CancellationToken cancellationToken);

    Task<IReadOnlySet<string>> GetProtectedRisIdsAsync(
        IReadOnlyCollection<string> risIds,
        DateTime nowUtc,
        CancellationToken cancellationToken);
}
