// Copyright ©2026 Scott Blomfield

using System;
using System.ComponentModel.DataAnnotations;
using JumpStart.Api.DTOs;
using RustArchon.Messaging.Contracts;

namespace RustArchon.Shared.DTOs;

/// <summary>
/// DTO for reading a registered Rust server. Deliberately excludes the RCON password - it is
/// encrypted at rest and never returned by the API. See <see cref="CreateRustServerDto"/> and
/// <see cref="UpdateRustServerDto"/> for how it is written. Also excludes <c>AssignedWorkerId</c>/
/// <c>LastHeartbeatUtc</c> - internal worker-ownership plumbing, not something the UI needs.
/// </summary>
public class RustServerDto : AuditableEntityDto
{
    public string Name { get; set; } = string.Empty;
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; }
    public string? Description { get; set; }
    public bool IsEnabled { get; set; }
    public RconConnectionStatus ConnectionStatus { get; set; }
    public string? ConnectionStatusDetail { get; set; }
    public DateTimeOffset? ConnectionStatusChangedAtUtc { get; set; }
}

/// <summary>
/// DTO for registering a new Rust server.
/// </summary>
public class CreateRustServerDto : ICreateDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string Host { get; set; } = string.Empty;

    [Range(1, 65535)]
    public int Port { get; set; } = 28016;

    /// <summary>
    /// The RCON password, in plaintext, over the wire only. Encrypted immediately on arrival at
    /// the API before it is ever persisted.
    /// </summary>
    [Required]
    public string RconPassword { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }
}

/// <summary>
/// DTO for updating an existing Rust server.
/// </summary>
public class UpdateRustServerDto : IUpdateDto
{
    public Guid Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string Host { get; set; } = string.Empty;

    [Range(1, 65535)]
    public int Port { get; set; }

    /// <summary>
    /// The new RCON password, in plaintext. Leave <c>null</c> or empty to keep the existing
    /// password unchanged.
    /// </summary>
    public string? RconPassword { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }
}
