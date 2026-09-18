// Copyright ©2026 Scott Blomfield

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RustArchon.Messaging.Contracts;

namespace RustArchon.Shared.DTOs;

/// <summary>
/// One Organization as a site admin sees it in the list - identity, what it is on, and whether it owes
/// anything.
/// </summary>
/// <remarks>
/// <para>
/// Deliberately denormalised. The point of the list is to find an account and see its shape without
/// opening it, so the plan, the renewal date, the server count and the balance are all carried here
/// rather than left to a second request per row.
/// </para>
/// <para>
/// Distinct from the reports' row DTOs, which each answer one question across the whole population
/// (what renews soon, who owes money). This answers "who is this customer", which is the question the
/// reports could never answer because none of them is about an individual account.
/// </para>
/// </remarks>
public class OrganizationSummaryDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? ContactEmail { get; set; }

    public DateTimeOffset CreatedOn { get; set; }

    /// <summary>Set once the Organization has been cancelled - see <see cref="Status"/>.</summary>
    public DateTimeOffset? CancelledOn { get; set; }

    public string PlanName { get; set; } = string.Empty;

    public string PlanColorCode { get; set; } = "#888888";

    public int TermMonths { get; set; }

    public SubscriptionStatus Status { get; set; }

    /// <summary>When <see cref="Status"/> last moved, if it ever has.</summary>
    public DateTimeOffset? StatusChangedOn { get; set; }

    /// <summary>Why it was moved, in the words of whoever moved it.</summary>
    public string? StatusReason { get; set; }

    /// <summary>End of the current billing period. Null for a cancelled Organization.</summary>
    public DateTimeOffset? RenewsOn { get; set; }

    /// <summary>Server slots bought - see <c>SubscriptionPeriod.Quantity</c>.</summary>
    public int Quantity { get; set; }

    public int ServerCount { get; set; }

    /// <summary>What this account is worth per month at list prices, for the term it is on.</summary>
    public decimal MonthlyValue { get; set; }

    /// <summary>Everything still owed across every open invoice.</summary>
    public decimal Outstanding { get; set; }

    public int OpenInvoiceCount { get; set; }

    public int MemberCount { get; set; }

    /// <summary>When this Organization moved onto its current plan.</summary>
    public DateTimeOffset OnPlanSince { get; set; }
}

/// <summary>Everything behind one Organization, for the admin detail page.</summary>
public class OrganizationDetailDto
{
    public OrganizationSummaryDto Summary { get; set; } = new();

    /// <summary>The plan-history intervals, newest first - see <c>Subscription</c>.</summary>
    public List<OrganizationIntervalDto> History { get; set; } = [];

    /// <summary>Recent billing periods, newest first - what was earned and over which dates.</summary>
    public List<OrganizationPeriodDto> Periods { get; set; } = [];

    public List<OrganizationServerDto> Servers { get; set; } = [];

    public List<InvoiceDto> Invoices { get; set; } = [];

    public List<OrganizationMemberDto> Members { get; set; } = [];

    /// <summary>The roles defined within this Organization, for assigning to its members.</summary>
    public List<OrganizationRoleDto> Roles { get; set; } = [];

    /// <summary>
    /// How many notes the caller can see on this Organization - for the Notes tab's badge, the same
    /// way <see cref="Servers"/>.Count and <see cref="Invoices"/>.Count feed theirs. Not the full list:
    /// <c>NoteList</c> only fetches when the tab is actually opened, and a private note excluded from
    /// that fetch is excluded from this count too - see <c>OrganizationAdminService.GetAsync</c>.
    /// </summary>
    public int NoteCount { get; set; }

    /// <summary>A plan change the Organization has accepted that has not taken effect yet.</summary>
    public OrganizationPendingChangeDto? PendingChange { get; set; }
}

/// <summary>
/// An Organization's own view of its roles, for the screen where it defines them.
/// </summary>
public class OrganizationRolesDto
{
    /// <summary>Whether this Organization's plan includes defining roles of its own.</summary>
    /// <remarks>
    /// False does not hide the screen - it explains it. Somebody on a plan without role separation
    /// should see what the feature is and that their plan does not include it, rather than an empty
    /// page that looks broken.
    /// </remarks>
    public bool PlanIncludesRoles { get; set; }

    /// <summary>
    /// Whether the person asking may change these roles, as opposed to only read them.
    /// </summary>
    /// <remarks>
    /// A second, independent gate, and the screen needs both. Reading this list requires only
    /// <c>Organization.ManageMembers</c> - you have to know which roles exist to hand one out - while
    /// changing what a role <em>means</em> requires <c>Organization.ManageRoles</c>. Without this
    /// flag the editor would be offered to somebody whose save is then refused, which is a worse way
    /// to learn the same thing.
    /// </remarks>
    public bool CanEditRoles { get; set; }

    /// <summary>The roles, built-in first.</summary>
    public List<OrganizationRoleDto> Roles { get; set; } = [];

    /// <summary>The permissions this Organization may put into a role, for the editor.</summary>
    public List<PermissionOptionDto> GrantablePermissions { get; set; } = [];
}

