using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Navigator.Data.Entities;

namespace Navigator.Data.RisIds;

public sealed class RisIdReactivationHoldService(
    DataContext dataContext,
    IOptions<RisIdLifecycleOptions> options,
    ILogger<RisIdReactivationHoldService> logger) : IRisIdReactivationHoldService
{
    public async Task MarkReactivatedAsync(
        IReadOnlyCollection<RisId> reactivatedRisIds,
        CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var protectUntil = now.AddDays(options.Value.ReactivationProtectionDays);

        foreach (var risId in reactivatedRisIds)
        {
            await dataContext.Database.ExecuteSqlInterpolatedAsync($"""
                INSERT INTO statistics.ris_id_reactivation_holds (
                    ris_id,
                    reactivated_at,
                    protect_until,
                    activation_count,
                    last_seen_at_reactivation,
                    last_inserted_at_reactivation
                )
                VALUES ({risId.Id}, {now}, {protectUntil}, 1, {risId.LastSeen}, {risId.LastInserted})
                ON CONFLICT (ris_id)
                DO UPDATE SET reactivated_at = EXCLUDED.reactivated_at,
                              protect_until = EXCLUDED.protect_until,
                              activation_count = statistics.ris_id_reactivation_holds.activation_count + 1,
                              last_seen_at_reactivation = EXCLUDED.last_seen_at_reactivation,
                              last_inserted_at_reactivation = EXCLUDED.last_inserted_at_reactivation;
                """, cancellationToken);
        }

        if (reactivatedRisIds.Count > 0)
        {
            logger.LogInformation(
                "Marked reactivated RIS IDs. Count={RisIdCount} ProtectUntil={ProtectUntil}",
                reactivatedRisIds.Count,
                protectUntil);
        }
    }

    public async Task<IReadOnlySet<string>> GetProtectedRisIdsAsync(
        IReadOnlyCollection<string> risIds,
        DateTime nowUtc,
        CancellationToken cancellationToken)
    {
        var protectedIds = await dataContext.RisIdReactivationHolds
            .Where(row => risIds.Contains(row.RisId))
            .Where(row => row.ProtectUntil > nowUtc)
            .Select(row => row.RisId)
            .ToListAsync(cancellationToken);

        return protectedIds.ToHashSet();
    }
}
