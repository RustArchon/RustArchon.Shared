// Copyright ©2026 Scott Blomfield

using System.Collections.Generic;

namespace RustArchon.Shared.DTOs;

/// <summary>
/// Why some of a plan's current subscribers would not be moved to a newer version of it. Each group is one <see cref="Code"/> and how many
/// organizations it applies to (an organization is counted under the first reason that applies to it).
/// </summary>
public class PlanMoveReasonDto
{
    /// <summary>
    /// A stable code the Panel turns into words: <c>price_higher</c>, <c>term_not_offered</c>, <c>pricing_model_changed</c>, <c>fewer_servers</c>,
    /// <c>fewer_users</c>, <c>shorter_retention</c>, <c>roles_removed</c>, <c>automatic_updates_removed</c>, <c>over_server_limit</c>,
    /// <c>not_in_good_standing</c>, <c>renewal_due</c>.
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>English wording, for a client that does not translate the code.</summary>
    public string Message { get; set; } = string.Empty;

    public int Count { get; set; }
}

/// <summary>
/// What moving a plan's current subscribers to a newer version would do, before anything is done. A subscriber is moved only when the newer version is no
/// worse for them - not dearer on their term, not lower on capacity, users or retention, and not missing a feature they have - so a move never changes
/// what someone pays for the worse without their say. Those who would be worse off stay on the version they signed up under.
/// </summary>
public class PlanMovePreviewDto
{
    /// <summary>Organizations currently on the plan (an open subscription).</summary>
    public int CurrentSubscribers { get; set; }

    /// <summary>How many of them would be moved.</summary>
    public int CanMove { get; set; }

    /// <summary>Why the rest would stay, grouped.</summary>
    public List<PlanMoveReasonDto> WouldStay { get; set; } = [];
}

/// <summary>What moving a plan's current subscribers to its latest version did.</summary>
public class PlanMoveResultDto
{
    public int Moved { get; set; }

    /// <summary>How many stayed where they were, and why.</summary>
    public int Stayed { get; set; }

    public List<PlanMoveReasonDto> StayedBecause { get; set; } = [];
}
