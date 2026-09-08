// Copyright ©2026 Scott Blomfield

using System;
using System.Collections.Generic;

namespace RustArchon.Shared.DTOs;

/// <summary>
/// Everything about a report result except its rows - the figures that summarise it, when it ran, and
/// whether it is complete.
/// </summary>
/// <remarks>
/// Split out from <see cref="ReportResult{TRow}"/> so the parts of the UI that frame a report - the
/// summary strip, the truncation warning, the generated-at line - can be written once against this,
/// without a type parameter for rows they never touch.
/// </remarks>
public abstract class ReportResultBase
{
    /// <summary>
    /// Pre-formatted figures for the strip above the results - "12 organizations", "$14,203.00". Strings
    /// rather than numbers deliberately: a summary is displayed and never computed against, and keeping
    /// it as label/value pairs means every report can share one result shape instead of needing a
    /// summary type of its own.
    /// </summary>
    public List<ReportSummaryValueDto> Summary { get; set; } = [];

    /// <summary>When this ran. Shown on the page so a reader knows how stale the numbers are.</summary>
    public DateTimeOffset GeneratedOn { get; set; }

    /// <summary>
    /// True when the result hit <see cref="RowLimit"/> and rows were left off the end.
    /// </summary>
    /// <remarks>
    /// Reports load their whole result set, which is what makes client-side sorting and export
    /// trivially correct. This flag is the tripwire for that decision: a report that starts setting it
    /// has outgrown the approach and wants server-side paging - and gives up client-side export at the
    /// same time. It is surfaced in the UI rather than logged, because silently truncated numbers are
    /// worse than no numbers.
    /// </remarks>
    public bool Truncated { get; set; }

    /// <summary>The cap <see cref="Truncated"/> refers to.</summary>
    public int RowLimit { get; set; }
}

/// <summary>
/// What every report endpoint returns: the rows, the figures that summarise them, and when it was run.
/// </summary>
/// <remarks>
/// <para>
/// The shape exists so that a report is <em>an endpoint plus a row type</em> and nothing more. The Panel
/// is one consumer of that endpoint; the CSV export is a second; a scheduled or emailed report later is
/// a third. None of them needs the reporting component to reach into the database, which is what keeps
/// them from diverging.
/// </para>
/// <para>
/// <strong><see cref="ReportResultBase.Summary"/> is computed server-side, over the whole result.</strong>
/// Totalling in the browser instead would work right up until a report exceeded
/// <see cref="ReportResultBase.RowLimit"/>, at which point the total would quietly start describing only
/// the rows that happened to be loaded - a wrong number that looks exactly like a right one.
/// </para>
/// </remarks>
/// <typeparam name="TRow">The row type this report returns - one per report.</typeparam>
public class ReportResult<TRow> : ReportResultBase
{
    public List<TRow> Rows { get; set; } = [];
}

/// <summary>
/// One choice in a report's dropdown filter - a plan, a status, whatever the report narrows by.
/// </summary>
/// <remarks>
/// Values are strings because they end up in the page's query string either way (see the Panel's
/// <c>SelectParameter</c>), and a report that filters by plan id and one that filters by a status name
/// have no reason to need different endpoints to populate their dropdowns.
/// </remarks>
public class ReportFilterOptionDto
{
    /// <summary>What goes in the URL and comes back as the filter value.</summary>
    public string Value { get; set; } = string.Empty;

    /// <summary>What the dropdown shows.</summary>
    public string Label { get; set; } = string.Empty;

    /// <summary>Optional grouping, for a dropdown long enough to want <c>optgroup</c>s.</summary>
    public string? Group { get; set; }
}

/// <summary>One figure in a report's summary strip.</summary>
public class ReportSummaryValueDto
{
    public string Label { get; set; } = string.Empty;

    /// <summary>The figure, already formatted for display - currency symbols, thousands separators and
    /// all. See <see cref="ReportResultBase.Summary"/> for why this isn't a number.</summary>
    public string Value { get; set; } = string.Empty;

    /// <summary>Optional smaller line under the value - a qualifier, a caveat, a breakdown.</summary>
    public string? Detail { get; set; }
}
