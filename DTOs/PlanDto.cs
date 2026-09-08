// Copyright ©2026 Scott Blomfield

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using JumpStart.Api.DTOs;

namespace RustArchon.Shared.DTOs;

/// <summary>
/// One term a plan is offered on, and what it costs. A plan is sold on exactly the terms it has these
/// for &mdash; the absence of a row is what "not available on that term" looks like, since a price of
/// zero already means free.
/// </summary>
/// <remarks>
/// <see cref="BaseAmount"/> alone is the whole price for a flat tier. For a per-unit plan,
/// <see cref="BaseAmount"/> covers <see cref="IncludedUnits"/> and each unit beyond that costs
/// <see cref="UnitAmount"/> &mdash; so "$2 for the first server, $2 for each after" is base 2.00,
/// included 1, unit 2.00.
/// </remarks>
public class PlanPriceDto
{
    /// <summary>How many months one billing period lasts. Any positive count - see <see cref="BillingTerms"/>.</summary>
    [Range(1, 120)]
    public int TermMonths { get; set; }

    [Range(0, double.MaxValue)]
    public decimal BaseAmount { get; set; }

    [Range(0, int.MaxValue)]
    public int IncludedUnits { get; set; }

    [Range(0, double.MaxValue)]
    public decimal UnitAmount { get; set; }

    /// <summary>ISO 4217 code for the amounts on this row.</summary>
    [MaxLength(3)]
    public string Currency { get; set; } = "USD";

    /// <summary>What this term costs for <paramref name="quantity"/> units of capacity.</summary>
    public decimal AmountFor(int quantity) =>
        BaseAmount + (Math.Max(0, quantity - IncludedUnits) * UnitAmount);
}

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

    /// <summary>What this plan costs, one entry per term it's offered on.</summary>
    public List<PlanPriceDto> Prices { get; set; } = [];

    /// <summary>Whether capacity is capped at a ceiling or bought by the unit.</summary>
    public PricingModel PricingModel { get; set; }

    /// <summary>How many days of console/chat/player history this plan retains.</summary>
    public int RetentionHistory { get; set; }

    public bool HasRoles { get; set; }

    /// <summary>
    /// Whether one person may only have a single Organization of their own on this plan. Set on the
    /// free tier so an unlimited supply of Organizations isn't an unlimited supply of free servers.
    /// </summary>
    public bool OnePerOwner { get; set; }

    /// <summary>Ceiling on servers, or <c>null</c> for none - a per-unit plan charges rather than caps.</summary>
    public int? MaximumServers { get; set; }

    public int MaximumUsers { get; set; }
    public bool Active { get; set; }

    /// <summary>
    /// How many Organizations have <em>ever</em> been on this specific Plan row - not the Name, this
    /// exact historical record. Drives the admin page's "editing this will create a new Plan instead"
    /// warning: zero means a plain in-place edit is safe, one or more means editing should go through
    /// the supersede flow instead. Computed server-side (a live count against Subscription), not a
    /// stored column.
    /// </summary>
    /// <remarks>
    /// "Ever", not "currently": Subscription is subscription history, so an Organization that moved off
    /// this Plan still has a closed interval pointing at it, and this counts that. A Plan's terms are
    /// the record of what somebody was actually billed, so a Plan becomes uneditable the moment it is
    /// first used and stays that way - a count that dropped back to zero when the last subscriber left
    /// would quietly re-enable editing the prices a past subscriber was charged under.
    /// </remarks>
    public int SubscriberCount { get; set; }
}

