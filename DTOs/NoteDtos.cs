// Copyright ©2026 Scott Blomfield

using System;
using System.ComponentModel.DataAnnotations;

namespace RustArchon.Shared.DTOs;

/// <summary>A site admin's own annotation on an Organization, a person, or both.</summary>
public class NoteDto
{
    public Guid Id { get; set; }

    public string? Title { get; set; }

    public string Content { get; set; } = string.Empty;

    public Guid? TenantId { get; set; }

    public Guid? UserId { get; set; }

    public bool IsPrivate { get; set; }

    public Guid CreatedById { get; set; }

    public DateTimeOffset CreatedOn { get; set; }

    public DateTimeOffset? ModifiedOn { get; set; }

    /// <summary>
    /// Whether the caller who asked for this note may edit or delete it.
    /// </summary>
    /// <remarks>
    /// Decided by the Api, not the Panel: the rule is "anyone may act on a public note, only the
    /// author may act on a private one", and the Api is the only side that knows which user is
    /// asking. Carrying the answer here means a screen only ever renders a button, never re-derives
    /// the permission itself.
    /// </remarks>
    public bool CanManage { get; set; }
}

/// <summary>A request to create or update a note. One shape for both, like <c>SaveRoleRequestDto</c>.</summary>
public class SaveNoteRequestDto
{
    [MaxLength(256)]
    public string? Title { get; set; }

    [Required]
    [MaxLength(4000)]
    public string Content { get; set; } = string.Empty;

    /// <summary>The Organization this note is about. Required unless <see cref="UserId"/> is set.</summary>
    public Guid? TenantId { get; set; }

    /// <summary>The person this note is about. Required unless <see cref="TenantId"/> is set.</summary>
    public Guid? UserId { get; set; }

    public bool IsPrivate { get; set; }
}
