// Copyright ©2026 Scott Blomfield

using System.Globalization;

namespace RustArchon.Shared.DTOs;

/// <summary>
/// Billing term lengths, expressed as a plain count of months.
/// </summary>
/// <remarks>
/// <para>
/// Deliberately <em>not</em> an enum. This started as one - <c>BillingTerm { Monthly = 1, Quarterly = 3,
/// Annual = 12 }</c> - which quietly made those three the only terms anyone could ever be billed on. The
/// same argument that made <see cref="PlanDto.Name"/> free text applies here: this catalog isn't part of
/// the AGPL-licensed product, so a self-hoster who wants to sell a six-month term shouldn't have to fork
/// the code to get one. As an <c>int</c>, a new term length is data.
/// </para>
/// <para>
/// The constants below are the three RustArchon's own marketing site sells, kept as named values so
/// call sites and seed data stay readable. Nothing enforces them: a plan offers exactly the terms it has
/// <c>PlanPrice</c> rows for, and any positive month count is valid.
/// </para>
/// <para>
/// Month counts rather than days, because that's what date arithmetic needs -
/// <c>periodStart.AddMonths(n)</c> lands on the right calendar date where a fixed day count drifts. A
/// term's length in days is always measured from the resulting period boundaries, never derived from
/// this number, since months differ in length.
/// </para>
/// </remarks>
public static class BillingTerms
{
    public const int Monthly = 1;
    public const int Quarterly = 3;
    public const int Annual = 12;

    /// <summary>
    /// How a term reads in a sentence - "monthly", "annually", "every 6 months". Used wherever a term is
    /// shown to a user, so the same length is never described two different ways in two different places.
    /// </summary>
    public static string Describe(int termMonths) => termMonths switch
    {
        Monthly => "monthly",
        Quarterly => "quarterly",
        6 => "every 6 months",
        Annual => "annually",
        24 => "every 2 years",
        _ => $"every {termMonths.ToString(CultureInfo.InvariantCulture)} months"
    };

    /// <summary>
    /// The noun form, for labelling a period rather than a cadence - "month", "quarter", "year".
    /// </summary>
    public static string DescribePeriod(int termMonths) => termMonths switch
    {
        Monthly => "month",
        Quarterly => "quarter",
        Annual => "year",
        _ => $"{termMonths.ToString(CultureInfo.InvariantCulture)}-month period"
    };

    /// <summary>Whether <paramref name="termMonths"/> is a usable term length.</summary>
    public static bool IsValid(int termMonths) => termMonths is > 0 and <= 120;
}

/// <summary>
/// How a plan's price responds to how much capacity a tenant holds.
/// </summary>
/// <remarks>
/// Both models run through the same arithmetic (see <c>PlanPrice</c>) - a flat tier is just the case
/// where the per-unit amount is zero. This flag exists so the pricing page and admin form know which
/// story to tell ("$14.95/mo" against "$2 per server/mo") and which fields to validate, not because the
/// money is calculated differently.
/// </remarks>
public enum PricingModel
{
    /// <summary>One price, up to a fixed ceiling of servers.</summary>
    Flat = 0,

    /// <summary>A price per server, with no ceiling - capacity is bought rather than capped.</summary>
    PerUnit = 1
}