/// <summary>One permission a customer can put into a role.</summary>
public class PermissionOptionDto
{
    public string Name { get; set; } = string.Empty;
    public string Group { get; set; } = string.Empty;
    public string? Description { get; set; }
}

/// <summary>Creating or renaming one of an Organization's own roles.</summary>
public class SaveRoleRequestDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

/// <summary>Replacing the permissions a role grants.</summary>
public class SetRolePermissionsRequestDto
{
    public List<string> Permissions { get; set; } = [];
}

/// <summary>A role an Organization holds - one it defined, or the built-in Owner.</summary>
/// <remarks>
/// The platform-wide "Site Admin" role is never listed here: it is not something an Organization
/// grants, and showing it would invite trying.
/// </remarks>
public class OrganizationRoleDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int MemberCount { get; set; }

    /// <summary>
    /// True for the shared, platform-wide Owner role every Organization holds.
    /// </summary>
    /// <remarks>
    /// Built-in roles are read-only to a customer: one definition serves every Organization, so
    /// letting one of them rename it or change what it grants would change it for all of them.
    /// </remarks>
    public bool IsBuiltIn { get; set; }

    /// <summary>The permissions this role grants.</summary>
    public List<string> Permissions { get; set; } = [];
}

/// <summary>One interval of the plan history.</summary>
public class OrganizationIntervalDto
{
    public Guid Id { get; set; }
    public string PlanName { get; set; } = string.Empty;
    public string PlanColorCode { get; set; } = "#888888";
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset? EndDate { get; set; }
    public SubscriptionStatus Status { get; set; }
    public int PeriodCount { get; set; }
}

/// <summary>One billable span - see <c>SubscriptionPeriod</c>.</summary>
public class OrganizationPeriodDto
{
    public Guid Id { get; set; }
    public string PlanName { get; set; } = string.Empty;
    public int TermMonths { get; set; }
    public int Quantity { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public DateTimeOffset PeriodStart { get; set; }
    public DateTimeOffset PeriodEnd { get; set; }
    public decimal EarnedAmount { get; set; }

    /// <summary>False when a change split this period - see <c>SubscriptionPeriod.IsWholePeriod</c>.</summary>
    public bool IsWholePeriod { get; set; }

    /// <summary>The invoice number this span was billed on, if it was billed at all.</summary>
    public string? InvoiceNumber { get; set; }
}

/// <summary>
/// One of the Organization's servers, as a site admin sees it.
/// </summary>
/// <remarks>
/// Carries the operational fields the tenant's own <c>RustServerDto</c> does not - the assigned worker
/// and the last heartbeat - because the reason a site admin is looking at someone else's server is
/// nearly always that it is not connecting, and those two fields are the first things to check. The RCON
/// password is never carried, here or anywhere else.
/// </remarks>
public class OrganizationServerDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; }
    public string? Description { get; set; }
    public bool IsEnabled { get; set; }
    public RconConnectionStatus ConnectionStatus { get; set; }
    public string? ConnectionStatusDetail { get; set; }
    public DateTimeOffset? ConnectionStatusChangedAtUtc { get; set; }
    public Guid? AssignedWorkerId { get; set; }
    public DateTimeOffset? LastHeartbeatUtc { get; set; }
    public DateTimeOffset CreatedOn { get; set; }
}

/// <summary>
/// One person's membership of an Organization.
/// </summary>
/// <remarks>
/// <strong>Carries a user id and no name.</strong> Identity lives in the Panel's own store and
/// membership lives here, in the Api's - two databases, deliberately, and this side genuinely does not
/// know who <see cref="UserId"/> is. The Panel joins the two when it renders the page, because the Panel
/// is the only component that can see both. See <c>OrganizationDetail.razor</c>.
/// </remarks>
public class OrganizationMemberDto
{
    public Guid UserId { get; set; }
    public bool IsActive { get; set; }
    public DateTimeOffset JoinedOn { get; set; }

    /// <summary>Roles this user holds *within this Organization* - not platform-wide ones.</summary>
    public List<string> Roles { get; set; } = [];

    /// <summary>The same roles by id, so the admin screen can toggle them.</summary>
    public List<Guid> RoleIds { get; set; } = [];
}

/// <summary>
/// One person as the platform sees them, across every Organization they belong to.
/// </summary>
/// <remarks>
/// Carries no name or email for the same reason <see cref="OrganizationMemberDto"/> doesn't: the Api has
/// no users table. The Panel merges this with its own Identity store to produce the directory a human can
/// read.
/// </remarks>
public class PlatformUserDto
{
    public Guid UserId { get; set; }

    /// <summary>Whether this user holds the platform-wide "Site Admin" role.</summary>
    public bool IsSiteAdmin { get; set; }

    public List<PlatformUserMembershipDto> Organizations { get; set; } = [];
}

