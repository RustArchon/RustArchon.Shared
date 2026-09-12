// Copyright ©2026 Scott Blomfield

namespace RustArchon.Shared.DTOs;

/// <summary>
/// The platform's own display name and public site URL - safe to expose to anyone, signed in or not,
/// unlike the rest of <see cref="PlatformSettingDto"/>'s surface (which includes things like SMTP
/// passwords). See <c>PublicBrandingController</c>.
/// </summary>
public class SiteBrandingDto
{
    public string SiteName { get; set; } = string.Empty;
    public string SiteUrl { get; set; } = string.Empty;
}
