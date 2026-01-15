using System.ComponentModel.DataAnnotations;

namespace Corelio.BlazorApp.Models.ViewModels;

/// <summary>
/// View model for the login form.
/// </summary>
public class LoginViewModel
{
    [Required(ErrorMessage = "EmailRequired")]
    [EmailAddress(ErrorMessage = "EmailInvalid")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "PasswordRequired")]
    [MinLength(8, ErrorMessage = "PasswordMinLength")]
    public string Password { get; set; } = string.Empty;
}