/// <summary>Whether the caller themselves currently holds the platform-wide "Site Admin" role.</summary>
/// <remarks>
/// Reachability is the whole answer: <c>PlatformUsersController</c> is gated by the
/// <c>ManageOrganizations</c> permission, which today only Site Admins hold, so a 200 here always
/// carries <see cref="IsSiteAdmin"/> true and a refusal (403) means false - see
/// <c>PlatformUsersController.GetMyStatus</c>. Self-scoped, unlike <see cref="PlatformUserDto"/>: a
/// customer-facing page (e.g. the Organization's own Members screen) asks this about the person
/// looking at it, to decide whether a link to the site-admin-only user page is worth offering, not to
/// look anyone else up.
/// </remarks>
public class PlatformAdminStatusDto
{
    public bool IsSiteAdmin { get; set; }
}

/// <summary>One of a user's Organization memberships, as listed in the platform directory.</summary>
public class PlatformUserMembershipDto
{
    public Guid TenantId { get; set; }
    public string TenantName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public List<string> Roles { get; set; } = [];
}

/// <summary>Moving an Organization between Active, PastDue and Suspended.</summary>
public class SetOrganizationStatusRequestDto
{
    public SubscriptionStatus Status { get; set; }

    /// <summary>Required when suspending - see <c>OrganizationsController.SetStatus</c>.</summary>
    public string? Reason { get; set; }
}

/// <summary>
/// Why an Organization's subscription is being cancelled - picks which email template
/// <c>OrganizationLifecycleService.CancelAsync</c> sends, alongside the free-text
/// <see cref="CancelOrganizationRequestDto.Reason"/> a site admin can add for detail.
/// </summary>
public enum CancellationReasonCategory
{
    CustomerRequest,
    NonPayment,

    /// <summary>Routes to <c>EmailTemplateRegistry.Codes.TosViolationNotice</c> instead of the ordinary
    /// cancellation template - the account is being closed specifically because of a policy violation,
    /// not left on ordinary terms.</summary>
    TosViolation,

    Other
}

/// <summary>Ending an Organization.</summary>
public class CancelOrganizationRequestDto
{
    public CancellationReasonCategory Category { get; set; }

    /// <summary>Optional extra detail included in the notice email, alongside <see cref="Category"/>.</summary>
    public string? Reason { get; set; }
}

/// <summary>Bringing a cancelled Organization back.</summary>
public class ReopenOrganizationRequestDto
{
    public Guid PlanId { get; set; }

    public int TermMonths { get; set; } = BillingTerms.Monthly;
}

/// <summary>
/// Moving an already-active Organization onto a different plan immediately - see
/// <c>OrganizationLifecycleService.ForcePlanChangeAsync</c> for exactly what this bypasses (the
/// upgrade/downgrade timing rule, proration, invoicing) versus what still applies (the target plan must
/// exist, be active, and be able to hold the Organization's current servers).
/// </summary>
public class AdminForcePlanChangeRequestDto
{
    [Required]
    public Guid PlanId { get; set; }

    [Range(1, 120)]
    public int TermMonths { get; set; } = BillingTerms.Monthly;

    /// <summary>Slots to hold, or null to derive them the same way a normal change would.</summary>
    public int? Quantity { get; set; }

    /// <summary>Required - this is an override of the normal billing rules, and the first thing anyone
    /// asks afterwards is why it happened.</summary>
    [Required]
    [MaxLength(500)]
    public string Reason { get; set; } = string.Empty;
}

/// <summary>Adding somebody to an Organization.</summary>
public class AddMemberRequestDto
{
    public Guid UserId { get; set; }

    /// <summary>Optionally assign a role at the same time - usually the Organization's Owner role.</summary>
    public Guid? RoleId { get; set; }
}

/// <summary>A plan change accepted but not yet in effect - see <c>ScheduledPlanChange</c>.</summary>
public class OrganizationPendingChangeDto
{
    public Guid Id { get; set; }
    public string PlanName { get; set; } = string.Empty;
    public int TermMonths { get; set; }
    public int Quantity { get; set; }
    public DateTimeOffset EffectiveDate { get; set; }
    public DateTimeOffset CreatedOn { get; set; }
}

/// <summary>Corrections to a server's connection details, made on the Organization's behalf.</summary>
/// <remarks>
/// Deliberately narrower than the tenant's own edit form: a site admin fixing a customer's server is
/// fixing why it will not connect, not renaming it or re-keying their Steam integration.
/// </remarks>
public class UpdateServerConnectionRequestDto
{
    public string Host { get; set; } = string.Empty;

    public int Port { get; set; } = 28016;

    /// <summary>Blank leaves the stored password alone - it is write-only and never sent back.</summary>
    public string? RconPassword { get; set; }
}

/// <summary>An RCON command sent to a customer's server by a site admin.</summary>
public class SendServerCommandRequestDto
{
    public string Command { get; set; } = string.Empty;
}

/// <summary>What the Organizations list is being asked for.</summary>
public class OrganizationQueryDto
{
    /// <summary>Matched against name and contact email, case-insensitively.</summary>
    public string? Search { get; set; }

    public Guid? PlanId { get; set; }

    public SubscriptionStatus? Status { get; set; }

    /// <summary>Include Organizations that have been cancelled. Off by default.</summary>
    public bool IncludeCancelled { get; set; }

    /// <summary>Only those owing at least this much.</summary>
    public decimal MinimumOutstanding { get; set; }
}
