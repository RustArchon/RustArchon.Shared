// Copyright ©2026 Scott Blomfield

using System;
using System.Collections.Generic;

namespace RustArchon.Shared.DTOs;

/// <summary>A plan somebody could start a new Organization on.</summary>
/// <remarks>
/// A fourth audience for a Plan, and it needs one field none of the others carry:
/// <see cref="AlreadyUsed"/> is about the <em>viewer</em>, not the plan, so it cannot come from
/// <c>PlanOptionDto</c> (which answers "what may this Organization move to?") or
/// <c>PublicPlanDto</c> (which has no viewer at all).
/// </remarks>
public class NewOrganizationPlanDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ColorCode { get; set; } = "#888888";

    public List<PlanPriceDto> Prices { get; set; } = [];

    /// <summary>Null when the plan has no ceiling - capacity is bought per unit.</summary>
    public int? MaximumServers { get; set; }

    public int MaximumUsers { get; set; }
    public bool HasRoles { get; set; }

    /// <summary>Whether a person may only have one Organization of their own on this plan.</summary>
    public bool OnePerOwner { get; set; }

    /// <summary>
    /// True when <see cref="OnePerOwner"/> applies and this caller has already used theirs.
    /// </summary>
    /// <remarks>
    /// Sent rather than filtered out so the screen can grey the plan out and say why. Somebody
    /// looking for the free tier and not finding it assumes the page is broken; somebody seeing it
    /// disabled with a reason learns the rule.
    /// </remarks>
    public bool AlreadyUsed { get; set; }
}

/// <summary>A request to create an Organization for the caller.</summary>
public class CreateOrganizationRequestDto
{
    public string Name { get; set; } = string.Empty;

    /// <summary>The plan to start on, or <c>null</c> for the platform default.</summary>
    public Guid? PlanId { get; set; }
}

/// <summary>The Organization that was just created.</summary>
public class CreatedOrganizationDto
{
    public Guid TenantId { get; set; }
    public string Name { get; set; } = string.Empty;
}

/// <summary>An Organization the caller founded.</summary>
public class FoundedOrganizationDto
{
    public Guid TenantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTimeOffset CreatedOn { get; set; }

    /// <summary>The plan it is on now, or <c>null</c> if it somehow has none.</summary>
    public string? PlanName { get; set; }
}
