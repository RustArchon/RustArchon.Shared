// Copyright ©2026 Scott Blomfield

using System;
using System.Collections.Generic;

namespace RustArchon.Shared.DTOs;

/// <summary>
/// Where a theme's content actually came from - not the same question as who is credited as its
/// <c>AuthorName</c>. <see cref="Uploaded"/> covers every path where the bytes originated outside this
/// admin's own typing: a manual .zip upload, <c>DefaultThemeSeeder</c>'s embedded seed package, and
/// <c>ThemeService.InstallUpdateAsync</c>'s downloaded package all set this (see
/// <c>ThemeService.UploadAsync</c>'s own default). <see cref="Built"/> is set only by
/// <c>ThemePackageBuilder</c>'s path - a package assembled from the Panel's own theme-builder form
/// fields, with no external package ever involved. The distinction exists for exactly one reason: an
/// admin editing an <see cref="Uploaded"/> theme in the builder is customizing something this instance
/// didn't author and has no way to keep in sync - see the Panel's own warning on that page.
/// </summary>
public enum ThemeSource
{
    Uploaded,
    Built
}

/// <summary>One row in the admin Themes list.</summary>
public class ThemeSummaryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    public bool IsActive { get; set; }
    public DateTimeOffset UploadedOn { get; set; }
    public ThemeSource Source { get; set; }

    /// <summary>Whether the theme's own <c>UpdateUrl</c> has reported a version newer than
    /// <see cref="Version"/> - computed server-side from <c>Theme.LatestKnownVersion</c> via
    /// <c>ThemeVersionComparer</c>, not stored, since a theme's own <see cref="Version"/> never changes
    /// after upload (see <c>ThemeService.UploadAsync</c>'s remarks - a revised look is a new upload).</summary>
    public bool UpdateAvailable { get; set; }
}

/// <summary>The full record behind one theme - what a click on a <see cref="ThemeSummaryDto"/> row
/// opens, or what an upload/activate call returns. The fields beyond <see cref="ThemeSummaryDto"/> all
/// come from the package's own <c>manifest.json</c> - see <c>RustArchon.Api.Data.Theme</c>'s remarks.</summary>
public class ThemeDetailDto : ThemeSummaryDto
{
    /// <summary>Every relative path this theme's package contains - see
    /// <c>RustArchon.Api.Data.Theme.AssetPaths</c>.</summary>
    public List<string> AssetPaths { get; set; } = [];

    public string? Description { get; set; }
    public string? AuthorName { get; set; }
    public string? AuthorEmail { get; set; }
    public string? Website { get; set; }
    public string? UpdateUrl { get; set; }

    /// <summary>The version <see cref="UpdateUrl"/> most recently reported - see
    /// <c>RustArchon.Api.Data.Theme.LatestKnownVersion</c>.</summary>
    public string? LatestKnownVersion { get; set; }

    /// <summary>Where to download the package for <see cref="LatestKnownVersion"/>, if the update feed
    /// reported one - see <c>RustArchon.Api.Data.Theme.LatestDownloadPackageUrl</c>. <c>null</c> means an
    /// update was found but can't be installed with one click.</summary>
    public string? LatestDownloadPackageUrl { get; set; }

    /// <summary>When the update check most recently ran, whether it succeeded or failed - <c>null</c> if
    /// one has never run.</summary>
    public DateTimeOffset? LastUpdateCheckOn { get; set; }

    /// <summary>Why the most recent update check failed, or <c>null</c> if it last succeeded (or has
    /// never run).</summary>
    public string? LastUpdateCheckError { get; set; }
}

/// <summary>A failed upload's reasons, from <c>ThemePackageValidator</c> - never partial: an upload
/// either succeeds in full or writes nothing at all.</summary>
public class ThemeUploadErrorDto
{
    public List<string> Errors { get; set; } = [];
}
