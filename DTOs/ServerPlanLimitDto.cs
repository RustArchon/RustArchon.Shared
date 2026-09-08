// Copyright ©2026 Scott Blomfield

namespace RustArchon.Shared.DTOs;

/// <summary>
/// The calling tenant's current Plan limits and how many servers they currently have - lets the
/// Panel warn "you're at your limit" the moment a user clicks Add Server, before they've filled out
/// the form, not just after they submit it and it's rejected. Purely informational: the authoritative
/// enforcement still happens server-side on <c>RustServersController.Create</c>, which this DTO's
/// values don't replace or bypass - a stale read here (another tab adding a server in the meantime,
/// say) just means the Create call is the one that actually catches it.
/// </summary>
/// <remarks>
/// <see cref="PlanName"/>/<see cref="MaximumServers"/> are <c>null</c> when the tenant has no Plan
/// assigned at all (shouldn't happen - see <c>SubscriptionBackfiller</c> - but <c>Create</c> fails open
/// in that case rather than blocking a legitimate request, and this mirrors that: no known limit
/// means nothing to warn about).
/// </remarks>
public class ServerPlanLimitDto
{
    public string? PlanName { get; set; }

    /// <summary>
    /// The plan's ceiling, or <c>null</c> when it has none.
    /// </summary>
    /// <remarks>
    /// No longer what gates adding a server - <see cref="Quantity"/> is. This is now the cap on how much
    /// capacity may be <em>bought</em>, and is here for wording ("upgrade to go past 5") rather than for
    /// enforcement.
    /// </remarks>
    public int? MaximumServers { get; set; }

    public int CurrentServerCount { get; set; }

    /// <summary>
    /// Server slots currently paid for - the actual limit. <c>null</c> when the tenant has no billing
    /// period at all, which the API treats as "no known limit" and lets through.
    /// </summary>
    public int? Quantity { get; set; }

    /// <summary>
    /// Whether more slots can be bought, or whether the plan itself has to change. False on a flat tier,
    /// where capacity comes with the plan and isn't sold separately.
    /// </summary>
    public bool CanBuyCapacity { get; set; }
}
