namespace Corelio.Domain.Enums;

/// <summary>
/// Payment methods for sales transactions.
/// </summary>
public enum PaymentMethod
{
    /// <summary>
    /// Cash payment.
    /// </summary>
    Cash = 0,

    /// <summary>
    /// Card payment (credit or debit).
    /// </summary>
    Card = 1,

    /// <summary>
    /// Bank transfer.
    /// </summary>
    Transfer = 2,

    /// <summary>
    /// Check payment.
    /// </summary>
    Check = 3,

    /// <summary>
    /// Credit/store credit.
    /// </summary>
    Credit = 4,

    /// <summary>
    /// Mixed payment methods.
    /// </summary>
    Mixed = 5,

    /// <summary>
    /// PayPal payment.
    /// </summary>
    PayPal = 6,

    /// <summary>
    /// Stripe payment.
    /// </summary>
    Stripe = 7
}
