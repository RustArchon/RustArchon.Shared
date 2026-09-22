// Copyright ©2026 Scott Blomfield

using System;
using System.ComponentModel.DataAnnotations;
using RustArchon.Shared.PluginZips;

namespace RustArchon.Shared.DTOs;

/// <summary>
/// What can be done about one outdated third-party plugin on a server: whether a checked file is waiting to be applied, whether an update is under way,
/// or how the last one went. Only plugins with something to say appear.
/// </summary>
public class ThirdPartyPluginUpdateOfferDto
{
    /// <summary>The plugin, as the server's plugin list names it.</summary>
    public string PluginName { get; set; } = string.Empty;

    /// <summary>
    /// One of <c>ready</c>, <c>applying</c>, <c>applied</c>, <c>rolled-back</c>, <c>failed</c>, <c>refused</c>, <c>changed</c>, <c>needs-instructions</c>,
    /// <c>not-applicable</c>. A code for the Panel to explain in the person's language, not text to show.
    /// </summary>
    public string State { get; set; } = string.Empty;

    /// <summary>
    /// What the server's plugin or the file check said (why a file cannot be applied, why an update was rolled back), for a person to read. It is text
    /// from elsewhere: shown as text and never as markup.
    /// </summary>
    public string Detail { get; set; } = string.Empty;

    /// <summary>The SHA-256 of the file that would be applied. Sent back with the request to apply it, so a file that changed after it was looked at is never applied.</summary>
    public string FileSha256 { get; set; } = string.Empty;

    /// <summary>Whether a person can apply it now: the plugin on the server can do it, nothing else is in progress, and there is a file to apply.</summary>
    public bool CanApply { get; set; }

    /// <summary>When automatic updates resume, if this server is inside its days-before-wipe window; otherwise null. Applying by hand is still possible.</summary>
    public DateTimeOffset? AutomaticUpdatesPausedUntilUtc { get; set; }

    /// <summary><c>cs</c> for a single plugin file, <c>zip</c> for an archive, which is applied by folder rules a person gives.</summary>
    public string Kind { get; set; } = "cs";

    /// <summary>For a zip: the files in it (path and declared size), so instructions can be given. Empty for anything else.</summary>
    public List<ZipEntryInfo> Files { get; set; } = [];

    /// <summary>For a zip: the instructions saved for this plugin on this server, to start from. Empty if there are none.</summary>
    public List<ZipMappingRule> SavedRules { get; set; } = [];

    /// <summary>For a zip whose saved instructions no longer cover it: the files (up to 20) they leave without a decision, which is why they are not used by themselves.</summary>
    public List<string> UncoveredPaths { get; set; } = [];

    /// <summary>Whether the saved instructions have produced a working update, so an automatic update may use them.</summary>
    public bool MappingTrusted { get; set; }
}

/// <summary>A request to apply a plugin's update now, by hand.</summary>
public class ApplyThirdPartyPluginUpdateDto
{
    /// <summary>The plugin, as the server's plugin list names it.</summary>
    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string? PluginName { get; set; }

    /// <summary>
    /// The SHA-256 of the file the person was shown. Required: the update is applied only if the file is still that one. Without it an author who
    /// replaced a file between the page loading and the click would have the new file applied unseen.
    /// </summary>
    [Required]
    [StringLength(64, MinimumLength = 64)]
    public string? FileSha256 { get; set; }

    /// <summary>
    /// For a zip: which folders go where, and what is skipped. Left out to use the instructions saved for this plugin on this server. Checked in full
    /// before anything is sent (every file installed or skipped, nothing unsafe or ambiguous), so a request with a bad set is refused with the reason.
    /// </summary>
    [MaxLength(ZipMapping.MaxRules)]
    public List<ZipMappingRule>? Rules { get; set; }

    /// <summary>For a zip given <see cref="Rules"/>: keep them for this plugin on this server, so a later update containing the same files in the same structure needs no asking.</summary>
    public bool SaveMapping { get; set; }
}

/// <summary>A request to download the file behind a plugin's update again, right now, rather than trusting whatever was last recorded.</summary>
public class RecheckThirdPartyPluginFileDto
{
    /// <summary>The plugin, as the server's plugin list names it.</summary>
    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string? PluginName { get; set; }
}

/// <summary>A request to say that a plugin's update notice does not apply here - never applied automatically or by hand until lifted. Also used to lift one: <see cref="Note"/> is left out then.</summary>
public class ExcludeThirdPartyPluginUpdateDto
{
    /// <summary>The plugin, as the server's plugin list names it.</summary>
    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string? PluginName { get; set; }

    /// <summary>Why, in the person's own words - shown back to them wherever the exclusion is. Never required.</summary>
    [StringLength(500)]
    public string? Note { get; set; }
}
