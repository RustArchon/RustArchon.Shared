// Copyright ©2026 Scott Blomfield

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RustArchon.Shared.DTOs;

/// <summary>Who an announcement goes to: the organizations on a plan, narrowed by two choices.</summary>
public class AnnouncementCriteriaDto
{
    /// <summary>
    /// False: only organizations on the plan itself. True: on the plan <em>and every plan that superseded it</em> (its newer versions, going forward - never
    /// the versions before it).
    /// </summary>
    public bool IncludeSupersedingVersions { get; set; }

    /// <summary>Whether organizations with an overdue invoice are included. Off by default: they are still customers, but it is a choice, not an assumption.</summary>
    public bool IncludePastDue { get; set; }
}

/// <summary>
/// How the words are sent. <see cref="PerLanguage"/>: the admin wrote a version for each language and each organization gets its own language's. <see cref="SingleLanguage"/>:
/// the admin picked one version and everyone gets it, whatever their language.
/// </summary>
public enum AnnouncementLanguageMode
{
    PerLanguage = 0,
    SingleLanguage = 1
}

/// <summary>One language's version of the message.</summary>
public class AnnouncementVersionDto
{
    /// <summary>The language, e.g. <c>en-US</c>.</summary>
    [Required]
    [MaxLength(35)]
    public string Culture { get; set; } = string.Empty;

    /// <summary>The subject, one line. May use <c>{{OrganizationName}}</c>, <c>{{PlanName}}</c> and <c>{{SiteName}}</c>.</summary>
    [Required]
    [MaxLength(200)]
    public string Subject { get; set; } = string.Empty;

    /// <summary>
    /// The message, as the editor's own structured content (a Quill Delta, as JSON) - <b>not HTML</b>. The Api builds the email's HTML from it and nothing
    /// else, so nothing the editor sends can carry markup into an email. May use the same tokens as the subject.
    /// </summary>
    [Required]
    [MaxLength(200_000)]
    public string BodyDelta { get; set; } = string.Empty;
}

/// <summary>The words of an announcement: a version per language, or one chosen for everyone.</summary>
public class AnnouncementContentDto
{
    public AnnouncementLanguageMode Mode { get; set; }

    /// <summary>For <see cref="AnnouncementLanguageMode.SingleLanguage"/>: which of <see cref="Versions"/> everyone gets.</summary>
    [MaxLength(35)]
    public string? SingleCulture { get; set; }

    [MinLength(1)]
    [MaxLength(20)]
    public List<AnnouncementVersionDto> Versions { get; set; } = [];
}

/// <summary>A count with its name - a plan version, a language, or a reason.</summary>
public class AnnouncementCountDto
{
    /// <summary>What is being counted (a plan's id, a culture name, ...), for the caller to key on.</summary>
    public string Key { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public int Count { get; set; }
}

/// <summary>Who an announcement would reach, worked out from the criteria and nothing sent.</summary>
public class AnnouncementPreviewDto
{
    /// <summary>How many organizations would be emailed.</summary>
    public int Recipients { get; set; }

    /// <summary>Recipients by plan version.</summary>
    public List<AnnouncementCountDto> ByPlanVersion { get; set; } = [];

    /// <summary>
    /// Recipients by the language of the person who created the organization (their profile's language; a blank <see cref="AnnouncementCountDto.Key"/> is
    /// "none set", which gets the platform's default language). Where a per-language send picks each organization's version.
    /// </summary>
    public List<AnnouncementCountDto> ByLanguage { get; set; } = [];

    /// <summary>
    /// Organizations on the plan(s) that would not be emailed, and why: <c>past_due_left_out</c>, <c>not_in_good_standing</c> (suspended, cancelled or inactive),
    /// <c>no_contact_email</c>.
    /// </summary>
    public List<PlanMoveReasonDto> LeftOut { get; set; } = [];

    /// <summary>The language a recipient with none set (or none of the versions' languages) is sent, per the platform's default.</summary>
    public string DefaultCulture { get; set; } = string.Empty;
}

/// <summary>Sending an announcement.</summary>
public class SendPlanAnnouncementDto
{
    [Required]
    public AnnouncementCriteriaDto Criteria { get; set; } = new();

    [Required]
    public AnnouncementContentDto Content { get; set; } = new();

    /// <summary>
    /// How many organizations the admin was shown and confirmed. If the real number is different by the time this arrives (an organization joined, left or
    /// changed standing) nothing is sent and the admin is asked to look again - what they confirmed is what goes out.
    /// </summary>
    public int ExpectedRecipients { get; set; }
}

/// <summary>Sending each language's version of an announcement to one address, to see how it will look.</summary>
public class SendAnnouncementTestDto
{
    [Required]
    public AnnouncementContentDto Content { get; set; } = new();

    [Required]
    [EmailAddress]
    [MaxLength(320)]
    public string ToAddress { get; set; } = string.Empty;
}

/// <summary>What an announcement send did.</summary>
public class AnnouncementResultDto
{
    /// <summary>The send's record, or null for a test.</summary>
    public Guid? BatchId { get; set; }

    /// <summary>How many emails were queued.</summary>
    public int Queued { get; set; }

    public List<PlanMoveReasonDto> LeftOut { get; set; } = [];
}

/// <summary>One past send, for the history under a plan.</summary>
public class AnnouncementBatchDto
{
    public Guid Id { get; set; }
    public DateTimeOffset SentOn { get; set; }
    public string Subject { get; set; } = string.Empty;
    public int Recipients { get; set; }
    public int LeftOut { get; set; }
    public bool IncludeSupersedingVersions { get; set; }
    public bool IncludePastDue { get; set; }
    public string LanguageMode { get; set; } = string.Empty;
}
