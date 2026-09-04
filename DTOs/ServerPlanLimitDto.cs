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
/// assigned at all (shouldn't happen - see <c>TenantPlanBackfiller</c> - but <c>Create</c> fails open
/// in that case rather than blocking a legitimate request, and this mirrors that: no known limit
/// means nothing to warn about).
/// </remarks>
public class ServerPlanLimitDto
{
    public string? PlanName { get; set; }
    public int? MaximumServers { get; set; }
    public int CurrentServerCount { get; set; }
}
