namespace Navigator.Data.RisIds;

public sealed class RisIdLifecycleOptions
{
    public const string SectionName = "RisIdLifecycle";

    public int ReactivationProtectionDays { get; init; } = 14;
}
