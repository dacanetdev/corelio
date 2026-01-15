using System.ComponentModel.DataAnnotations;

namespace Corelio.BlazorApp.Models.ViewModels;

/// <summary>
/// View model for the forgot password form.
/// </summary>
public class ForgotPasswordViewModel
{
    [Required(ErrorMessage = "EmailRequired")]
    [EmailAddress(ErrorMessage = "EmailInvalid")]
    public string Email { get; set; } = string.Empty;
}