/// <summary>
/// DTO for creating a new <c>Plan</c> row - either a brand-new Name, or a fresh draft for a Name
/// whose current Plan has never been used (see <see cref="PlanDto.SubscriberCount"/>). If
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

    /// <summary>The terms this plan is sold on. At least one is required - a plan nobody can buy is not a plan.</summary>
    [MinLength(1, ErrorMessage = "A plan must be offered on at least one billing term.")]
    public List<PlanPriceDto> Prices { get; set; } = [];

    public PricingModel PricingModel { get; set; }

    [Range(0, int.MaxValue)]
    public int RetentionHistory { get; set; }

    public bool HasRoles { get; set; }

    /// <summary>
    /// Whether one person may only have a single Organization of their own on this plan. Set on the
    /// free tier so an unlimited supply of Organizations isn't an unlimited supply of free servers.
    /// </summary>
    public bool OnePerOwner { get; set; }

    /// <summary>Null for a per-unit plan, which has no ceiling.</summary>
    [Range(0, int.MaxValue)]
    public int? MaximumServers { get; set; }

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
/// zero; the admin page routes to the supersede flow instead once any Organization has ever been on
/// this Plan - including one that has since moved off it. If
/// <see cref="Active"/> is set to <c>true</c>, any other currently-active Plan with the same Name is
/// deactivated automatically.
/// </summary>
public class UpdatePlanDto : IUpdateDto
{
    public Guid Id { get; set; }

    [Required]
    [RegularExpression("^#[0-9A-Fa-f]{6}$", ErrorMessage = "Must be a 6-digit hex color, e.g. #b08553.")]
    public string ColorCode { get; set; } = "#888888";

    [MinLength(1, ErrorMessage = "A plan must be offered on at least one billing term.")]
    public List<PlanPriceDto> Prices { get; set; } = [];

    public PricingModel PricingModel { get; set; }

    [Range(0, int.MaxValue)]
    public int RetentionHistory { get; set; }

    public bool HasRoles { get; set; }

    /// <summary>
    /// Whether one person may only have a single Organization of their own on this plan. Set on the
    /// free tier so an unlimited supply of Organizations isn't an unlimited supply of free servers.
    /// </summary>
    public bool OnePerOwner { get; set; }

    [Range(0, int.MaxValue)]
    public int? MaximumServers { get; set; }

    [Range(0, int.MaxValue)]
    public int MaximumUsers { get; set; }

    public bool Active { get; set; }
}

/// <summary>
/// Request body for superseding a Plan that already has subscribers: creates a new Plan row (same
/// Name as the one being superseded, these field values, <c>Active: true</c>), then deactivates the
/// old one along with any other currently-active Plan with that Name. Existing Organizations stay
/// pointed at the old (now inactive) row - see <c>Subscription</c>'s remarks - so this never changes
/// what current subscribers are paying/entitled to, only what new sign-ups get.
/// </summary>
public class SupersedePlanDto
{
    [Required]
    [RegularExpression("^#[0-9A-Fa-f]{6}$", ErrorMessage = "Must be a 6-digit hex color, e.g. #b08553.")]
    public string ColorCode { get; set; } = "#888888";

    [MinLength(1, ErrorMessage = "A plan must be offered on at least one billing term.")]
    public List<PlanPriceDto> Prices { get; set; } = [];

    public PricingModel PricingModel { get; set; }

    [Range(0, int.MaxValue)]
    public int RetentionHistory { get; set; }

    public bool HasRoles { get; set; }

    /// <summary>
    /// Whether one person may only have a single Organization of their own on this plan. Set on the
    /// free tier so an unlimited supply of Organizations isn't an unlimited supply of free servers.
    /// </summary>
    public bool OnePerOwner { get; set; }

    [Range(0, int.MaxValue)]
    public int? MaximumServers { get; set; }

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

    /// <summary>The terms this plan is actually sold on - the pricing page renders a toggle from these
    /// rather than assuming all three exist.</summary>
    public List<PlanPriceDto> Prices { get; set; } = [];

    public PricingModel PricingModel { get; set; }
    public int RetentionHistory { get; set; }
    public bool HasRoles { get; set; }

    /// <summary>
    /// Whether one person may only have a single Organization of their own on this plan. Set on the
    /// free tier so an unlimited supply of Organizations isn't an unlimited supply of free servers.
    /// </summary>
    public bool OnePerOwner { get; set; }

    /// <summary>Null means no ceiling - capacity is bought per unit rather than capped.</summary>
    public int? MaximumServers { get; set; }

    public int MaximumUsers { get; set; }
}
