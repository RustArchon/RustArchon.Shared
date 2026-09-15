// Copyright ©2026 Scott Blomfield

using System;
using System.ComponentModel.DataAnnotations;

namespace RustArchon.Shared.DTOs;

/// <summary>How much a discount takes off.</summary>
public enum DiscountAmountType
{
    /// <summary>The amount value is a percentage (0-100) of the invoice amount.</summary>
    PercentOff,

    /// <summary>The amount value is a flat currency amount, capped at the invoice amount (a discount
    /// never makes an invoice negative).</summary>
    FlatAmountOff
}

/// <summary>
/// How long a discount keeps applying once redeemed.
/// </summary>
/// <remarks>
/// Only <see cref="OneTime"/> is implemented - see <c>RustArchon.Api.Data.Discount</c>'s own remarks.
/// The enum exists now, with the second value reserved and unused, so that adding real
/// recurring-duration support later is a new code path rather than a breaking schema change to every
/// already-issued redemption.
/// </remarks>
public enum DiscountFrequency
{
    /// <summary>Reduces exactly one invoice, then the redemption is spent.</summary>
    OneTime,

    /// <summary>Reserved - not implemented. Intended to keep reducing every renewal for some duration
    /// once that's built.</summary>
    Recurring
}

/// <summary>A discount code, as the admin catalog and redemption screens see it.</summary>
public class DiscountDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DiscountAmountType AmountType { get; set; }
    public decimal AmountValue { get; set; }
    public DiscountFrequency Frequency { get; set; }
    public int? MaxRedemptions { get; set; }
    public int TimesRedeemed { get; set; }
    public bool OncePerOrganization { get; set; }
    public Guid? RestrictedToTenantId { get; set; }
    public string? RestrictedToOrganizationName { get; set; }
    public DateTimeOffset? ExpiresOn { get; set; }
    public bool IsActive { get; set; }
    public DateTimeOffset CreatedOn { get; set; }
}

/// <summary>Request body for creating a new discount - see <c>DiscountService.CreateAsync</c>.</summary>
public class CreateDiscountRequestDto
{
    [Required]
    public string Code { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Description { get; set; }

    public DiscountAmountType AmountType { get; set; }

    public decimal AmountValue { get; set; }

    public int? MaxRedemptions { get; set; }

    public bool OncePerOrganization { get; set; }

    public Guid? RestrictedToTenantId { get; set; }

    public DateTimeOffset? ExpiresOn { get; set; }
}

/// <summary>Request body for redeeming a code - the same shape self-service and admin-assignment both
/// send, since the underlying rules are identical either way.</summary>
public class RedeemDiscountRequestDto
{
    [Required]
    public string Code { get; set; } = string.Empty;
}

/// <summary>
/// The outcome of a redemption attempt. Always a 200 with this body, never a 4xx for a code that simply
/// doesn't work - "that code has expired" is an ordinary, expected outcome a customer or admin needs to
/// read, not an error condition.
/// </summary>
public class DiscountRedemptionResultDto
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
}
