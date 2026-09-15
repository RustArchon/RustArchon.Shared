// Copyright ©2026 Scott Blomfield

using System;
using System.ComponentModel.DataAnnotations;

namespace RustArchon.Shared.DTOs;

/// <summary>
/// Request body for an admin redeeming a code on a specific Organization's behalf - the same
/// <c>DiscountService.RedeemAsync</c> rules as self-service, just naming the tenant explicitly instead of
/// taking it from the caller's own token.
/// </summary>
public class AdminRedeemDiscountRequestDto
{
    [Required]
    public Guid TenantId { get; set; }

    [Required]
    public string Code { get; set; } = string.Empty;
}
