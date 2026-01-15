using System.ComponentModel.DataAnnotations;

namespace Corelio.BlazorApp.Models.ViewModels;

/// <summary>
/// View model for the reset password form.
/// </summary>
public class ResetPasswordViewModel
{
    public string Email { get; set; } = string.Empty;

    public string Token { get; set; } = string.Empty;

    [Required(ErrorMessage = "PasswordRequired")]
    [MinLength(8, ErrorMessage = "PasswordMinLength")]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Required")]
    [Compare(nameof(NewPassword), ErrorMessage = "PasswordsMustMatch")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
