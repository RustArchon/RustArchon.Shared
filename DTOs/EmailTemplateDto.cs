// Copyright ©2026 Scott Blomfield

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RustArchon.Shared.DTOs;

/// <summary>One editable email template - see <c>RustArchon.Api.Data.EmailTemplate</c>. The
/// Subject/HtmlBody wording itself lives one level down, in <see cref="Translations"/> - one per
/// language it's been translated into.</summary>
public class EmailTemplateDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    /// <summary>The placeholders this template currently uses - editable through
    /// <c>EmailTemplatesController.UpdatePlaceholders</c>, not just a hint. Shared across every
    /// language in <see cref="Translations"/>, not per-translation.</summary>
    public List<EmailPlaceholderDto> Placeholders { get; set; } = [];

    /// <summary>Every language this template currently has wording for - never one row per
    /// <em>expected</em> language, only ones actually saved. The Panel admin editor computes which
    /// expected languages are missing by comparing this list's <see cref="EmailTemplateTranslationDto.Culture"/>
    /// values against its own compiled-in resource cultures - see <c>EmailTemplateDetail.razor</c>.</summary>
    public List<EmailTemplateTranslationDto> Translations { get; set; } = [];
}

/// <summary>One language's Subject/HtmlBody for a template - see
/// <c>RustArchon.Api.Data.EmailTemplateTranslation</c>.</summary>
public class EmailTemplateTranslationDto
{
    /// <summary>The culture this translation is written in (e.g. <c>"en-US"</c>) - matches a
    /// RustArchon.Panel compiled resource culture, though this Api never checks that itself.</summary>
    public string Culture { get; set; } = string.Empty;

    public string Subject { get; set; } = string.Empty;
    public string HtmlBody { get; set; } = string.Empty;
}

/// <summary>One reusable <c>{{Token}}</c> placeholder - see <c>RustArchon.Api.Data.EmailPlaceholder</c>.
/// <see cref="Name"/> is code-owned (it's the literal substitution key real sending code fills in) and
/// never editable through the Panel; <see cref="Description"/>/<see cref="Sample"/> are the admin's own,
/// via <c>EmailPlaceholdersController</c>.</summary>
public class EmailPlaceholderDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Sample { get; set; } = string.Empty;
}

/// <summary>
/// The two fields an admin can actually change about one language's translation of a template -
/// everything else on <see cref="EmailTemplateDto"/> (<see cref="EmailTemplateDto.Code"/>,
/// <see cref="EmailTemplateDto.Name"/>, etc.) is code-defined metadata synced by
/// <c>EmailTemplateRegistry</c>, the same split <c>UpdatePlatformSettingValueDto</c> draws for platform
/// settings. Unlike that DTO's <c>Value</c>, both fields here are <c>[Required]</c> on purpose - an
/// empty subject or body is never a legitimate saved translation, only ever a mistake.
/// </summary>
/// <remarks>
/// Upserts, not just updates: <c>EmailTemplatesController.UpsertTranslation</c> creates the row for
/// <see cref="Culture"/> if it doesn't already exist - the Panel editor's "add a translation" and "edit
/// an existing translation" are the same call, per Scott's choice not to give each language its own
/// editor page.
/// </remarks>
public class UpdateEmailTemplateTranslationDto
{
    [Required]
    [MaxLength(20)]
    public string Culture { get; set; } = string.Empty;

    [Required]
    public string Subject { get; set; } = string.Empty;

    [Required]
    public string HtmlBody { get; set; } = string.Empty;
}

/// <summary>Which placeholders a template uses, by <see cref="EmailPlaceholderDto.Name"/> - the whole
/// set, not a delta; see <c>EmailTemplatesController.UpdatePlaceholders</c>.</summary>
public class UpdateEmailTemplatePlaceholdersDto
{
    public List<string> PlaceholderNames { get; set; } = [];
}

/// <summary>The two fields an admin can change about a placeholder - never its <see cref="EmailPlaceholderDto.Name"/>,
/// the literal substitution key real sending code depends on.</summary>
public class UpdateEmailPlaceholderDto
{
    [MaxLength(500)]
    public string? Description { get; set; }

    [Required]
    public string Sample { get; set; } = string.Empty;
}
