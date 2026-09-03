// Copyright ©2026 Scott Blomfield

using System.ComponentModel.DataAnnotations;
using JumpStart.Api.DTOs;

namespace RustArchon.Shared.DTOs;

/// <summary>
/// DTO for reading a <c>Plan</c>, used by the platform-admin management page. Includes every
/// historical row (active or not) - see <c>PublicPlanDto</c> for the trimmed, active-only shape the
/// marketing site consumes.
/// </summary>
public class PlanDto : AuditableEntityDto
{
    /// <summary>
    /// Admin-chosen display name (e.g. "Wood", "Starter", "Pro") - see <c>Plan.Name</c>'s remarks for
    /// why this replaced a fixed enum: this catalog isn't part of the AGPL-licensed product, so
    /// self-hosters aren't restricted to RustArchon's own marketing naming.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Hex color (e.g. <c>#b08553</c>) used for this plan's swatch/accent on the marketing site.</summary>
    public string ColorCode { get; set; } = string.Empty;

    public decimal MonthlyPrice { get; set; }
    public decimal QuarterlyPrice { get; set; }
    public decimal AnnualPrice { get; set; }

    /// <summary>How many days of console/chat/player history this plan retains.</summary>
    public int RetentionHistory { get; set; }

    public bool HasRoles { get; set; }
    public int MaximumServers { get; set; }
    public int MaximumUsers { get; set; }
    public bool Active { get; set; }

    /// <summary>
    /// How many Organizations are currently assigned to this specific Plan row - not the Name, this
    /// exact historical record. Drives the admin page's "editing this will create a new Plan instead"
    /// warning: zero means a plain in-place edit is safe, one or more means editing should go through
    /// the supersede flow instead. Computed server-side (a live count against TenantPlan), not a
    /// stored column.
    /// </summary>
    public int SubscriberCount { get; set; }
}

/// <summary>
/// DTO for creating a new <c>Plan</c> row - either a brand-new Name, or a fresh draft for a Name
/// whose current Plan has no subscribers yet (see <see cref="PlanDto.SubscriberCount"/>). If
/// <see cref="Active"/> is <c>true</c>, any other currently-active Plan with the same
/// <see cref="Name"/> is deactivated automatically - see <c>Plan.Active</c>'s remarks.
/// </summary>
public class CreatePlanDto : ICreateDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [RegularExpression("^#[0-9A-Fa-f]{6}$", ErrorMessage = "Must be a 6-digit hex color, e.g. #b08553.")]
    public string ColorCode { get; set; } = "#888888";

    [Range(0, double.MaxValue)]
    public decimal MonthlyPrice { get; set; }

    [Range(0, double.MaxValue)]
    public decimal QuarterlyPrice { get; set; }

    [Range(0, double.MaxValue)]
    public decimal AnnualPrice { get; set; }

    [Range(0, int.MaxValue)]
    public int RetentionHistory { get; set; }

    public bool HasRoles { get; set; }

    [Range(0, int.MaxValue)]
    public int MaximumServers { get; set; }

    [Range(0, int.MaxValue)]
    public int MaximumUsers { get; set; }

    public bool Active { get; set; }
}

/// <summary>
/// DTO for editing an existing <c>Plan</c> in place. <see cref="PlanDto.Name"/> is deliberately not
/// editable here - changing it would really mean creating a different plan, not editing this one (same
/// reasoning <c>Type</c> used to carry before it was replaced). <see cref="ColorCode"/> IS editable -
/// purely cosmetic branding, not a term any subscriber is relying on, so there's no reason to withhold
/// it the way price/limits are. Only meant to be used when <see cref="PlanDto.SubscriberCount"/> is
/// zero; the admin page routes to the supersede flow instead once any Organization is assigned. If
/// <see cref="Active"/> is set to <c>true</c>, any other currently-active Plan with the same Name is
/// deactivated automatically.
/// </summary>
public class UpdatePlanDto : IUpdateDto
{
    public Guid Id { get; set; }

    [Required]
    [RegularExpression("^#[0-9A-Fa-f]{6}$", ErrorMessage = "Must be a 6-digit hex color, e.g. #b08553.")]
    public string ColorCode { get; set; } = "#888888";

    [Range(0, double.MaxValue)]
    public decimal MonthlyPrice { get; set; }

    [Range(0, double.MaxValue)]
    public decimal QuarterlyPrice { get; set; }

    [Range(0, double.MaxValue)]
    public decimal AnnualPrice { get; set; }

    [Range(0, int.MaxValue)]
    public int RetentionHistory { get; set; }

    public bool HasRoles { get; set; }

    [Range(0, int.MaxValue)]
    public int MaximumServers { get; set; }

    [Range(0, int.MaxValue)]
    public int MaximumUsers { get; set; }

    public bool Active { get; set; }
}

/// <summary>
/// Request body for superseding a Plan that already has subscribers: creates a new Plan row (same
/// Name as the one being superseded, these field values, <c>Active: true</c>), then deactivates the
/// old one along with any other currently-active Plan with that Name. Existing Organizations stay
/// pointed at the old (now inactive) row - see <c>TenantPlan</c>'s remarks - so this never changes
/// what current subscribers are paying/entitled to, only what new sign-ups get.
/// </summary>
public class SupersedePlanDto
{
    [Required]
    [RegularExpression("^#[0-9A-Fa-f]{6}$", ErrorMessage = "Must be a 6-digit hex color, e.g. #b08553.")]
    public string ColorCode { get; set; } = "#888888";

    [Range(0, double.MaxValue)]
    public decimal MonthlyPrice { get; set; }

    [Range(0, double.MaxValue)]
    public decimal QuarterlyPrice { get; set; }

    [Range(0, double.MaxValue)]
    public decimal AnnualPrice { get; set; }

    [Range(0, int.MaxValue)]
    public int RetentionHistory { get; set; }

    public bool HasRoles { get; set; }

    [Range(0, int.MaxValue)]
    public int MaximumServers { get; set; }

    [Range(0, int.MaxValue)]
    public int MaximumUsers { get; set; }
}

/// <summary>
/// The trimmed, public shape of a Plan - only ever the currently-active one per Name, for
/// RustArchon.Web's pricing page. No <c>Id</c>/audit fields/<see cref="PlanDto.SubscriberCount"/> -
/// none of that is anyone else's business.
/// </summary>
public class PublicPlanDto
{
    public string Name { get; set; } = string.Empty;
    public string ColorCode { get; set; } = string.Empty;
    public decimal MonthlyPrice { get; set; }
    public decimal QuarterlyPrice { get; set; }
    public decimal AnnualPrice { get; set; }
    public int RetentionHistory { get; set; }
    public bool HasRoles { get; set; }
    public int MaximumServers { get; set; }
    public int MaximumUsers { get; set; }
}
