using System.ComponentModel.DataAnnotations;

namespace Corelio.BlazorApp.Models.ViewModels;

/// <summary>
/// View model for the user registration form.
/// </summary>
public class RegisterViewModel
{
    [Required(ErrorMessage = "EmailRequired")]
    [EmailAddress(ErrorMessage = "EmailInvalid")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "PasswordRequired")]
    [MinLength(8, ErrorMessage = "PasswordMinLength")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Required")]
    [Compare(nameof(Password), ErrorMessage = "PasswordsMustMatch")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "FirstNameRequired")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "LastNameRequired")]
    public string LastName { get; set; } = string.Empty;

    public List<string> Roles { get; set; } = [];
}
