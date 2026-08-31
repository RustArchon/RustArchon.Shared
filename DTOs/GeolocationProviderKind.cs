// Copyright ©2026 Scott Blomfield

namespace RustArchon.Shared.DTOs;

/// <summary>
/// Which IP geolocation/VPN-detection provider a server is configured to use, if any. Shared between
/// <c>RustArchon.Api</c> (dispatches to the matching <c>IGeolocationProvider</c>) and
/// <c>RustArchon.Panel</c> (renders the selection dropdown), so both sides agree on the same values.
/// </summary>
public enum GeolocationProviderKind
{
    /// <summary>No provider configured - geolocation/VPN lookups are skipped entirely for this server.</summary>
    None,
    ProxyCheckIo,
    IpHubInfo,
    IpInfoIo
}
