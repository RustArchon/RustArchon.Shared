// Copyright ©2026 Scott Blomfield

using System;

namespace RustArchon.Shared.DTOs;

/// <summary>
/// One Organization whose billing period ends inside the requested window - what they are on now, what
/// renewal will charge them, and whether a change they already accepted lands at the same moment.
/// </summary>
/// <remarks>
/// <para>
/// Two amounts, because they are genuinely different questions.
/// <see cref="CurrentAmount"/> is what this period was billed - a matter of record, and possibly a
/// prorated slice rather than a full period. <see cref="RenewalAmount"/> is what the <em>next</em>
/// period will cost, priced from today's catalog against today's server count, and against the
/// <em>scheduled</em> plan where one is due to land at renewal. Reporting only the first would
/// misstate the forecast for every organization with a pending downgrade, which is exactly the set of
/// organizations a renewals report exists to draw attention to.
/// </para>
/// <para>
/// <see cref="RenewalAmountUnknown"/> is not a rounding concern - it means the plan carries no price
/// row for the term this organization is billed on, so renewal has nothing to charge. That is a
/// catalog defect rather than a zero, and the report surfaces it rather than quietly contributing $0
/// to the forecast.
/// </para>
/// </remarks>
public class UpcomingRenewalRowDto
{
    public Guid TenantId { get; set; }

    /// <summary>The Organization's name, as it appears everywhere else in the Panel.</summary>
    public string OrganizationName { get; set; } = string.Empty;

    public string? ContactEmail { get; set; }

    public string PlanName { get; set; } = string.Empty;
    public string PlanColorCode { get; set; } = string.Empty;

    public int TermMonths { get; set; }

    /// <summary>End of the current billing period - the renewal date.</summary>
    public DateTimeOffset RenewsOn { get; set; }

    /// <summary>Whole days from now until <see cref="RenewsOn"/>. Negative for a period that has ended
    /// but not yet been rolled over by the scheduler, which is worth seeing rather than hiding.</summary>
    public int DaysUntilRenewal { get; set; }

    /// <summary>Servers this Organization currently has.</summary>
    public int ServerCount { get; set; }

    /// <summary>
    /// Server slots bought - what the renewal is actually priced against. Equal to
    /// <see cref="ServerCount"/> on a flat tier, but not on a per-unit plan where capacity is held
    /// whether or not it's used.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>What the current period was billed. See this class's remarks.</summary>
    public decimal CurrentAmount { get; set; }

    /// <summary>What the next period is expected to cost. See this class's remarks.</summary>
    public decimal RenewalAmount { get; set; }

    /// <summary>True when the plan has no price for this term, so <see cref="RenewalAmount"/> is not a
    /// real figure. See this class's remarks.</summary>
    public bool RenewalAmountUnknown { get; set; }

    /// <summary>True when a plan or term change is due to take effect at this renewal.</summary>
    public bool HasScheduledChange { get; set; }

    public string? ScheduledPlanName { get; set; }
    public int? ScheduledTermMonths { get; set; }
}
